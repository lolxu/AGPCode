using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace __OasisBlitz.Player.Gauntlets
{
    public class GauntletVisuals : MonoBehaviour
    {
        public GameObject baseGauntlet;
        public GameObject blade;

        public MMF_Player bladeExtendFeedback;
        public MMF_Player bladeRetractFeedback;

        [Description("Duration of trail when blades extended")]
        public float trailDurationExtended;
        [Description("Duration of trail when blades retracted")]
        public float trailDurationRetracted;

        [SerializeField] private TrailRenderer bladeTrail;

        void Awake()
        {
            SetRetractedBlades();
        }

        public void SetExtendedBlades()
        {
            blade.SetActive(true);
            bladeExtendFeedback.PlayFeedbacks();

            bladeTrail.emitting = true;
            bladeTrail.time = trailDurationExtended;
        }

        public void SetRetractedBlades()
        {
            blade.SetActive(false);
            bladeRetractFeedback.PlayFeedbacks();

            bladeTrail.emitting = true;
            bladeTrail.time = trailDurationRetracted;

        }

        public void SetDrill()
        {
            bladeTrail.emitting = false;
            blade.SetActive(false);
        }

        public void UnsetDrill()
        {
            bladeTrail.emitting = true;
        }

    }
}
