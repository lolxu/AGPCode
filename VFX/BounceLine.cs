using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceLine : MonoBehaviour
{
    [SerializeField] private float LifeTime = 0.5f;

    [SerializeField] private LineRenderer lineRenderer;

    public void Init(Vector3 startPoint, Vector3 endPoint)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        StartCoroutine(DestroyAfterLifetime()); 

    }
    
    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(LifeTime);
        Destroy(gameObject);
    }

}
