using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class ChargedWeaponEffectsHandler : MonoBehaviour
    {
        [Header("Visual")] [Tooltip("Object that will be affected by charging scale & color changes")]
        public GameObject ChargingObject;

        [Tooltip("The spinning frame")] public GameObject SpinningFrame;

        [Tooltip("Scale of the charged object based on charge")]
        public MinMaxVector3 Scale;

        [Header("Particles")] [Tooltip("Particles to create when charging")]
        public GameObject DiskOrbitParticlePrefab;

        [Tooltip("Local position offset of the charge particles (relative to this transform)")]
        public Vector3 Offset;

        [Tooltip("Parent transform for the particles (Optional)")]
        public Transform ParentTransform;

        [Tooltip("Orbital velocity of the charge particles based on charge")]
        public MinMaxFloat OrbitY;

        [Tooltip("Radius of the charge particles based on charge")]
        public MinMaxVector3 Radius;

        [Tooltip("Idle spinning speed of the frame based on charge")]
        public MinMaxFloat SpinningSpeed;

        [Header("Sound")] [Tooltip("Audio clip for charge SFX")]
        public AudioClip ChargeSound;

        [Tooltip("Sound played in loop after the change is full for this weapon")]
        public AudioClip LoopChargeWeaponSfx;

        [Tooltip("Duration of the cross fade between the charge and the loop sound")]
        public float FadeLoopDuration = 0.5f;

        [Tooltip(
            "If true, the ChargeSound will be ignored and the pitch on the LoopSound will be procedural, based on the charge amount")]
        public bool UseProceduralPitchOnLoopSfx;

        [Range(1.0f, 5.0f), Tooltip("Maximum procedural Pitch value")]
        public float MaxProceduralPitchValue = 2.0f;

        public GameObject ParticleInstance { get; set; }

        ParticleSystem _diskOrbitParticle;
        WeaponController _weaponController;
        ParticleSystem.VelocityOverLifetimeModule _velocityOverTimeModule;

        AudioSource _audioSource;
        AudioSource _audioSourceLoop;

        float _lastChargeTriggerTimestamp;
        float _chargeRatio;
        float _endchargeTime;

        void Awake()
        {
            _lastChargeTriggerTimestamp = 0.0f;

            // The charge effect needs it's own AudioSources, since it will play on top of the other gun sounds
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = ChargeSound;
            _audioSource.playOnAwake = false;

            // create a second audio source, to play the sound with a delay
            _audioSourceLoop = gameObject.AddComponent<AudioSource>();
            _audioSourceLoop.clip = LoopChargeWeaponSfx;
            _audioSourceLoop.playOnAwake = false;
            _audioSourceLoop.loop = true;
        }

        void SpawnParticleSystem()
        {
            ParticleInstance = Instantiate(DiskOrbitParticlePrefab,
                ParentTransform != null ? ParentTransform : transform);
            ParticleInstance.transform.localPosition += Offset;

            FindReferences();
        }

        public void FindReferences()
        {
            _diskOrbitParticle = ParticleInstance.GetComponent<ParticleSystem>();
            DebugUtility.HandleErrorIfNullGetComponent<ParticleSystem, ChargedWeaponEffectsHandler>(_diskOrbitParticle,
                this, ParticleInstance.gameObject);

            _weaponController = GetComponent<WeaponController>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, ChargedWeaponEffectsHandler>(
                _weaponController, this, gameObject);

            _velocityOverTimeModule = _diskOrbitParticle.velocityOverLifetime;
        }

        void Update()
        {
            if (ParticleInstance == null)
                SpawnParticleSystem();

            _diskOrbitParticle.gameObject.SetActive(_weaponController.IsItemActive);
            _chargeRatio = _weaponController.CurrentCharge;

            ChargingObject.transform.localScale = Scale.GetValueFromRatio(_chargeRatio);
            if (SpinningFrame != null)
            {
                SpinningFrame.transform.localRotation *= Quaternion.Euler(0,
                    SpinningSpeed.GetValueFromRatio(_chargeRatio) * Time.deltaTime, 0);
            }

            _velocityOverTimeModule.orbitalY = OrbitY.GetValueFromRatio(_chargeRatio);
            _diskOrbitParticle.transform.localScale = Radius.GetValueFromRatio(_chargeRatio * 1.1f);

            // update sound's volume and pitch 
            if (_chargeRatio > 0)
            {
                if (!_audioSourceLoop.isPlaying &&
                    _weaponController.LastChargeTriggerTimestamp > _lastChargeTriggerTimestamp)
                {
                    _lastChargeTriggerTimestamp = _weaponController.LastChargeTriggerTimestamp;
                    if (!UseProceduralPitchOnLoopSfx)
                    {
                        _endchargeTime = Time.time + ChargeSound.length;
                        _audioSource.Play();
                    }

                    _audioSourceLoop.Play();
                }

                if (!UseProceduralPitchOnLoopSfx)
                {
                    float volumeRatio =
                        Mathf.Clamp01((_endchargeTime - Time.time - FadeLoopDuration) / FadeLoopDuration);
                    _audioSource.volume = volumeRatio;
                    _audioSourceLoop.volume = 1 - volumeRatio;
                }
                else
                {
                    _audioSourceLoop.pitch = Mathf.Lerp(1.0f, MaxProceduralPitchValue, _chargeRatio);
                }
            }
            else
            {
                _audioSource.Stop();
                _audioSourceLoop.Stop();
            }
        }
    }
}