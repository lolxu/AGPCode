#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

namespace __OasisBlitz.LevelLoading
{
    [ExecuteAlways]
    public class LevelLoaderForScene : MonoBehaviour
    {
        [SerializeField] public string _levelToLoadInEditor = "";
        [SerializeField] private bool _Load;
        [SerializeField] private bool _Save;
        [SerializeField] public int _DropDownIndex;
        private void Start()
        {
            m_LevelLoaderUIManager = GameObject.FindGameObjectWithTag("LevelLoaderUI").GetComponent<LevelLoaderUIManager>();
            m_LevelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                m_LevelLoader.EditorLevelStorage(_levelToLoadInEditor, _Load, false);
            }
            else
            {
                m_LevelLoaderUIManager.CurrFileSelected = _levelToLoadInEditor;
            }
            _Load = false;
            _Save = false;
        }

        //add a button for each function below and have it called by 
        [SerializeField] private LevelLoaderUIManager m_LevelLoaderUIManager;
        [SerializeField] private LevelLoader m_LevelLoader;
        private Button LoadIntoSceneButton;
        private Button SaveAndLoadIntoSceneButton;
    
        public void LoadOutOfPlayMode()
        {
            _levelToLoadInEditor = m_LevelLoaderUIManager.CurrFileSelected;
            _Load = true;
            _Save = false;
            //stop play mode
            UnityEditor.EditorApplication.isPlaying = false;
        }

        public void SaveAndLoadOutOfPlayMode()
        {
            //save
            _levelToLoadInEditor = m_LevelLoaderUIManager.CurrFileSelected;
            _Load = true;
            _Save = true;
            m_LevelLoader.EditorLevelStorage(_levelToLoadInEditor, false, _Save);
            //stop play mode
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
#endif
