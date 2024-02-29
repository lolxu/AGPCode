using System.Collections.Generic;
using __OasisBlitz.Player.StateMachine.RootStates;
using __OasisBlitz.Player.StateMachine.SubStates;

namespace __OasisBlitz.Player.StateMachine
{
  public enum PlayerStates
  {
    Idle,
    Walk,
    Slide,
    RestrictedHorizontalMovement,
    Drill,
    DrillAbove,
    DrillBelow,
    Dash,
    FreeFall,
    Grounded,
    Dead,
    Ball
  }

  public class PlayerStateFactory
  {
    PlayerStateMachine context;
    public Dictionary<PlayerStates, BaseState> states = new Dictionary<PlayerStates, BaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
      context = currentContext;
      states[PlayerStates.Idle] = new IdleState(context, this);
      states[PlayerStates.Walk] = new WalkState(context, this);
      states[PlayerStates.Slide] = new SlideState(context, this);
      states[PlayerStates.RestrictedHorizontalMovement] = new RestrictedHorizontalMovementState(context, this);
      states[PlayerStates.Drill] = new DrillState(context, this);
      states[PlayerStates.DrillAbove] = new DrillAboveState(context, this);
      states[PlayerStates.DrillBelow] = new DrillBelowState(context, this);
      states[PlayerStates.Dash] = new DashState(context, this);
      states[PlayerStates.FreeFall] = new FreeFallState(context, this);
      states[PlayerStates.Grounded] = new GroundedState(context, this);
      states[PlayerStates.Dead] = new DeadState(context, this);
      states[PlayerStates.Ball] = new BallState(context, this);
    }
    public BaseState Idle()
    {
      return states[PlayerStates.Idle];
    }
    public BaseState Walk()
    {
      return states[PlayerStates.Walk];
    }
    public BaseState Slide()
    {
      return states[PlayerStates.Slide];
    }
    public BaseState Dash()
    {
      return states[PlayerStates.Dash];  
    }
    public BaseState Drill()
    {
      return states[PlayerStates.Drill];
    }
    public BaseState DrillAbove()
    {
      return states[PlayerStates.DrillAbove];
    }
    
    public BaseState DrillBelow()
    {
      return states[PlayerStates.DrillBelow];
    }
    
    public BaseState FreeFall()
    {
      return states[PlayerStates.FreeFall];
    }

    public BaseState Grounded()
    {
      return states[PlayerStates.Grounded];
    }

    public BaseState Dead()
    {
      return states[PlayerStates.Dead];
    }

    public BaseState Ball()
    {
      return states[PlayerStates.Ball];
    }
  }
}