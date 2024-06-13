using System;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.FEEL;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using MeshRenderer = UnityEngine.MeshRenderer;

namespace __OasisBlitz.__Scripts.Collectables
{
    public class Decor : CollectableObject
    {
        public enum DecorType
        {
            BED
        }
        
        public bool isPlaced { get; set; } = false;
        [SerializeField] private DecorType _decorType;
        [SerializeField] private GameObject _decorObject;
        [SerializeField] private GameObject _decorGlowingBase;
        [SerializeField] private List<GameObject> _objectComponents;
        [SerializeField] private Material _unplacedMaterial;
        [SerializeField] private List<Material> _placedMaterial;
        [SerializeField] private Collider _collider;
        [SerializeField] private List<Collider> _colliders;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!SceneManager.GetActiveScene().name.Contains("Burrow") && !isPlaced)
                {
                    CollectSequence();
                }
                else if (SceneManager.GetActiveScene().name.Contains("Burrow") && !isPlaced)
                {
                    StartInteractSequence();
                }
            }
        }

        protected override void OnStart()
        {
            isPlaced = CollectableManager.Instance.LookupDecorPlacement(colletctableIndex);
            if (!SceneManager.GetActiveScene().name.Contains("Burrow"))
            {
                Vector3 endRot = _decorObject.transform.eulerAngles;
                _decorObject.transform.DORotate(endRot + new Vector3(0.0f, 360.0f, 30.0f), 1.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
                
                _decorGlowingBase.SetActive(true);
                Debug.Log(isPlaced);
                if (isPlaced)
                {
                    for(int i = 0; i < _objectComponents.Count; i++)
                    {
                        var _renderer = _objectComponents[i].GetComponent<MeshRenderer>();
                        _renderer.material = _unplacedMaterial;
                    }
                }
            }
            else
            {
                _decorGlowingBase.SetActive(false);
                if (!isPlaced)
                {
                    for(int i = 0; i < _objectComponents.Count; i++)
                    {
                        var _renderer = _objectComponents[i].GetComponent<MeshRenderer>();
                        _renderer.material = _unplacedMaterial;
                    }
                }
                else
                {
                    _collider.isTrigger = false;
                    for (int i = 0; i < _colliders.Count; i++)
                    {
                        _colliders[i].isTrigger = false;
                    }
                }
            }
        }

        protected override void OnCollected()
        {
            // TODO Something custom for collecting a decor - called in level
        }

        protected override void OnInteract()
        {
            Debug.Log("Interact with decor");
            switch (_decorType)
            {
                case DecorType.BED:
                    Debug.Log("You are sleeping lmao");
                    break;
            }
            // FeelEnvironmentalManager.Instance.PlayPlantCollectFeedback(transform.position, 1.5f);
        }

        protected override void OnPlaced()
        {
            if (!isPlaced)
            {
                Debug.Log("On Placed");
                // _renderer.material = placedMaterial;
                CollectableManager.Instance.ChangeCollectableStatus(this, true, true);

                for(int i = 0; i < _objectComponents.Count; i++)
                {
                    var _renderer = _objectComponents[i].GetComponent<MeshRenderer>();
                    _renderer.material = _placedMaterial[i];
                }
                
                isPlaced = true;
                _collider.isTrigger = false;
                for (int i = 0; i < _colliders.Count; i++)
                {
                    _colliders[i].isTrigger = false;
                }
            }
        }
    }
}