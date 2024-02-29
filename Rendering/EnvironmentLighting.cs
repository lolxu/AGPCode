using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace __OasisBlitz.LevelLoading
{
    //Sets the lighting setting upon being placed in a scene - play and 
#if (UNITY_EDITOR)
    [ExecuteAlways]
    public class EnvironmentLighting : MonoBehaviour
    {
        [Header("General Lighting Settings")] 
        [SerializeField] private LightingSettings lightSettings;
        private LightingSettings lightSettingsH;
        
        [Header("Environment Lighting Settings")] 
        [SerializeField] private Material skyboxMaterial;
        private Material skyboxMaterialH;
        [SerializeField] private Light sunLight;
        private Light sunLightH;
        [SerializeField] private Color realtimeShadowColor;
        private Color realtimeShadowColorH;
        [Range(0.0f, 8.0f)]
        [SerializeField] private float intensityMultiplier;
        private float intensityMultiplierH;
        [SerializeField] private Color ambientColor;
        [SerializeField] private AmbientMode ambientMode;
        
        
        [Header("Environment Reflection Settings")] 
        [ValueDropdown("resolutionOptions")]
        [SerializeField] private int resolution;
        private int resolutionH;
        private int[] resolutionOptions = { 16, 32, 64, 128, 256, 512, 1024, 2048 };
        //[SerializeField] private ReflectionCubemapCompression compression; idk how assign this :(((((
        [Range(0.0f, 1.0f)]
        [SerializeField] private float reflectionIntensityMultiplier;
        private float reflectionIntensityMultiplierH;
        [Header("Fog Settings")] 
        [SerializeField] private bool fog;
        private bool fogH;
        [SerializeField] private Color fogColor;
        private Color fogColorH;
        [SerializeField] private FogMode fogMode;
        private FogMode fogModeH;
        [SerializeField] private float fogStartDistance;
        private float fogStartDistanceH;
        [SerializeField] private float fogEndDistance;
        private float fogEndDistanceH;
        
        [Header("Other")]
        [Range(0.0f, 1.0f)]
        [SerializeField] private float haloStrength;
        private float haloStrengthH;
        [SerializeField] private float flareFadeSpeed;
        private float flareFadeSpeedH;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float flareStrength;
        private float flareStrengthH;

        private void OnValidate()
        {
            if (lightSettings != lightSettingsH || skyboxMaterial != skyboxMaterialH || sunLight != skyboxMaterialH ||
                sunLight != sunLightH || realtimeShadowColor != realtimeShadowColorH
                || intensityMultiplier != intensityMultiplierH || resolution != resolutionH ||
                reflectionIntensityMultiplier != reflectionIntensityMultiplierH || fog != fogH
                || fogColor != fogColorH || fogMode != fogModeH || fogStartDistance != fogStartDistanceH ||
                fogEndDistance != fogEndDistanceH || haloStrength != haloStrengthH || flareFadeSpeed != flareFadeSpeedH
                || flareStrength != flareStrengthH)
            {
                ApplyLightSettings();
            }
        }

        private void ApplyLightSettings()
        {
            //GeneralLightingSettings
            Lightmapping.lightingSettings = lightSettings;
            //Environment Lighting Settings
            RenderSettings.skybox = skyboxMaterial;
            RenderSettings.sun = sunLight;
            RenderSettings.subtractiveShadowColor = realtimeShadowColor;
            RenderSettings.ambientIntensity = intensityMultiplier;
            RenderSettings.ambientMode = ambientMode;
            RenderSettings.ambientLight = ambientColor;
            //Environment Reflection Settings
            RenderSettings.defaultReflectionResolution = resolution;
            RenderSettings.reflectionIntensity = reflectionIntensityMultiplier;
            //Fog Settings
            RenderSettings.fog = fog;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogStartDistance = fogStartDistance;
            RenderSettings.fogEndDistance = fogEndDistance;
            //other settings
            RenderSettings.haloStrength = haloStrength;
            RenderSettings.flareFadeSpeed = flareFadeSpeed;
            RenderSettings.flareStrength = flareStrength;
        }
    }
    #endif
}
