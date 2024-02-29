using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Camera.StateMachine.RootStates;
using UnityEngine;

namespace __OasisBlitz.Camera.StateMachine
{
  public enum CameraStates
  {
    SurfaceDefault,
    SurfaceAscending,
    DiveLargePenetrable,
    DiveSmallPenetrable,
    LevelPanCamera
  }

  public class CameraStateFactory
  {
    CameraStateMachine context;
    public Dictionary<CameraStates, CameraBaseState> states = new Dictionary<CameraStates, CameraBaseState>();

    public CameraStateFactory(CameraStateMachine currentContext)
    {
      context = currentContext;
      
      states[CameraStates.SurfaceDefault] = new SurfaceDefaultState(context, this);
      states[CameraStates.SurfaceAscending] = new SurfaceAscendingState(context, this);
      states[CameraStates.DiveLargePenetrable] = new DiveLargePenetrableState(context, this);
      states[CameraStates.DiveSmallPenetrable] = new DiveSmallPenetrableState(context, this);
      states[CameraStates.LevelPanCamera] = new CinematicsCameraState(context, this);

    }
    
    public CameraBaseState SurfaceDefault()
    {
      return states[CameraStates.SurfaceDefault];
    }
    
    public CameraBaseState SurfaceAscending()
    {
      return states[CameraStates.SurfaceAscending];
    }
    
    public CameraBaseState DiveLargePenetrable()
    {
      return states[CameraStates.DiveLargePenetrable];
    }
    
    public CameraBaseState DiveSmallPenetrable()
    {
      return states[CameraStates.DiveSmallPenetrable];
    }

    public CameraBaseState LevelPanCamera()
    {
      return states[CameraStates.LevelPanCamera];
    }

  }
}