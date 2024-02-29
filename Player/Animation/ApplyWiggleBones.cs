using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Obi;
using UnityEngine;
using UnityEngine.Serialization;

public class ApplyWiggleBones : MonoBehaviour
{
    [FormerlySerializedAs("tailBone")] public string tailBoneName;
    [FormerlySerializedAs("leftEarBone")] public string leftEarBoneName;
    [FormerlySerializedAs("rightEarBone")] public string rightEarBoneName;

    public float earMass = 20;
    public float tailMass = 10;
    public float earRotationalMass = 0.5f;
    
    // Start is called before the first frame update
    void Awake()
    {
        GameObject tail = transform.MMFindDeepChildDepthFirst(tailBoneName).gameObject;
        GameObject leftEar = transform.MMFindDeepChildDepthFirst(leftEarBoneName).gameObject;
        GameObject rightEar = transform.MMFindDeepChildDepthFirst(rightEarBoneName).gameObject;

        ObiBone tailBone = tail.AddComponent<ObiBone>();
        ObiBone leftEarBone = leftEar.AddComponent<ObiBone>();
        ObiBone rightEarBone = rightEar.AddComponent<ObiBone>();
        
        tailBone.stretchBones = false;
        leftEarBone.stretchBones = false;
        rightEarBone.stretchBones = false;
        
        tailBone.mass.multiplier = tailMass;
        leftEarBone.mass.multiplier = earMass;
        rightEarBone.mass.multiplier = earMass;
        leftEarBone.rotationalMass.multiplier = earRotationalMass;
        rightEarBone.rotationalMass.multiplier = earRotationalMass;
        
        tailBone.UpdateMasses();
        leftEarBone.UpdateMasses();
        rightEarBone.UpdateMasses();
        
    }

}
