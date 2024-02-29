using System.Collections;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Enemy.old
{
    public class ProjectileLauncher : MonoBehaviour
    {
        [SerializeField] private Transform _player;

        private bool _canShoot;

        [SerializeField] private float range;

        [SerializeField] private GameObject bulletPrefab;

        [SerializeField] private float cooldownTime;

        [SerializeField] private float projectileSpeed;
        
        [SerializeField] private EnemyAudio _enemyAudio;

        // Start is called before the first frame update
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _canShoot = true;
        }
        // Update is called once per frame
        void Update()
        {
            Quaternion targetRotation = Quaternion.LookRotation(_player.position - transform.position);
            transform.rotation = targetRotation;

            if (((_player.position - transform.position).magnitude < range) && _canShoot)
            {
                Debug.Log("Shooting!");
                Shoot();
                StartCoroutine(ShotCooldown());
            }
        }

        private IEnumerator ShotCooldown()
        {
            _canShoot = false;
            yield return new WaitForSeconds(cooldownTime);
            _canShoot = true;
        }

        private void Shoot()
        {
            Vector3 shootDirection = (_player.position - transform.position).normalized;
            GameObject bullet = Instantiate(bulletPrefab, transform.position + shootDirection * 2f, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity =
                shootDirection * projectileSpeed;

            _enemyAudio.PlayShoot();
        }
    }
}