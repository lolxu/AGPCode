using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player.Physics;
using Obi;
using UnityEngine;

namespace __OasisBlitz.Player.StateMachine.SubStates
{
  public class SlideState : BaseState
  {
    public SlideState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
      : base(currentContext, playerStateFactory)
    {
      StateEnum = PlayerStates.Slide;
    }

    public override void EnterState()
    {
      Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Slide;
      Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Slide;

      if (CurrentSuperStateEnum == PlayerStates.Grounded)
      {
        Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.Slide;
      }

      Ctx.IsSliding = true;
      Ctx.BanditAnimationController.SetSliding(true);
      Ctx.ModelRotator.SetSliding(true);

      Ctx.PlayerAudio.StartSlide();
    }

    public override void UpdateState()
    {
      Ctx.DustTrail.SetEmissionRate((Ctx.PlayerPhysics.Velocity.magnitude - Ctx.PlayerPhysics.blitzSpeedThreshold)/(Ctx.PlayerPhysics.blitzSpeedThreshold) * 2.0f);
      CheckSwitchStates();
    }

    public override void ExitState()
    {
      Ctx.IsSliding = false;
      Ctx.BanditAnimationController.SetSliding(false);
      Ctx.ModelRotator.SetSliding(false);
      Ctx.DustTrail.DisableDust();
      Ctx.PlayerAudio.StopSlide();
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchStates()
    {
      if (!Ctx.PlayerPhysics.CheckBlitzSpeed() && !Ctx.OnSlipperySurface)
      {
        SwitchState(Factory.Walk());
      }
    }

    public override string StateName()
    {
      return "Slide";
    }
  }
}
