using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.Serialization;

public class BoardBehavior : MonoBehaviour
{
    // This script anchors the board to Bandit's feet -- the main
    // motivation for this is to allow animation to drive the board, instead of just
    // gameobject logic. One possible use would be sick tricks!
    
    public Transform boardAnchor;
    public Transform board;
    public Transform leftFoot;
    public Transform rightFoot;
    
    public bool visible { get; private set; }

    private Vector3 startScale;
    
    TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> currentTween;

    void Awake()
    {
        startScale = transform.localScale;
    }
    
    public void StartBoard()
    {
        // Check if the drill is already visible, to avoid flashing white when certain animations re-trigger this
        gameObject.SetActive(true);
        
        if (!visible)
        {
            currentTween?.Kill();
            currentTween = board.DOScaleX(startScale.x, 0.1f);
            visible = true;
        }

        
    }

    public void StopBoard()
    {
        // Start a dotween to flash white for 0.05s
        currentTween?.Kill();
        currentTween = board.DOScaleX(0, 0.1f).OnComplete(() =>
        {
            gameObject.SetActive(false); 
        });

        visible = false;
    }

    private Vector3 GetBoardUp()
    {
        return Vector3.zero;
    }
    
    
}
