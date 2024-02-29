using System.Collections;
using UnityEngine;

public class DangerSphereSpawner : MonoBehaviour
{
    // todo: make this pooled just in case so you don't have to keep reinstantiating it
    public GameObject dangerSphere;
    public Transform[] spawnPoints;
    public float spawnTimer = 1.0f;  // spawn one sphere every 1 second
    public float despawnTimer = 20.0f;  // how long to wait before despawning spheres.

    private void Start()
    {
        StartCoroutine(SpawnSpheres());
    }

    private IEnumerator SpawnSpheres()
    {
        // todo: make this not just true
        while (true)
        {
            // spawn one at one of the transforms (random now, make them not as random later)
            int index = Random.Range(0, spawnPoints.Length);
            // spawn sphere
            GameObject sphere = Instantiate(dangerSphere, spawnPoints[index].position, Quaternion.identity);
            StartCoroutine(DespawnSphere(sphere));
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    private IEnumerator DespawnSphere(GameObject sphere)
    {
        yield return new WaitForSeconds(despawnTimer);
        Destroy(sphere);
    }
}
