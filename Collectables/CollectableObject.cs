using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace __OasisBlitz.__Scripts.Collectables
{
    public enum CollectableType
    {
        PLANT,
        DECOR,
        CRITTER
    };
    
    public class CollectableObject : MonoBehaviour
    {
        public bool isCollectable = true;
        public int colletctableIndex;
        public CollectableType CollectableType;
        private bool canInteractAgain = true;

        private IEnumerator Start()
        {
            yield return null;
            yield return null;
            // if (CollectableManager.Instance.CheckIsSaved(colletctableIndex) && SceneManager.GetActiveScene().name != "Burrow")
            // {
            //     // gameObject.SetActive(false);
            // }
            // else
            // {
            //     OnStart();
            // }
            
            OnStart();
        }

        /// <summary>
        /// Override to add start behaviors
        /// </summary>
        protected virtual void OnStart() {}

        /// <summary>
        /// Override to add interaction behaviors
        /// </summary>
        protected virtual void OnInteract() {}
        
        /// <summary>
        /// Override to add collect behaviors
        /// </summary>
        protected virtual void OnCollected() {}
        
        /// <summary>
        /// For COLLECTABLES ONLY
        /// </summary>
        protected virtual void OnPlaced() {}

        // TODO this is shit code I swear I will change this
        public void StartInteractSequence()
        {
            if (canInteractAgain)
            {
                canInteractAgain = false;
                // OnInteract();
                
                /*
                transform.DOScale(transform.localScale * 1.5f, 0.2f).SetEase(Ease.InOutExpo).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                {
                    canInteractAgain = true;
                    if (isCollectable && SceneManager.GetActiveScene().name.Contains("Burrow"))
                    {
                        OnPlaced();
                    }
                });
                */
                OnPlaced();
            }
        }

        public void PlaceInBurrow()
        {
            OnPlaced();
        }

        public void CollectSequence()
        {
            if (isCollectable && (!SceneManager.GetActiveScene().name.Contains("Burrow")))
            {
                // Debug.LogError("Done Collecting");
                transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    // Debug.LogError("Done Collecting");
                    CollectableManager.Instance.ChangeCollectableStatus(this, true, false);
                    OnCollected();
                    gameObject.SetActive(false);
                });
            }
        }
    }
}