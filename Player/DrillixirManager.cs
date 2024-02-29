using System;
using __OasisBlitz.Player.StateMachine;
using __OasisBlitz.UI;
using DG.Tweening;
using UnityEngine;

namespace __OasisBlitz.Player
{

    enum DrillRefillType
    {
        Instant,
        Linear,
        Quadratic
    }
    public class DrillixirManager : MonoBehaviour
    {
        public bool UsingIntegerDrillixir = true;
        
        private float _maxDrillixir = 0.0f;
        [SerializeField] private DrillixirIndicator _indicator;
        //DO NOT MAKE 0
        [Range(0.5f, 10.0f)] [SerializeField] private float _drillixirRefillTime;
        [SerializeField] private float _drillixirConsumptionTime;
        [Range(0.0f, 1.0f)] [SerializeField] private float minimumDrillixirToStartDrillingAsPercent = 0.0f;
        [Range(0.0f, 1.0f)] [SerializeField] private float minimumDrillixirToBlastAsPercent = 0.0f;
        [Range(0.0f, 1.0f)] [SerializeField] private float minimumDrillixirToDashAsPercent = 0.0f;
        private float _currentDrillixir;
        [SerializeField] private DrillRefillType refillType;
        private float currTime = 0.0f;
        
        
        // optional integer drillixir changes
        private int DrillixirCharges;
        [SerializeField] private int MaxDrillixirCharges = 1;

        [SerializeField] private GameObject LGauntletTrail, RGauntletTrail, DrillTrail;
        [SerializeField] private GameObject AuraDrill, AuraLeftGauntlet, AuraLeftDrill, AuraRightGauntlet, AuraRightDrill, AuraFox, AuraClothing;
        [SerializeField] private PlayerAudio pAudio;    // Blast ready sound effect when refilling drillixir charge
        
        [SerializeField] private GameObject RefreshParticle;
        private PlayerStateMachine ctx;

        [Header("Blast Indicator Options")]
        [SerializeField] private bool BanditGlow;
        [SerializeField] private bool ClothesGlow;
        [SerializeField] private bool DrillGlow;
        [SerializeField] private bool GauntletsGlow;
        [SerializeField] private bool LRDrillGlow;
        [SerializeField] private bool TrailsAsIndicator;
        public float CurrentDrillixir
        {
            get { return _currentDrillixir; }
            set { _currentDrillixir = value; }
        }
        
        // Start is called before the first frame update
        void Awake()
        {
            
            ctx = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStateMachine>();
            //If drillixir refill timer is low, make it high so exponential fill up works properly
            if (_drillixirRefillTime < 0.5f)
            {
                _drillixirRefillTime = 1.0f;
            }
            //_maxDrillixir is a function of the time it takes to refill - this ensures that the refill time is accurate for our exponential time
            _maxDrillixir = _drillixirRefillTime * _drillixirRefillTime * _drillixirRefillTime;
            _currentDrillixir = _maxDrillixir;
            FullRefillDrillixir();
            if(UsingIntegerDrillixir)
            {
                EnableAura(DrillixirCharges > 0 ? true : false);
                if(TrailsAsIndicator) { EnableTrails(DrillixirCharges > 0 ? true : false); }
                else { EnableTrails(true); }
            }
        }

        public bool CanStartDrilling()
        {
            // TODO: As a temp way to test out the reversed drillixir behavior, I'm just going to return true here
            // return _currentDrillixir >= _maxDrillixir * minimumDrillixirToStartDrillingAsPercent;
            return true;
        }

        public bool CanDash()
        {
            // TODO: Setting this to always be true for now, because it ended up feeling frustrating to not be able to dash
            // bool canDash = _currentDrillixir >= _maxDrillixir * minimumDrillixirToDashAsPercent;
            bool canDash = true;
            if(!canDash)
            {
                _indicator.PlayDrillixirLowLimited();
            }
            return canDash;
        }

        public void ConsumeDashDrillixir()
        {
            // TODO: In tandem with always returning true above, we temporarily switch this to not consume any drillixir when dashing.
            // _indicator.PlayDrillixirConsumed();
            // ConsumeDrillixir(minimumDrillixirToDashAsPercent);
        }
        
        public bool CanBlast()
        {
            bool canBlast;
            if (UsingIntegerDrillixir)
            {
                canBlast = DrillixirCharges > 0;
            }
            else
            {
                canBlast = _currentDrillixir >= _maxDrillixir * minimumDrillixirToBlastAsPercent;
            }
            
            if(!canBlast)
            {
                _indicator.PlayDrillixirLowLimited();
            }
            return canBlast;
        }
        
        public void ConsumeBlastDrillixir()
        {
            _indicator.PlayDrillixirConsumed();
            if (UsingIntegerDrillixir)
            {
                ConsumeDrillixirCharges(1);
            }
            else
            {
                ConsumeDrillixir(minimumDrillixirToBlastAsPercent);
            }
            
            RefreshIndicator();
        }

        public void ConsumeDrillixirCharges(int Charges)
        {
            DrillixirCharges = Math.Max(0, DrillixirCharges - 1);
            if(DrillixirCharges == 0) { 
                EnableAura(false);
                if(TrailsAsIndicator) { EnableTrails(false); }
                HUDManager.Instance.SetBlastNotAvailable();
            }
        }
        
