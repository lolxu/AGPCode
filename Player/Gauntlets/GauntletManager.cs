using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using __OasisBlitz.Player;
using MoreMountains.Tools;
using UnityEngine;

namespace __OasisBlitz.Player.Gauntlets
{
    public class GauntletManager : MonoBehaviour
    {
        public GauntletVisuals leftGauntlet;
        public GauntletVisuals rightGauntlet;
        public DrillBehavior drill;
        
        [SerializeField]
        private string LeftWristBoneName = "left_palm_ik_handle";
        [SerializeField]
        private string RightWristBoneName = "right_palm_ik_handle";

        private Vector3 LeftStartPositionOffset;
        private Quaternion LeftStartRotation;

        private Transform leftGauntletBone;
        private Transform rightGauntletBone;

        void Awake()
        {

            LeftStartPositionOffset = leftGauntlet.transform.localPosition;
            LeftStartRotation = leftGauntlet.transform.rotation;

            // Get bones by name (usually we try to avoid finding by name, but in this case we want to automatically
            // set this up when importing a new version of the rig)
            leftGauntletBone = transform.MMFindDeepChildDepthFirst(LeftWristBoneName);
            rightGauntletBone = transform.MMFindDeepChildDepthFirst(RightWristBoneName);
            leftGauntlet.transform.SetParent(leftGauntletBone.transform, true);
            rightGauntlet.transform.SetParent(rightGauntletBone.transform, true);

            // leftGauntlet.transform.localPosition = LeftStartPositionOffset;
            // rightGauntlet.transform.localPosition = LeftStartRotation;

            // leftGauntlet.transform.localRotation = LeftStartRotation;
            // rightGauntlet.transform.localRotation = Quaternion.identity;

        }

        public void ExtendBlades()
        {
        }

        public void RetractBlades()
        {

        }

        // public void FormDrill()
        // {
        //     extended = false;
        //     
        //     drill.SetDrillVisible();
        //     drill.StartDrillSpin();
        //     leftGauntlet.SetDrill();
        //     rightGauntlet.SetDrill();
        //
        // }
        //
        // public void UnformDrill()
        // {
        //     leftGauntlet.UnsetDrill();
        //     rightGauntlet.UnsetDrill();
        //     drill.StopDrillSpin();
        //     drill.SetDrillInvisible();
        // }

    }
}