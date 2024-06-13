using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Enemy.StateMachine;
using UnityEngine;

public class CallEnemyAnimationEvents : MonoBehaviour
{
    [SerializeField] private Swarmer stateMachine;

    public void MeleeChomp()
    {
        stateMachine.MeleeChomp();
    }
}
