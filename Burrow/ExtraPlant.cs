using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ExtraPlant : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(false);
    }
    
    public void PlaceAnimated()
    {
        gameObject.SetActive(true);
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x, 0, originalScale.z);
        // Get a random delay value between 0 and 0.3f
        float randomDelay = Random.Range(0, 0.3f);
        transform.DOScale(originalScale, 0.5f).SetEase(Ease.InOutBounce).SetDelay(randomDelay);
    }

    public void PlaceInstant()
    {
        gameObject.SetActive(true);   
    }
    
}
