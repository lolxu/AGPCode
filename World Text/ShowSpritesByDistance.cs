using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShowSpritesByDistance : MonoBehaviour
{
    public float maxDistance = 8f;   // Set the max distance at which the GameObject will be fully visible
    public float animationTime = 0.5f;
    public GameObject fadeImageObjectParent;

    private SpriteRenderer[] sprites;
    private TMP_Text[] texts;
    private bool isDisplaying = false;
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        texts = GetComponentsInChildren<TMP_Text>();
        player = GameObject.FindGameObjectWithTag("Player");

        foreach (var sprite in sprites)
        {
            sprite.DOFade(0, 0.1f);
        }
        foreach (var text in texts)
        {
            text.DOFade(0, 0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisplaying) return;
        
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < maxDistance)
        {
            isDisplaying = true;
            foreach (var sprite in sprites)
            {
                sprite.DOFade(1f, animationTime);
            }
            foreach (var text in texts)
            {
                text.DOFade(1f, animationTime);
            }
        }

        // Debug.Log(distance);
    }
}
