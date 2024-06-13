using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.old;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.Enemy.StateMachine;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using __OasisBlitz.Utility;
using DG.Tweening;
using Lofelt.NiceVibrations;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyFacePlayerBehavior))]
public class Swarmer : EnemyStateMachine
{
    //TODO: add function that has enemy look around instead of just starring forward
    [SerializeField] private Material _whiteMaterial;
    [SerializeField] protected List<MeshRenderer> toFlash;
    [SerializeField] protected Collider _collider;
    [SerializeField] protected Collider _meleeCollider;
    
    //Animation
    [Header("Animation")] 
    [SerializeField] protected Animator _animator;
    private int awareID;
    private int attackID;

    [Header("Mesh Components for Swarmer")] 
    [SerializeField] private List<GameObject> _meshComponents;
    [SerializeField] private GameObject _animatedComponent;
    [SerializeField] private GameObject _deathVisualComponent;
    
    protected EnemyAudio _enemyAudio;
    private EnemyFacePlayerBehavior enemyFacePlayerBehavior;
    private Vector3 startSpawnLocation;

    private List<Material> ogMaterials = new List<Material>();
    //Set to false if Swarmer Varient has its own death logic
    protected bool NormalDeath = true;
    
    private Sequence shrinkSeq;
    private List<Vector3> orgBodyPartsLocalPos;
    private List<Quaternion> orgBodyPartsLocalRot;
    private List<Vector3> orgBodyPartsLocalScale;
    
    public override void OnAwake()
    {
        base.OnAwake();
        awareID = Animator.StringToHash("aware");
        attackID = Animator.StringToHash("aggro");
        _enemyAudio = GetComponent<EnemyAudio>();
        enemyFacePlayerBehavior = GetComponent<EnemyFacePlayerBehavior>();
        for (int i = 0; i < toFlash.Count; i++)
        {
            ogMaterials.Add(toFlash[i].material);
        }
        StartIdleAnimations();
        
        shrinkSeq = DOTween.Sequence();
        orgBodyPartsLocalPos = new List<Vector3>();
        orgBodyPartsLocalRot = new List<Quaternion>();
        orgBodyPartsLocalScale = new List<Vector3>();
        
        foreach (var mesh in _meshComponents)
        {
            orgBodyPartsLocalPos.Add(mesh.transform.localPosition);
            orgBodyPartsLocalRot.Add(mesh.transform.localRotation);
            orgBodyPartsLocalScale.Add(mesh.transform.localScale);
        }

        if (_deathVisualComponent && _animatedComponent)
        {
            _deathVisualComponent.SetActive(false);
            _animatedComponent.SetActive(true);
        }
        
    }

    private void StartIdleAnimations()
    {
        // foreach (var comp in _meshComponents)
        // {
        //     var moveDir = Random.insideUnitSphere;
        //     comp.transform.DOLocalMove(comp.transform.localPosition + moveDir * 0.5f, Random.Range(1.0f, 3.0f))
        //         .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        // }
        //
        // var headMoveDir = Random.insideUnitSphere;
        // foreach (var comp in _headMeshes)
        // {
        //     comp.transform.DOLocalMove(comp.transform.localPosition + headMoveDir * 0.25f, Random.Range(1.0f, 3.0f))
        //         .SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        // }
    }

    public override void ResetOnEnable()
    {
        base.ResetOnEnable();
        transform.position = startSpawnLocation;
        _collider.enabled = true;

        BackToNormalMaterials();
        shrinkSeq.Kill();
        for (int i = 0; i < _meshComponents.Count; i++)
        {
            _meshComponents[i].transform.localPosition = orgBodyPartsLocalPos[i];
            _meshComponents[i].transform.localRotation = orgBodyPartsLocalRot[i];
            _meshComponents[i].transform.localScale = orgBodyPartsLocalScale[i];
        }

        if (_deathVisualComponent && _animatedComponent)
        {
            _deathVisualComponent.SetActive(false);
            _animatedComponent.SetActive(true);
        }
    }

    public override void SetSpawnLocation()
    {
        startSpawnLocation = transform.position;
    }

    public override void HandleEnterDeath()
    {
        base.HandleEnterDeath();
        if (NormalDeath)
        {
            Kill();
        }
    }
    public override void HandleDeath()
    {
    }

    public override void HandleAlert()
    {

    }

    public override void HandleAlertMove() //handle enemy move, rotation, and tilt
    {
        enemyFacePlayerBehavior.FacePlayer();
    }

    public override void HandleIdleMove()
    {
        //_animator.SetBool(awareID, false);
    }

    public override void HandleEnterAlert()
    {
        base.HandleEnterAlert();
        if (_animator)
            _animator.SetBool(awareID, true);
    }

    public override void HandleEnterAttack()
    {
        base.HandleEnterAttack();
        if (_animator)
            _animator.SetTrigger(attackID);
    }

    public void MeleeChomp()
    {
        if (_meleeCollider.bounds.Contains(playerTransform.position))
        {
            playerStateMachine.InstantKill();
        }
    }

    public override void HandleIdleLOSMove()
    {
        enemyFacePlayerBehavior.FacePlayer();
    }

    public void Kill()
    {
        if (_deathVisualComponent && _animatedComponent)
        {
            _deathVisualComponent.SetActive(true);
            _animatedComponent.SetActive(false);
        }

        DeathFlash();
        _collider.enabled = false;
        
        _enemyAudio.PlayDeathSound();
    }
    
    private void DeathFlash()
    {
        //change material to white
        for (int i = 0; i < toFlash.Count; i++)
        {
            toFlash[i].material = _whiteMaterial;
        }
        StartCoroutine(WaitThenDestroy());
    }

    private void BackToNormalMaterials()
    {
        //change material back
        for (int i = 0; i < toFlash.Count; i++)
        {
            toFlash[i].material = ogMaterials[i];
        }
    }
    
    private IEnumerator WaitThenDestroy()
    {
        PlayerStateMachine psm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
        
        foreach (var bodyMesh in _meshComponents)
        {
            var curRigidbody = bodyMesh.gameObject.GetComponent<Rigidbody>();
            curRigidbody.isKinematic = false;
            curRigidbody.AddForce(Random.insideUnitSphere * psm.PlayerPhysics.Velocity.magnitude, ForceMode.Impulse);
            shrinkSeq.Join(bodyMesh.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InSine).OnComplete(() =>
            {
                bodyMesh.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }));
        }
        
        FeelEnvironmentalManager.Instance.PlayEnemyDeathFeedback(transform.position, Random.Range(0.95f, 1.25f));
        
        yield return new WaitForSeconds(0.15f);
        BackToNormalMaterials();
        
        yield return new WaitForSeconds(0.35f);
        gameObject.SetActive(false);
        
    }
}
