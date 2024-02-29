using __OasisBlitz.Player.Physics;
using __OasisBlitz.Utility;
using UnityEngine;

namespace __OasisBlitz.Player.StateMachine.SubStates
{
  public class IdleState : BaseState
  {
    public IdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
      : base(currentContext, playerStateFactory)
    {
      StateEnum = PlayerStates.Idle;
    }
    public override void EnterState()
    {
      Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Idle;
      Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Idle;
      Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.Idle;
      // Ctx.PlayerAudio.StartWalk();
    }

    public override void UpdateState()
    {
      if (Ctx.PlayerPhysics.Velocity.magnitude > 0.0f)
      {
        // Ctx.PlayerAudio.UpdateWalkAudio(Ctx.PlayerPhysics.Velocity.magnitude);
      }
      CheckSwitchStates();
    }
    public override void ExitState()
    {
      // Ctx.PlayerAudio.StopWalk();
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchStates()
    {
      if (Ctx.ToggleSlide && Ctx.PlayerPhysics.CheckBlitzSpeed() || Ctx.OnSlipperySurface)
      {
        SwitchState(Factory.Slide());
      }
      else if (Ctx.IsMovementPressed)
      {
        SwitchState(Factory.Walk());
      }
    }

    public override string StateName()
    {
      return "Idle";
    }
  }
}