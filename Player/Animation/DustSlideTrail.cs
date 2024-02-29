using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public class DustSlideTrail : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> DustParticles;
    private int currDust = 0;
    public void SetEmissionRate(float Norm)
    {
        Norm = Mathf.Clamp(Norm, 0.3f, 1.0f);
        int newDust = (int)(Norm / 0.05f) - 6;
        if (newDust != currDust)
        { 
            DustParticles[currDust].Stop();
            DustParticles[newDust].Play();
            currDust = newDust;
        }else if (!DustParticles[newDust].isPlaying)
        {
            DustParticles[newDust].Play();
        }
    }

    public void DisableDust()
    {
        DustParticles[currDust].Stop();
    }
}
