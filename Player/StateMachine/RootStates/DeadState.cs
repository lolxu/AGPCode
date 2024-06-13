using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.Enemies;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Camera.StateMachine.RootStates;
using __OasisBlitz.LevelLoading;
using __OasisBlitz.Utility;
using DG.Tweening;
using Lofelt.NiceVibrations;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.Player.StateMachine.RootStates
{
    public class DeadState : BaseState, IRootState
    {
        
        // TODO: This is messy because something about the interaction between transform and character controller
        // was messing things up. Find a more elegant solution
        //private float framesSinceRespawn;
        private bool InTheProcessOfDying = false;

        public DeadState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
            StateEnum = PlayerStates.Dead;
            IsRootState = true;
        }

        public override void EnterState()
        {
            UIManager.Instance.canPauseGame = false;
            Ctx.PlayerAudio.PlayDeathSound();
            Ctx.IsDead = true;
            if (!Ctx.DeathByBarrier)
            {
                //Set velocity to zero when dead
                Ctx.BanditAnimationController.PlayDeath();
                Ctx.PlayerPhysics.SetVelocity(Vector3.zero);
                Ctx.PlayerPhysics.SetStuck(Ctx.TimeTillDeathFadeToBlack, Vector3.zero);
                
                Ctx.PlayerFeedbacks.deathFeedback.PlayFeedbacks(Ctx.ModelRotator.transform.position);
                Ctx.ModelRotator.BanditDeathModelSequence();
                
                // Ctx.DrillMeshInstance.SetActive(true);
                // Ctx.DrillMeshInstance.transform.position = Ctx.ModelRotator.transform.position;
                // Ctx.DrillMeshInstance.GetComponent<Rigidbody>().AddForce(Random.Range(15.0f, 35.0f) * Random.insideUnitSphere, ForceMode.Impulse);
            }
            else
            {
                // We freeze the camera by changing to cinematics camera
                CameraStateMachine.Instance.SwitchToCinematicsCamera(CinematicsType.DeathPan);
            }
            
            HapticsManager.Instance.PlayPlayerDeathHaptic();
            InTheProcessOfDying = false;
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState()
        {
            Ctx.IsDead = false;
            // in my opinion (john) the ideas of code should be more tied to their names in this area
            // yes
            KeyValuePair<Vector3, Vector3> spawnPos = RespawnManager.Instance.Respawn();
            RespawnCharacter(spawnPos);
            Ctx.ModelRotator.RevealBandit();
            // Ctx.DrillMeshInstance.SetActive(false);
            // Ctx.DrillixirManager.FullRefillDrillixir();
            //Set velocity to zero when dead
            Ctx.PlayerPhysics.SetVelocity(Vector3.zero);
            UIManager.Instance.canPauseGame = true;
        }

        public override void CheckSwitchStates()
        {
            if (Ctx.PlayerPhysics.isStuckTimer <= LevelManager.Instance.m_deathTransitionDuration && !Ctx.DeathByBarrier && !InTheProcessOfDying)
            {
                InTheProcessOfDying = true;
                //Transition To Black
                HUDManager.Instance.GetSceneTransitionImage().DOFade(1.0f, LevelManager.Instance.m_deathTransitionDuration)
                    .SetEase(Ease.InOutQuad).OnComplete(SwtichToGroundState);
            }
        }

        private void SwtichToGroundState()
        {
            SwitchState(Factory.Grounded());
            HUDManager.Instance.GetSceneTransitionImage().DOFade(0.0f, LevelManager.Instance.m_deathTransitionDuration * 0.5f)
                .SetEase(Ease.InOutQuad);
        }

        public override void InitializeSubState()
        {
        }

        public override string StateName()
        {
            return "Dead";
        }

        public void HandleGravity()
        {
        }
    }
}