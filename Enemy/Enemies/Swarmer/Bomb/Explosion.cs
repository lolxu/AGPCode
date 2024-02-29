using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private FMODUnity.EventReference explodeEvent;

    public void PlayExplodeSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(explodeEvent, transform.position);
    }
    
    public void TriggerExplosion()
    {
        //particles.Play();
        PlayExplodeSound();
        StopCoroutine(waitThenKill());
    }

    private IEnumerator waitThenKill()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
