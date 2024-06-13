using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.Player.StateMachine;
using UnityEngine;

public class WaterRipples : MonoBehaviour
{
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    [SerializeField] private PlayerStateMachine Ctx;
    public void LeftFootRipple()
    {
        if (Ctx.InWaterTrigger)
        {
            FeelEnvironmentalManager.Instance.PlayWaterRippleFeedback(leftFoot.position, 1.0f);
        }
    }
        
    public void RightFootRipple()
    {
        if (Ctx.InWaterTrigger)
        {
            FeelEnvironmentalManager.Instance.PlayWaterRippleFeedback(rightFoot.position, 1.0f);
        }
    }

    public void StartIdleRipples()
    {
        if (!Ctx)
        {
            return;
        }
        if (Ctx.InWaterTrigger)
        {
            FeelEnvironmentalManager.Instance.PlayWaterIdleRippleFeedback(leftFoot.position, rightFoot.position);
        }
    }

    public void StopIdleRipples()
    {

        FeelEnvironmentalManager.Instance.StopWaterIdleRippleFeedback();
    }
}
