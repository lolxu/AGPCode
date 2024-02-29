using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Enemy.Enemies;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using __OasisBlitz.LevelLoading;
using __OasisBlitz.Utility;
using DG.Tweening;
using Lofelt.NiceVibrations;
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
            Ctx.PlayerAudio.PlayDeathSound();
            Ctx.BanditAnimationController.PlayDeath();
            Ctx.IsDead = true;
            if (!Ctx.DeathByBarrier)
            {
                //Set velocity to zero when dead
                Ctx.PlayerPhysics.SetStuck(Ctx.TimeTillDeathFadeToBlack, Vector3.zero);
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
            KeyValuePair<Vector3, Vector3> spawnPos = RespawnManager.Instance.Respawn();
            RespawnCharacter(spawnPos);
            Ctx.DrillixirManager.FullRefillDrillixir();
            //Set velocity to zero when dead
            Ctx.PlayerPhysics.SetVelocity(Vector3.zero);
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