        public void ConsumeDrillixir(float percent)
        {
            _currentDrillixir -= percent * _maxDrillixir;
            
            if (_currentDrillixir < 0)
            {
                _currentDrillixir = 0;
            }
            currTime = 0.0f;
        }

        public void TickDrillixirConsume(float deltaTime)
        {
            _currentDrillixir -= (deltaTime / _drillixirConsumptionTime) * _maxDrillixir;
            if (_currentDrillixir < 0)
            {
                _currentDrillixir = 0;
            }
            currTime = 0.0f;
            RefreshIndicator();
        }

        void SpawnRefreshParticles()
        {
            // todo: re-enable this
            return;
            
            GameObject particles = Instantiate(RefreshParticle, ctx.gameObject.transform);
            particles.transform.localScale = ctx.transform.localScale;
            particles.transform.position = ctx.transform.position;
            ParticleSystem rechargeParticles = particles.GetComponent<ParticleSystem>();
            rechargeParticles.Play();
            
            DOVirtual.DelayedCall(.3f, () =>
            {
                ParticleSystem.MinMaxCurve radialCurve = new ParticleSystem.MinMaxCurve();
                radialCurve = -70.0f;
                ParticleSystem.VelocityOverLifetimeModule module = rechargeParticles.velocityOverLifetime;
                module.radial = radialCurve;
            }, false);
            DOVirtual.DelayedCall(3.0f, () => Destroy(particles), false);
        }

        public void AddDrillixirCharge()
        {
            if (DrillixirCharges == 0)
            {
                pAudio.PlayBlastReady();
                
                SpawnRefreshParticles();
            }
            if (UsingIntegerDrillixir)
            {
                DrillixirCharges = Math.Min(MaxDrillixirCharges, DrillixirCharges + 1);
                RefreshIndicator();
            }
            EnableAura(true);
            if(TrailsAsIndicator) { EnableTrails(true); }
            HUDManager.Instance.SetBlastAvailable();
        }

        public void RefillDrillixir(float percent)
        {
            if (!UsingIntegerDrillixir)
            {
                _currentDrillixir += percent * _maxDrillixir;
        
                if (_currentDrillixir > _maxDrillixir)
                {
                    _currentDrillixir = _maxDrillixir;
                }

                RefreshIndicator();
            }
        }
        
        public void FullRefillDrillixir()
        {
            if (UsingIntegerDrillixir)
            {
                if (DrillixirCharges == 0)
                {
                    SpawnRefreshParticles();
                }
                DrillixirCharges = MaxDrillixirCharges;
            }
            else
            {
                _currentDrillixir = _maxDrillixir;
            }
            
            RefreshIndicator();
        }

        public void TimeBasedRefill(float deltaTime)
        {
            if(!_indicator.recovering) { _indicator.recovering = true; }
            if (refillType == DrillRefillType.Instant)
            {
                _currentDrillixir = _maxDrillixir;
            }else if (refillType == DrillRefillType.Linear)
            {
                _currentDrillixir += (deltaTime / _drillixirRefillTime) * _maxDrillixir;
                if (_currentDrillixir > _maxDrillixir)
                {
                    _currentDrillixir = _maxDrillixir;
                }
            }
            else //refillType == DrillRefillType.Exponential
            {
                float prevTimeCubed = currTime * currTime * currTime;
                currTime += deltaTime;
                float currTimeCubed = currTime * currTime * currTime;
                //x^3 is the integration factor will give us 1 over 1 second
                _currentDrillixir += currTimeCubed - prevTimeCubed;
                if (_currentDrillixir > _maxDrillixir)
                {
                    _currentDrillixir = _maxDrillixir;
                }
            }
            
            RefreshIndicator();
        }

        private void RefreshIndicator()
        {
            if (UsingIntegerDrillixir)
            {
                _indicator.SetPercentFull((float) DrillixirCharges / MaxDrillixirCharges);
            }
            else
            {
                _indicator.SetPercentFull(_currentDrillixir / _maxDrillixir);
            }

            if (BounceAbility.Instance &&
                !BounceAbility.Instance.BounceEnabled)
            {
                return;
            }
            
            _indicator.InstantShowDrillixirHUD();
            _indicator.StartReleaseTimer();
        }

        private void EnableTrails(bool status)
        {
            LGauntletTrail.SetActive(status);
            RGauntletTrail.SetActive(status);
            DrillTrail.SetActive(status);
        }
        private void EnableAura(bool status)
        {
            if (!CanBlast())
            {
                status = false;
            }
            
            
            
            
            if(BanditGlow) { AuraFox.SetActive(status); }
            if(ClothesGlow) { AuraClothing.SetActive(status); }
            if(DrillGlow) { AuraDrill.SetActive(status); }
            if(GauntletsGlow) { 
                AuraLeftGauntlet.SetActive(status);
                AuraRightGauntlet.SetActive(status);
            }
            if(LRDrillGlow)
            {
                AuraLeftDrill.SetActive(status);
                AuraRightDrill.SetActive(status);
            }
        }
    }
}