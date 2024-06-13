using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

public class BanditAnimations : MonoBehaviour
{
    public enum BanditAnimationTypes
    {
        nullAnim,
        Idle,
        Run,
        Jump,
        Slide,
        Blast,
        GrappleStart,
        GrappleImpact,
        DrillStart,
        Drill,
        DrillEnd,
        Freefall,
        Celebrate
    }

    [System.Serializable]
    public class AnimationMapping
    {
        public BanditAnimationTypes key;
        public ClipTransition value;
    }

    public List<AnimationMapping> mappings;

    void Reset()
    {
        // Initialize the animation list with all enum values when the component is first added
        mappings = new List<AnimationMapping>();
        foreach (BanditAnimationTypes anim in Enum.GetValues(typeof(BanditAnimationTypes)))
        {
            mappings.Add(new AnimationMapping() { key = anim });
        }
    }
    
    public ClipTransition GetAnimation(BanditAnimationTypes type)
    {
        foreach (var mapping in mappings)
        {
            if (mapping.key == type)
            {
                return mapping.value;
            }
        }

        return null;
    }
}
