using __OasisBlitz.Player.Physics;
using __OasisBlitz.Utility;
using UnityEngine;

namespace __OasisBlitz.Player.StateMachine.SubStates
{
  public class WalkState : BaseState
  {
    public WalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
      : base(currentContext, playerStateFactory)
    {
      StateEnum = PlayerStates.Walk;
    }

    public override void EnterState()
    {
      // Kinda defeats the purpose of this state, shuffling things around while adding slide behavior
      if (CurrentSuperStateEnum == PlayerStates.Grounded)
      {
        Ctx.PlayerPhysics.CurrentDragMode = PlayerPhysics.DragMode.Grounded;
        Ctx.PlayerPhysics.CurrentInputMode = PlayerPhysics.InputMode.Grounded;
        Ctx.PlayerPhysics.CurrentGravityMode = PlayerPhysics.GravityMode.Grounded;
        // Ctx.PlayerAudio.StartWalk();
      }
    }
    public override void UpdateState()
    {
      // Ctx.PlayerAudio.UpdateWalkAudio(Ctx.PlayerPhysics.Velocity.magnitude);
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
      }else if (!Ctx.IsMovementPressed)
      {
        SwitchState(Factory.Idle());
      }
    }

    public override string StateName()
    {
      return "Walk";
    }
  }
}