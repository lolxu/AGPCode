using __OasisBlitz.__Scripts.FEEL;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.Player.Environment.WinCondition
{
    public class WinObject : MonoBehaviour
    {
        [Header("Win Object Settings")] 
        [SerializeField] private float animationTime;
        /*
        [SerializeField] private string objectiveText;
        */
        private bool hasPlayed = false;

        public void ActivateObject()
        {
            StopTimer();
            transform.DOScale(Vector3.zero, animationTime)
                .SetEase(Ease.InOutSine);
            // Play some particles maybe?
            if (!hasPlayed)
            {
                FeelEnvironmentalManager.Instance.winObjectFeedback.PlayFeedbacks(transform.position);
                hasPlayed = true;
                // UIManager.Instance.Win(objectiveText);
            }
            
        }
        /*public void OpenWinScreen()
        {
            UIManager.Instance.Win(objectiveText);
        }*/

        public void StopTimer()
        {
            // UIManager.Instance.StartStopTime();
        }
    }
}