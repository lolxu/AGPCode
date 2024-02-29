using System.Collections;
using UnityEngine;

namespace __OasisBlitz.__Scripts.Enemy.old
{
    public class BasicEnemyController : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Material _whiteMaterial;

        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;

        private EnemyAudio _enemyAudio;
    
        // Start is called before the first frame update
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _enemyAudio = GetComponent<EnemyAudio>();
        }

        // Update is called once per frame
        void Update()
        {
            Quaternion targetRotation = Quaternion.LookRotation(_player.position - transform.position);
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }

        public void Kill()
        {
            DeathFlash();
            _collider.enabled = false;
            _enemyAudio.PlayDeathSound();
        
        }

        private void DeathFlash()
        {
            _meshRenderer.material = _whiteMaterial;
            StartCoroutine(WaitThenDestroy());
        }

        private IEnumerator WaitThenDestroy()
        {
            yield return new WaitForSeconds(0.25f);
            Destroy(gameObject);
        }
    }
}
