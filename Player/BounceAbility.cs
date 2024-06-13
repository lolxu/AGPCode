using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.UI;
using __OasisBlitz.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class BounceAbility : MonoBehaviour
{
    struct BounceSphereQueryResult
    {
        public BounceSphereQueryResult(Collider colliderToBounceOffOf, Vector3 colliderClosestPoint, bool foundEnemy)
        {
            ColliderToBounceOffOf = colliderToBounceOffOf;
            ColliderClosestPoint = colliderClosestPoint;
            bFoundEnemy = foundEnemy;
        }
        
        public Collider ColliderToBounceOffOf;
        public Vector3 ColliderClosestPoint;
        public bool bFoundEnemy;
    }
    
    // If true, you need to touch the ground to blast, otherwise, you can bounce as many times as you press in air
    public bool TouchGroundToBlast = false;
    
    public static BounceAbility Instance;
    private PlayerStateMachine ctx;

    private bool _canBounce = true;
    private Tween RefreshTween;
    private bool bIsRefreshing = false;
    
    [SerializeField] public bool BounceEnabled = true;

    [SerializeField] private float DelayAfterExitingToRefreshBounce = .1f;

    [SerializeField] private GameObject BounceParticle;
    [SerializeField] private FMODUnity.EventReference bounceEvent;
    [SerializeField] private HUDManager HUD;

    [SerializeField] private DrillixirIndicator drillixirIndicator;

    private bool initialSet;
    /*
     * Properties for the different jumps:
     */
    // The highest speed you can reach by spamming bounce -- to prevent super high ground speeds
    public float MaxBounceSpeed = 100f;
    [Header("Standing Boost")]

    [Header("Air Boost Forwards")]
    public float ForwardsForwardsVelocity = 50.0f;
    public float ForwardsUpwardsVelocity = 20.0f;
    
    [Header("Air Boost Backwards")]
    public float BackwardsForwardsVelocity = 50.0f;
    public float BackwardsUpwardsVelocity = 60.0f;
    
    [Header("Air Boost Neutral")]
    public float NeutralForwardsVelocity = 25.0f;
    public float NeutralUpwardsVelocity = 40.0f;

    [Header("Drillixir Usage")]
    [Range(0.0f, 1.0f)] public float DrillixirCostAsPercent = 1.0f;
    public float MinimumDrillixir = 1.0f;

    [SerializeField] private float inputLeniancy = .4f;

    public bool DrawCollisionSphere = true;
    public bool DrawNearestImpactRaycast = true;
    public bool DrawImpactNormal = true;
    public bool DrawBounceVelocity = true;

    public class BounceAttemptResult
    {
        public bool DidBounce;
        public Vector3 BounceVelocity;

        public BounceAttemptResult()
        {
            DidBounce = false;
            BounceVelocity = Vector3.zero;
        }
    }
    
    public bool CanBounce
    {
        get
        {
            return _canBounce;
        }

        set
        {
            if (!value || (value && !bIsRefreshing))
            {
                _canBounce = value;
                if (_canBounce)
                {
                    HUD.SetBlastAvailable();
                    if (initialSet)     // If first canBounce is already set, go ahead and play the sound
                    {
                        drillixirIndicator.PlayBlastReady();
                    }
                    else
                    {
                        initialSet = true;
                    }
                }
                else
                {
                    HUD.SetBlastNotAvailable();
                }
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += RefreshBounce;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= RefreshBounce;
    }

    void RefreshBounce(Scene scene, LoadSceneMode mode)
    {
        RefreshBounce();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        CanBounce = true;
        initialSet = false;
        ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
    }

    public Material[] _outlineMaterials;

    private void UpdateOutlineShader(bool enabled)
    {
        //_OutlineSize
        //_OutlineColor
    }
    
    private void ConsumeBounce()
    {
        CanBounce = false;
        
    }

    public void RefreshBounce()
    {
        if (!_canBounce)
        {
            CanBounce = true;
        }

        // if (!_canBounce && !bIsRefreshing)
        // {
        //     bIsRefreshing = true;
        //     RefreshTween = DOVirtual.DelayedCall(DelayAfterExitingToRefreshBounce,
        //         ()=>
        //     {
        //         bIsRefreshing = false;
        //         CanBounce = true;
        //         DrillixirManager.AddDrillixirCharge();
        //     }, false);
        // }
    }

    public BounceAttemptResult AirBounce()
    {
        BounceAttemptResult bounceAttemptResult = new BounceAttemptResult();

        if (!_canBounce)
        {
            return bounceAttemptResult;
        }
        
        ConsumeBounce();

        // check of result of query
        Vector3 PlayerPos = ctx.transform.position;
        
        bounceAttemptResult.DidBounce = true;
        bounceAttemptResult.BounceVelocity = PerformDoubleJump();

        return bounceAttemptResult;
    }

    /// <summary>
    /// Do a completely vertical blast
    /// </summary>
    /// <returns> The velocity added (for use when displaying visuals) </returns>
    private Vector3 PerformDoubleJump()
    {
        // bounce of the air (double jump)
        
        Vector3 RemoveNegativeYVelocity = ctx.PlayerPhysics.Velocity;
        if (RemoveNegativeYVelocity.y < 0)
        {
            RemoveNegativeYVelocity.y = 0;
        }
        // remove y element of current velocity
        ctx.PlayerPhysics.SetVelocity(RemoveNegativeYVelocity);

        Vector3 AddedVelocity = GetVelocityWithInput();

        ctx.PlayerPhysics.AddVelocity(AddedVelocity, MaxResultantSpeed:MaxBounceSpeed);
            
        BounceFeedback(ctx.transform.position, AddedVelocity);

        InLevelMetrics.Instance?.LogEvent(MetricAction.Blast);
        
        return AddedVelocity;
    }

    private Vector3 GetVelocityWithInput()
    {
        float inputDot = Vector3.Dot(ctx.PlayerPhysics.Velocity.normalized, ctx.MovementInput.normalized);
        
        Vector3 AddedVelocity = Vector3.up;
        
        if (inputDot > inputLeniancy)
        {
            // forwards bounce
            AddedVelocity = ctx.MovementInput * ForwardsForwardsVelocity;
            AddedVelocity += Vector3.up * ForwardsUpwardsVelocity;

        }
        else if (inputDot < -inputLeniancy)
        {
            // backwards bounce
            AddedVelocity = ctx.MovementInput * BackwardsForwardsVelocity;
            AddedVelocity += Vector3.up * BackwardsUpwardsVelocity;
        }
        else
        {
            // neutral bounce
            AddedVelocity = ctx.MovementInput * NeutralForwardsVelocity;
            AddedVelocity += Vector3.up * NeutralUpwardsVelocity;
        }

        return AddedVelocity;
    }

    private void BounceFeedback(Vector3 BouncePoint, Vector3 BounceDir)
    {
        // play visual / audio feedback
        // play particles in the direction of velocity at the point of impact:
        GameObject bounceParticles = Instantiate(BounceParticle, BouncePoint, Quaternion.identity);
        // GameObject bounceLine = Instantiate(BounceLine, BouncePoint, Quaternion.identity);
        // bounceLine.GetComponent<BounceLine>().Init(BouncePoint, BouncePoint - BounceDir.normalized * 4.0f);
        FMODUnity.RuntimeManager.PlayOneShot(bounceEvent, BouncePoint);
        
        bounceParticles.transform.localScale = ctx.transform.localScale;
        // hitBoxParticles.transform.rotation = PlayerTransform.rotation;  // todo: make this the normal of the exited surf.
        
        bounceParticles.transform.rotation = Quaternion.FromToRotation(Vector3.up, -BounceDir);
        bounceParticles.transform.position = BouncePoint;
        bounceParticles.GetComponent<ParticleSystem>().Play();
    }
}
