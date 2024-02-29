using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using Animancer;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class DashTargetPoint : MonoBehaviour
{
    [SerializeField] private float shrunkScale = 0.0f;
    [SerializeField] private float fullScale = 1.0f;
    [SerializeField] private float growTime = 0.3f;
    [SerializeField] private float shrinkTime = 0.3f;
    [SerializeField] private float furthestRenderDistance = 10.0f;

    [Description("The radius of the invisible sphere that the drill will attach to when dashing to this point")]
    public float drillAttachRadius;

    private Vector3 startLocalScale;
    private Renderer myRenderer;
    private Material orgMat;
    private Sequence scaleSequence; // To control the tween

    public bool isValidTarget = true;
    private bool isInRange = false;
    private bool isDashTarget = false;
    private float matDist = 3.0f;
    private CameraStateMachine cameraStateMachine;
    private TweenerCore<Vector3, Vector3, VectorOptions> scaleTween;
    private Vector3 beforeFlashScale;
    private float distanceOffset = 0.0f;

    void Awake()
    {
        startLocalScale = transform.localScale;
        
        // Start in shrunken form
        transform.localScale = startLocalScale * shrunkScale;
        myRenderer = GetComponent<MeshRenderer>();
        orgMat = myRenderer.material;
    }

    void OnEnable()
    {
        isValidTarget = true;
        // RespawnManager.OnReset += UnsetAsTarget;
        // RespawnManager.OnReset += UnsetDashable;
    }
    
    /// <summary>
    /// Enable the gameObject and use the spring to make it grow
    /// </summary>
    public void SetAsTarget(float actualDashableRadius)
    {
        distanceOffset = actualDashableRadius;
        
        Vector3 targetScale = startLocalScale * fullScale;
        
        scaleSequence?.Kill();
        
        scaleSequence = DOTween.Sequence();
        
        scaleSequence.Append(transform.DOScale(targetScale, growTime).SetEase(Ease.OutExpo));

        isInRange = true;
    }

    /// <summary>
    /// Shrink the gameObject and then disable it
    /// </summary>
    public void UnsetAsTarget()
    {
        if (gameObject)
        {
            Vector3 targetScale = startLocalScale * shrunkScale;

            scaleSequence?.Kill();

            scaleSequence = DOTween.Sequence();

            scaleSequence.Append(transform.DOScale(targetScale, shrinkTime).SetEase(Ease.OutExpo));

            isInRange = false;
        }
    }

    /// <summary>
    /// Function to set the material to indicate it's dashable
    /// </summary>
    public void SetDashable()
    {
        if (!isDashTarget)
        {
            myRenderer.material.SetInt("_IsInRange", 1);
            // beforeFlashScale = startLocalScale * fullScale;
            // scaleTween = transform.DOScale(transform.localScale * 1.15f, 0.2f)
            //     .SetLoops(-1, LoopType.Yoyo);
            isDashTarget = true;
        }
        HUDManager.Instance.SetDisplayDashPrompt(true);
    }

    /// <summary>
    /// Function to set the material to indicate it's not dashable
    /// </summary>
    public void UnsetDashable()
    {
        // Debug.Log("Here");
        if (isDashTarget)
        {
            myRenderer.material.SetInt("_IsInRange", 0);
            scaleTween.Kill(true);
            // transform.localScale = beforeFlashScale;
            isDashTarget = false;
        }
    }

    private void Update()
    {
        if (isInRange)
        {
            float distance = Vector3.Distance(transform.position, CameraStateMachine.Instance.CameraSurface.transform.position);
            matDist = Mathf.Clamp01((distance - distanceOffset) / furthestRenderDistance) * 3.0f - 0.5f;
            // Debug.Log(matDist);
            myRenderer.material.SetFloat("_Distance", matDist);
        }
    }
}
