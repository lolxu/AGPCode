using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceKnightSounds : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference shieldSpikeEvent;

    public void PlayShieldSpikeSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(shieldSpikeEvent, transform.position);
    }
}
