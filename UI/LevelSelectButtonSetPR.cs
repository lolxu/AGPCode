using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectButtonSetPR : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PersonalBest;

    public void SetPBText(string sceneName)
    {
        float PB = XMLFileManager.Instance.LookupPBTime(sceneName);
        if(PB == -1)
        {
            PersonalBest.text = "00:00:000";
        }
        else
        {
            int min = (int)PB / 60;
            int sec = (int)PB - 60 * min;
            int ms = (int)(1000 * (PB - min * 60 - sec));
            PersonalBest.text = string.Format("{0:00}:{1:00}:{2:000}", min, sec, ms);
            //PersonalBest.text = PB.ToString();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetPBText(this.gameObject.name);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
