using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace __OasisBlitz.__Scripts.Collectables
{
    public class CollectableManager : MonoBehaviour
    {

        public static CollectableManager Instance;
        public Action OnPlantCollected;

        private GameObject m_player;

        [Serializable]
        private class PlantCollectables
        {
            public int collectableIndex;
            public GameObject plantPrefab;
            public bool isSaved;
            public bool isPlaced;
        }

        [Serializable]
        private class DecorCollectables
        {
            public int collectableIndex;
            public GameObject collectablePrefab;
            public bool isSaved;
            public bool isPlaced;
        }

        [SerializeField] private List<PlantCollectables> _plantCollectablesList;
        [SerializeField] private List<DecorCollectables> _decorCollectablesList;
        [SerializeField] private float collectablePickupRadius = 3.0f;

        private bool isFirstLoad;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                isFirstLoad = true;
            }

            m_player = GameObject.FindGameObjectWithTag("Player");
            SceneManager.sceneLoaded += OnSceneload;
        }

        private void OnSceneload(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Contains("Burrow") && isFirstLoad)
            {
                XMLFileManager.Instance.Load();
                XMLFileManager.Instance.LoadAllData();
                isFirstLoad = false;
            }
        }

        public void ChangeCollectableStatus(CollectableObject targetCollectableObject, bool collected, bool placed)
        {
            Debug.Log(targetCollectableObject + " changed collectable status. Collected: " + collected + " Placed: " + placed);

            if (targetCollectableObject.CollectableType == CollectableType.PLANT)
            {
                bool found = false;
                foreach (var plant in _plantCollectablesList)
                {
                    if (plant.collectableIndex == targetCollectableObject.colletctableIndex)
                    {
                        plant.isSaved = collected;
                        plant.isPlaced = placed;
                        found = true;
                        break;
                    }
                }

                // If the plant is collected save the new thing
                if (collected && found)
                {
                    XMLFileManager.Instance.SaveLevelStatus(SceneManager.GetActiveScene().name);
                    XMLFileManager.Instance.SavePlantStatus(targetCollectableObject.colletctableIndex, targetCollectableObject.name, placed);
                }
            }
            else if (targetCollectableObject.CollectableType == CollectableType.DECOR)
            {
                bool found = false;
                foreach (var decor in _decorCollectablesList)
                {
                    if (decor.collectableIndex == targetCollectableObject.colletctableIndex)
                    {
                        decor.isSaved = collected;
                        decor.isPlaced = placed;
                        found = true;
                        break;
                    }
                }

                // If the plant is collected save the new thing
                if (collected && found)
                {
                    Debug.Log("Saving decor status");
                    XMLFileManager.Instance.SaveDecorStatus(targetCollectableObject.colletctableIndex, targetCollectableObject.name, placed);
                }
            }
            
        }

        public void LoadPlantCollectableStatus(int collectableIndex, bool collected, bool placed)
        {
            foreach (var plant in _plantCollectablesList)
            {
                if (plant.collectableIndex == collectableIndex)
                {
                    plant.isSaved = collected;
                    plant.isPlaced = placed;
                }
            }
        }
        
        public void LoadDecorCollectableStatus(int collectableIndex, bool collected, bool placed)
        {
            foreach (var decor in _decorCollectablesList)
            {
                if (decor.collectableIndex == collectableIndex)
                {
                    decor.isSaved = collected;
                    decor.isPlaced = placed;
                }
            }
        }

        public void ClearAllCollectableStatus()
        {
            foreach (var plant in _plantCollectablesList)
            {
                plant.isSaved = false;
                plant.isPlaced = false;
            }
            
            foreach (var decor in _decorCollectablesList)
            {
                decor.isSaved = false;
                decor.isPlaced = false;
            }
        }

        
        /// <summary>
        /// Look up the placement status of a plant
        /// </summary>
        /// <param name="index"> The index of the plant </param>
        /// <returns></returns>
        public bool LookupPlantPlacement(int index)
        {
            foreach (var plantCollectables in _plantCollectablesList)
            {
                if (plantCollectables.collectableIndex == index)
                {
                    return plantCollectables.isPlaced;
                }
            }
            Debug.LogError("Plant index doesn't exist");
            return false;
        }

        public bool LookupDecorPlacement(int index)
        {
            foreach (var decor in _decorCollectablesList)
            {
                if (decor.collectableIndex == index)
                {
                    return decor.isPlaced;
                }
            }
            Debug.LogError("Decor index doesn't exist");
            return false;
        }
        
        public void RequestForCollectableAppearance(CollectableType type, int index, Vector3 location, Quaternion rotation)
        {
            // FOR PLANTS
            if (type == CollectableType.PLANT)
            {
                foreach (var plantCollectables in _plantCollectablesList)
                {
                    if (plantCollectables.collectableIndex == index && plantCollectables.isSaved && (SceneManager.GetActiveScene().name.Contains("Burrow")))
                    {
                        GameObject objInstance = Instantiate(plantCollectables.plantPrefab, location, rotation);
                        objInstance.name = plantCollectables.plantPrefab.name;
                        objInstance.GetComponent<Plant>().isCollectable = true;
                        objInstance.GetComponent<Plant>().isPlaced = plantCollectables.isPlaced;
                    }
                }
            }
            else if (type == CollectableType.DECOR)
            {
                // For decors
                foreach (var decorCollectables in _decorCollectablesList)
                {
                    if (decorCollectables.collectableIndex == index && decorCollectables.isSaved && (SceneManager.GetActiveScene().name.Contains("Burrow")))
                    {
                        GameObject objInstance = Instantiate(decorCollectables.collectablePrefab, location, rotation);
                        objInstance.name = decorCollectables.collectablePrefab.name;
                        objInstance.GetComponent<Decor>().isCollectable = true;
                        objInstance.GetComponent<Decor>().isPlaced = decorCollectables.isPlaced;
                    }
                }
            }
        }

        /*public void RequestPickupCollectable()
        {
            // TODO make an angle check later as well for better interactions
            
            Collider[] cols = Physics.OverlapSphere(m_player.transform.position, collectablePickupRadius);
            foreach (var col in cols)
            {
                if (col.CompareTag("Collectable"))
                {
                    col.gameObject.GetComponent<CollectableObject>().StartInteractSequence();
                }
            }
        }*/

        public bool CheckIsSaved(int index)
        {
            foreach (var collectable in _plantCollectablesList)
            {
                if (collectable.collectableIndex == index && collectable.isSaved)
                {
                    return true;
                }
            }
            return false;
        }

        
    }
}