using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.Utility
{
    public class RuntimeMeshSetup : MonoBehaviour
    {
        [SerializeField] private SetupBackfaces setupBackfaces;
        // [SerializeField] private MeshUnion meshUnion;

        void Awake()
        {
            SceneManager.sceneLoaded += SetupOnSceneLoad;
        }
        
        private void SetupOnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            // Only perform this once, then unbind it
            SceneManager.sceneLoaded -= SetupOnSceneLoad;
            SetupMeshes();
        }

        public void SetupMeshes()
        {
            // First, create any unions that need to be created
            // meshUnion.FindAndConstructAllMeshUnions();
            
            // Now, setup the backfaces
            setupBackfaces.CreateBackfaceMeshes();
            
        }
    }
}