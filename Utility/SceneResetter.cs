using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace __OasisBlitz.Utility
{
    public class SceneResetter : MonoBehaviour
    {
        [SerializeField]
        private InputActionReference ResetButton1;

        [SerializeField]
        private InputActionReference ResetButton2;

        public int SceneID;

        void Awake()
        {
            ResetButton1.action.Enable();
            ResetButton2.action.Enable();    
        }
    
        // Update is called once per frame
        void Update()
        {
            if (ResetButton1.action.inProgress && ResetButton2.action.inProgress)
            {
                Debug.Log("Reset!");
                SceneManager.LoadScene(SceneID);
                // Stop all running fmod events
            
            }

        }
    }
}
