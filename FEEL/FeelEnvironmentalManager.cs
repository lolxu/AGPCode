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
        
    }
}

