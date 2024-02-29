using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

public class FlipAnimation : MonoBehaviour
{
    public AnimationClip rollPose;
    public AnimationClip drillPose;
    public AnimationClip jumpPose;

    private LinearMixerState drillIntoJump;
    private LinearMixerState jumpIntoDrill;
    
    [SerializeField] private AnimancerComponent animancer;
    
    private LinearMixerState currentFlipMixer;
    
    
        
    // Start is called before the first frame update
    void Start()
    {
        drillIntoJump = new LinearMixerState();
        drillIntoJump.Add(drillPose, 0);
        drillIntoJump.Add(rollPose, 0.1f);
        // jumpIntoDrill.Add(rollPose, 0.7f);
        drillIntoJump.Add(jumpPose, 1.0f);
        
        jumpIntoDrill = new LinearMixerState();
        jumpIntoDrill.Add(jumpPose, 0);
        jumpIntoDrill.Add(rollPose, 0.1f);
        // jumpIntoDrill.Add(rollPose, 0.7f);
        jumpIntoDrill.Add(drillPose, 1.0f);
    }

    public void FlipJumpToDrill()
    {
        currentFlipMixer = jumpIntoDrill;
        animancer.Play(currentFlipMixer);
        currentFlipMixer.Parameter = 1;
    }

    public void FlipDrillIntoJump()
    {
        currentFlipMixer = drillIntoJump;
        animancer.Play(currentFlipMixer);
    }

    public void SetFlipProgress(float flipProgress)
    {
        if (currentFlipMixer == null) return;
        currentFlipMixer.Parameter = flipProgress;   
    }
}
