using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.Physics;
using __OasisBlitz.Player.StateMachine;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Player.Environment.Cannon
{
    public class Cannon : MonoBehaviour
    {
        [SerializeField] protected Transform exitPoint;
        [SerializeField] protected Transform scalePoint;
        [SerializeField] protected Collider cannonCollider;
        
        [SerializeField] private Material enabledMaterial;
        [SerializeField] private Material disabledMaterial;
        public MeshRenderer _mesh;

        [SerializeField] protected float transitionTime = .2f;
        [SerializeField] protected float buildUpTime = .6f;
        [SerializeField] protected float launchVelocity = 100.0f;
        
        //Cannon Animation
        [SerializeField] protected float horizontalScale = 1.2f;
        [SerializeField] protected float verticalScale = 0.6f;
        [SerializeField] protected float localYMoveTo = 1.8f;
        [SerializeField] protected float localYTipMoveTo = 0.5f;
        [SerializeField] private Transform pedals1;
        [SerializeField] private Transform pedals2;
        [SerializeField] private Transform shaft;
        [SerializeField] private Transform tip;
        protected Vector3 initialScale;
        protected float initialLocalY;
        protected float initialLocalYTip;

        private TargetedDash _dash;
        protected PlayerStateMachine ctx;
        protected float initialScaleY;

        private bool bCanEnterCannon = true;

        protected Sequence CannonSequence = null;

        private void Start()
        {
            pedals1.DOLocalRotate(new Vector3(0f, 360.0f, 0f), 80.0f, RotateMode.FastBeyond360)
                .SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
            pedals2.DOLocalRotate(new Vector3(0f, -360.0f, 0f),60.0f, RotateMode.FastBeyond360)
                .SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }

        private void OnEnable()
        {
            RespawnManager.OnReset += KillCannonSequence;
        }

        private void OnDisable()
        {
            RespawnManager.OnReset -= KillCannonSequence;
        }

        protected virtual void KillCannonSequence()
        {
            // reset cannon properties
            CannonSequence?.Kill();
        }

        protected void ResetCannon()
        {
            cannonCollider.enabled = true;
            bCanEnterCannon = true;
            _dash.DashEnabled = true;
        }
        
        // cannon sequence methods begin

        protected void InitializePlayerAndCannon()
        {
            EnterCannon();

            PlayerPhysics phys = ctx.PlayerPhysics;

            //cancel out acceleration and prev velocity
            phys.ZeroAcceleration();
            phys.SetVelocity(Vector3.zero);

            // store player's ability to bounce, to be restored after exiting cannon:
            BounceAbility.Instance.CanBounce = false;
        }

        protected void SetCameraToCannon()
        {
            float cameraTransitionTime = .25f;
            Vector3 CameraForward = transform.up;

            CameraStateMachine.Instance.SetToLookAtTarget(transform.position + CameraForward * 100.0f);
        }

        protected void ExitCannon()
        {
            PlayerPhysics phys = ctx.PlayerPhysics;
            
            // release and launch player
            ctx.CharacterController.RequestJump();
            phys.ZeroAcceleration();
            phys.SetVelocity(transform.up * launchVelocity);
            BounceAbility.Instance.RefreshBounce();

            OnExitCannon();
        }
        
        // cannon sequence methods end

        protected virtual void CreateCannonSequence()
        {
            CannonSequence = DOTween.Sequence();

            CannonSequence.AppendCallback(InitializePlayerAndCannon);
            CannonSequence.Append(DOTween.To(() => ctx.CharacterController.transform.position,
                x => ctx.CharacterController.SetPosition(x),
                tip.position, transitionTime));
            CannonSequence.AppendCallback(SetCameraToCannon);

            CannonSequence.AppendCallback(OnLaunchCannonAction);
            
            // launch animation
            // CannonSequence.Append(
            //     scalePoint.transform.DOScaleY(initialScaleY * .2f,
            //         (7 * buildUpTime) / 8.0f).SetEase(Ease.OutCubic).OnComplete(() =>
            //         scalePoint.transform.DOScaleY(initialScaleY, buildUpTime / 8.0f).SetEase(Ease.InOutCubic)));
            CannonSequence.Append(
                shaft.DOScale(new Vector3(initialScale.x * horizontalScale, initialScale.y * verticalScale, initialScale.z * horizontalScale),
                    (7 * buildUpTime) / 8.0f).SetEase(Ease.OutCubic).OnComplete(() =>
                    shaft.DOScale(initialScale, buildUpTime / 8.0f).SetEase(Ease.OutElastic)));
            CannonSequence.Join(shaft.DOLocalMoveY(localYMoveTo,
                (7 * buildUpTime) / 8.0f).SetEase(Ease.OutCubic).OnComplete(() =>
                shaft.DOLocalMoveY(initialLocalY, buildUpTime / 8.0f).SetEase(Ease.OutElastic)));
            CannonSequence.Join(tip.DOLocalMoveY(localYTipMoveTo,
                (7 * buildUpTime) / 8.0f).SetEase(Ease.OutCubic).OnComplete(() =>
                tip.DOLocalMoveY(initialLocalYTip, buildUpTime / 8.0f).SetEase(Ease.OutElastic)));
            // have player pos match 
            CannonSequence.Join(DOTween.To(() => ctx.CharacterController.transform.position,
                x => ctx.CharacterController.SetPosition(tip.position),
                tip.position,
                buildUpTime));

            // actual launch code
            CannonSequence.AppendCallback(ExitCannon);
            // re-enable the cannon to use it again after a little
            CannonSequence.Append(DOVirtual.DelayedCall(.5f, () => OnPostExitCannon(), false));

            // do not lock player to cannon if reset, reset collider and scale
            CannonSequence.OnKill(() =>
            {
                ResetCannon();
            });
        }
        
        public void BeginLaunchRoutine()
        {
            if (bCanEnterCannon)
            {
                CreateCannonSequence();
            }
        }
        
        private void Awake()
        {
            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
            _dash = GameObject.FindGameObjectWithTag("Player").GetComponent<TargetedDash>();
            initialScale = shaft.localScale;
            initialLocalY = shaft.localPosition.y;
            initialLocalYTip = tip.localPosition.y;
            // CreateCannonSequence();
        }

        // properties the player and cannon should have while the player is inside of it
        private void EnterCannon()
        {
            // stop a dash when the player enters a cannon
            _dash.EndDash();
            _dash.DashEnabled = false;

            // Play cannon enter sound
            AudioManager.instance.PlayOneShot(FMODEvents.instance.cannonEnter, transform.position);
            
            bCanEnterCannon = false;
            cannonCollider.enabled = false;
            OnEnterCannonAction();
        }

        public void SetEnabledMaterial(bool isEnabled)
        {
            if (isEnabled)
            {
                _mesh.material = enabledMaterial;
            }
            else
            {
                _mesh.material = disabledMaterial;
            }
        }

        // called on the same frame as exiting the cannon
        private void OnExitCannon()
        {
            // re-enable player vulnerability
            ctx.DrillixirManager.FullRefillDrillixir();
            AudioManager.instance.PlayOneShot(FMODEvents.instance.cannonBoom, transform.position);
            CameraStateMachine.Instance.SetToLookAtVelocity();
        }

        // called a fraction of a second after exiting the cannon
        protected void OnPostExitCannon()
        {
            // re-enable cannon collider
            ResetCannon();

            PostExitCannonAction();
        }
        
        /*
         *
         * Override section for action hooks
         * 
         */

        /// <summary>
        /// Implement to have things trigger after exiting cannons
        /// </summary>
        protected virtual void PostExitCannonAction() { }

        /// <summary>
        /// Implement to do stuff when entering cannons
        /// </summary>
        protected virtual void OnEnterCannonAction() { }

        protected virtual void OnLaunchCannonAction()
        {
        }
    }
}
