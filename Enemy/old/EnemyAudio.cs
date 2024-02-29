using UnityEngine;

namespace __OasisBlitz.__Scripts.Enemy.old
{
    public class EnemyAudio : MonoBehaviour
    {

        public void PlayDeathSound()
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyHurt, transform.position);
        }

        public void PlayShoot()
        {
            FMODUnity.RuntimeManager.PlayOneShot(FMODEvents.instance.enemyShoot, transform.position);
        }
    }
}