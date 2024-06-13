using System;
using System.Collections;
using System.Globalization;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace __OasisBlitz.__Scripts.Player.Environment.Checkpoints
{
    public class BackToLevel : MonoBehaviour
    {
        [SerializeField] private TextMeshPro myHint;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject KeyboardHotkey;
        [SerializeField] private GameObject ControllerHotkeys;
        private string hotkeyType;

        private PlayerStateMachine ctx;
        private Transform myCameraTransform;
        private bool pressed = false;

        private void Start()
        {
            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
            myCameraTransform = GameObject.FindGameObjectWithTag("PlayerCamera").transform;
        }

        private void Update()
        {
            // myHint.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);

            canvas.transform.LookAt(2 * gameObject.transform.position - myCameraTransform.position);
            canvas.transform.rotation = Quaternion.Euler(0.0f, canvas.transform.rotation.eulerAngles.y, 0.0f);

            if (hotkeyType != GlobalSettings.Instance.displayedController)
                {
                    hotkeyType = GlobalSettings.Instance.displayedController;
                    switch (hotkeyType)
                    {
                        case "KEYBOARD":
                            KeyboardHotkey.SetActive(true);
                            ControllerHotkeys.SetActive(false);
                            break;
                        case "XBOX":
                        case "PLAYSTATION":
                        case "OTHER":
                            KeyboardHotkey.SetActive(false);
                            ControllerHotkeys.SetActive(true);
                            break;

                    }
                }
            }

        public void RequestLevelTransit()
        {
            // if (LevelManager.Instance.TeleportRequested)
            // {
            //     LevelManager.Instance.LoadAnySceneAsync(GameMetadataTracker.Instance.GetPreviousSceneName());
            // }
        }
    }
}