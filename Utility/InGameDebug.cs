using TMPro;
using UnityEngine;

namespace __OasisBlitz.Utility
{
    public class InGameDebug : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        // Start is called before the first frame update
        void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void SetText(string text)
        {
            // _text.text = text;
        }
    }
}