using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.FEEL
{
    public class FeelEnvironmentalManager : MonoBehaviour
    {
        public static FeelEnvironmentalManager Instance;

        public MMF_Player plantCollectFeedback;
        public MMF_Player checkpointFeedback;
        public MMF_Player winObjectFeedback;
        public MMF_Player burrowCannonFeedback;
        public MMF_Player sandBurstFeedback;
        public MMF_Player waterBurstFeedback;
        public MMF_Player waterSplashFeedback;
        public MMF_Player waterRippleFeedback;
        public MMF_Player enemyDeathFeedback;
        public MMF_Player landFeedback;
        public MMF_Player walkFeedback;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void PlayPlantCollectFeedback(Vector3 position, float feedbackIntensity)
        {
            plantCollectFeedback.PlayFeedbacks(position, feedbackIntensity);
        }

        public void PlayBurrowCannonFeedback(Vector3 position, float feedbackIntensity, int index)
        {
            if (index != -1)
            {
                burrowCannonFeedback.FeedbacksList[index].Play(position, feedbackIntensity);
            }
        }

        public void PlaySandBurstFeedback(Vector3 position, float feedbackIntensity)
        {
            sandBurstFeedback.PlayFeedbacks(position, feedbackIntensity);
        }

        public void PlayWaterBurstFeedback(Vector3 position, float feedbackIntensity)
        {
            waterBurstFeedback.PlayFeedbacks(position, feedbackIntensity);
        }
        
        public void PlayWaterSplashFeedback(Vector3 position, float feedbackIntensity)
        {
            waterSplashFeedback.PlayFeedbacks(position, feedbackIntensity);
        }

        public void PlayWaterRippleFeedback(Vector3 position, float feedbackIntensity)
        {
            waterRippleFeedback.PlayFeedbacks(position, feedbackIntensity);
        }

        [SerializeField] private Transform leftFootIdleRipple;
        [SerializeField] private Transform rightFootIdleRipple;
        public void PlayWaterIdleRippleFeedback(Vector3 leftFoot, Vector3 rightFoot)
        {
            if (leftFootIdleRipple.gameObject.activeSelf)
            {
                return;
            }
            leftFootIdleRipple.position = leftFoot;
            rightFootIdleRipple.position = rightFoot;
            leftFootIdleRipple.gameObject.SetActive(true);
            rightFootIdleRipple.gameObject.SetActive(true);
        }

        public void StopWaterIdleRippleFeedback()
        {
            leftFootIdleRipple.gameObject.SetActive(false);
            rightFootIdleRipple.gameObject.SetActive(false);
        }

        public void PlayEnemyDeathFeedback(Vector3 position, float feedbackIntensity)
        {
            enemyDeathFeedback.PlayFeedbacks(position, feedbackIntensity);
        }

        public void PlayLandFeedback(Vector3 position, float feedbackIntensity)
        {
            landFeedback.PlayFeedbacks(position, feedbackIntensity);
        }

        public void PlayWalkFeedback(Vector3 position, float feedbackIntensity)
        {
            walkFeedback.PlayFeedbacks(position, feedbackIntensity);
        }
        
    }
}

