using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BurrowTimerText : MonoBehaviour
{
    [SerializeField] private string m_sceneName;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        yield return null;
        XMLFileManager.Instance.Load();
        float time = XMLFileManager.Instance.LookupPBTime(m_sceneName);

        // Debug.LogError(time);
        if (Math.Abs(time - (-1.0f)) > 0.001f)
        {
            int min = (int)time / 60;
            int sec = (int)time - 60 * min;
            int ms = (int)(1000 * (time - min * 60 - sec));
            GetComponent<TextMeshPro>().text = string.Format("{0:00}:{1:00}:{2:000}", min, sec, ms);
        }

    }

}
