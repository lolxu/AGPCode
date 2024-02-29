using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace __OasisBlitz.__Scripts.WorldText
{
    public class FadeWithDistance : MonoBehaviour
    {
        public float maxDistance = 8f;   // Set the max distance at which the GameObject will be fully visible

        private TMP_Text tmpText;
        private float originalAlpha;
        private GameObject player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
            
            tmpText = GetComponent<TMP_Text>();
            originalAlpha = tmpText.alpha;
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // Normalize distance value between 0 (player at object) and 1 (player at maxDistance or beyond)
            float normalizedDistance = Mathf.Clamp(distance / maxDistance, 0, originalAlpha);
            normalizedDistance = originalAlpha - normalizedDistance;
            // Adjust alpha based on normalized distance (the closer the player, the lower the alpha)
            //float alpha = Mathf.Lerp(0f, originalColor.a, normalizedDistance);

            tmpText.alpha = normalizedDistance;
        }
    }
}