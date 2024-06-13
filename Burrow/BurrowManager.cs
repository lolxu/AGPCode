using System;
using System.Collections;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.FEEL;
using __OasisBlitz.__Scripts.Player.Environment.Cannon;
using __OasisBlitz.Camera.StateMachine;
using __OasisBlitz.Player.StateMachine;
using Unity.Cinemachine;
using UnityEngine;

namespace __OasisBlitz.Player.Environment.Cannon
{
    public class BurrowManager : MonoBehaviour
    {
        public static BurrowManager Instance;
        
        [Header("Cannons In Level Sequence")]
        [SerializeField] private LevelCannonObjects _cannonObject;
        [SerializeField] private int totalLevels = 3;

        [Header("Burrow Cameras")] 
        public CinemachineCamera plantCamera;
        public CinemachineCamera cannonCamera;
        public CinemachineCamera burrowCamera;

        private PlayerStateMachine ctx;
        private PlayerInput playerInput;
        
        public StartBelowGround startBelowGround;
        public BurrowPlantManager burrowPlantManager;

        public bool playCutscene = false;

        public Slideshow openingCutscene;
        public Slideshow endingCutscene;
        public GameplayMusic burrowMusic;

        private Coroutine slideRoutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            
            Destroy(gameObject);
        }

        private IEnumerator Start()
        {
            yield return null;
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                ctx = player.GetComponent<PlayerStateMachine>();
                playerInput = player.GetComponent<PlayerInput>();
            }

            // Initializing cannons
            XMLFileManager.Instance.Load();

            if (playCutscene && XMLFileManager.Instance.ShouldPlayCutscene())
            {
                openingCutscene.gameObject.SetActive(true);
                XMLFileManager.Instance.SaveCutsceneViewed();
                DisableBanditModelAndInput();
                StartOpeningCutscene();
            }
            else if (playCutscene && XMLFileManager.Instance.ShouldPlayEndingCutscene())
            {
                endingCutscene.gameObject.SetActive(true);
                XMLFileManager.Instance.SaveEndingCutsceneViewed();
                DisableBanditModelAndInput();
                StartEndingCutscene();
            }
            else
            {
                StartBurrowMusicAndAmbience();
            }

        }

        private void DisableBanditModelAndInput()
        {
            ctx.ModelRotator.HideBandit();
            playerInput.SwitchCurrentInputState(PlayerInput.PlayerInputState.SlideShowControls);
        }
        
        private void EnableBanditModelAndInput()
        {
            ctx.ModelRotator.RevealBandit();
            playerInput.SwitchCurrentInputState(PlayerInput.PlayerInputState.Character);
        }

        private void StartBurrowMusicAndAmbience()
        {
            burrowMusic.StartMusic();  
            burrowMusic.StartAmbience();
        }
        
        private void StartOpeningCutscene()
        {
            slideRoutine = StartCoroutine(openingCutscene.StartSlideshow());
            openingCutscene.OnSlideshowEnd += OnOpeningCutsceneEnd;
            openingCutscene.OnSlideshowMusicEnd += OnOpeningCutsceneMusicEnd;
            openingCutscene.OnSlideshowSkip += SlideShowSkipped;
        }
        
        private void StartEndingCutscene()
        {
            slideRoutine = StartCoroutine(endingCutscene.StartSlideshow(true));
            endingCutscene.OnSlideshowEnd += OnOpeningCutsceneEnd;
            endingCutscene.OnSlideshowMusicEnd += OnOpeningCutsceneMusicEnd;
            endingCutscene.OnSlideshowSkip += SlideShowSkipped;
        }

        private void OnOpeningCutsceneEnd()
        {
            EnableBanditModelAndInput();
        }

        private void OnOpeningCutsceneMusicEnd()
        {
            StartBurrowMusicAndAmbience();
        }

        private void SlideShowSkipped()
        {
            StopCoroutine(slideRoutine);
            OnOpeningCutsceneEnd();
            OnOpeningCutsceneMusicEnd();
        }
        
        private void OnEnable()
        {
            startBelowGround.OnEmergeFromGround += PlacePlantOnEmerge;
        }
        
        private void OnDisable()
        {
            startBelowGround.OnEmergeFromGround -= PlacePlantOnEmerge;
        }

        private void PlacePlantOnEmerge()
        {
            burrowPlantManager.unplacedPlant?.PlaceInBurrow();
        }

        private void CannonParticleEffect(GameObject cannon, int index)
        {
            FeelEnvironmentalManager.Instance.PlayBurrowCannonFeedback(cannon.transform.position, 2.0f, index);
        }
    }
}