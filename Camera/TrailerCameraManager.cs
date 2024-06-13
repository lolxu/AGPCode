using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Feedbacks;
using Unity.Cinemachine;
using UnityEngine;

public class TrailerCameraManager : MonoBehaviour
{
    public CinemachineCamera leftTrailerCam;
    public CinemachineCamera rightTrailerCam;
    public CinemachineCamera frontTrailerCam;

    public CinemachineCamera sceneCam1;
    public CinemachineCamera sceneCam2;
    public CinemachineCamera sceneCam3;
    public CinemachineCamera sceneCam4;

    public float offsetPerFrame = 0.01f;

    private bool UIVisible = true;
    private bool SlowMotion = false;

    private CinemachineCamera[] cameras;
    
    private bool gameplayCamOn = true;

    private void Awake()
    {
        cameras = new CinemachineCamera[7];
        
        cameras[0] = leftTrailerCam;
        cameras[1] = rightTrailerCam;
        cameras[2] = frontTrailerCam;
        cameras[3] = sceneCam1;
        cameras[4] = sceneCam2;
        cameras[5] = sceneCam3;
        cameras[6] = sceneCam4;
        
        SetToGameplay();
        
    }
    
    private enum CameraState
    {
        LeftTrailerCam,
        RightTrailerCam,
        FrontTrailerCam,
        Scene1,
        Scene2,
        Scene3,
        Scene4
    }

    private CameraState currentCameraState;
    
    private void SetToCamera(CameraState camera)
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == (int) camera)
            {
                try
                {
                    cameras[i].Priority.Value = 1000;
                }
                catch (System.Exception e)
                {
                    // This is dev code, sometimes there won't be cameras and it's no biggie, don't ship with the trailer
                    // cam prefab enabled
                }
            }
            else
            {
                try
                {
                    cameras[i].Priority.Value = -1;
                }
                catch (System.Exception e)
                {
                    // This is dev code, sometimes there won't be cameras and it's no biggie, don't ship with the trailer
                    // cam prefab enabled
                }
            }
        }
    }

    private void SetToGameplay()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            try
            {
                cameras[i].Priority.Value = -1;
            }
            catch (System.Exception e)
            {
                // This is dev code, sometimes there won't be cameras and it's no biggie, don't ship with the trailer
                // cam prefab enabled
            }
        }
        
    }

    private void AdjustCurrentCameraOffset(Vector3 offset)
    {
        Debug.Log("Adjust offset");
        switch(currentCameraState)
        {
            case CameraState.LeftTrailerCam:
                leftTrailerCam.GetComponent<CinemachineFollow>().FollowOffset += offset;
                break;
            case CameraState.RightTrailerCam:
                rightTrailerCam.GetComponent<CinemachineFollow>().FollowOffset += offset;
                break;
            case CameraState.FrontTrailerCam:
                frontTrailerCam.GetComponent<CinemachineFollow>().FollowOffset += offset;
                break;
        }
    }

    private void ToggleUIVisiblity()
    {
        UIVisible = !UIVisible;
        
        if (!UIVisible)
        {
            // Find all gameobjects on layer "UI"
            FindObjectsOfType<GameObject>().Where(go => go.layer == LayerMask.NameToLayer("UI")).ToList()
                .ForEach(go => go.layer = LayerMask.NameToLayer("Cull"));
        }
        else
        {
            FindObjectsOfType<GameObject>().Where(go => go.layer == LayerMask.NameToLayer("Cull")).ToList()
                .ForEach(go => go.layer = LayerMask.NameToLayer("UI"));
            
        }
        
    }

    private void ToggleSlowMotion()
    {
        SlowMotion = !SlowMotion;
        
        if (SlowMotion)
        {
            MMTimeManager.Instance.NormalTimeScale = 0.3f;
        }
        else
        {
            MMTimeManager.Instance.NormalTimeScale = 1.0f;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            ToggleUIVisiblity();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleSlowMotion();  
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentCameraState = CameraState.LeftTrailerCam;
            gameplayCamOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentCameraState = CameraState.RightTrailerCam;
            gameplayCamOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentCameraState = CameraState.FrontTrailerCam;
            gameplayCamOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetToGameplay();
            gameplayCamOn = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentCameraState = CameraState.Scene1;
            gameplayCamOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            currentCameraState = CameraState.Scene2;
            gameplayCamOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            currentCameraState = CameraState.Scene3;
            gameplayCamOn = false;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            currentCameraState = CameraState.Scene4;
            gameplayCamOn = false;
        }

        if (!gameplayCamOn)
        {
            SetToCamera(currentCameraState);
        }

        if (Input.GetKey(KeyCode.Z) && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            AdjustCurrentCameraOffset(new Vector3(0, 0, offsetPerFrame));
        }
        else if (Input.GetKey(KeyCode.Z) && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            AdjustCurrentCameraOffset(new Vector3(0, 0, -offsetPerFrame));
        }
        
        if (Input.GetKey(KeyCode.Y) && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            AdjustCurrentCameraOffset(new Vector3(0, offsetPerFrame, 0));
        }
        else if (Input.GetKey(KeyCode.Y) && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            AdjustCurrentCameraOffset(new Vector3(0, -offsetPerFrame, 0));
        }
        
        if (Input.GetKey(KeyCode.X) && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            AdjustCurrentCameraOffset(new Vector3(offsetPerFrame, 0, 0));
        }
        else if (Input.GetKey(KeyCode.X) && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            AdjustCurrentCameraOffset(new Vector3(-offsetPerFrame, 0, 0));
        }
        
    }
}
