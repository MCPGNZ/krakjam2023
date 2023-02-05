namespace Krakjam
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Krakjam/Game Balance")]
    public class GameBalance : ScriptableObjectSingleton<GameBalance>
    {
        [FoldoutGroup("Player audio", true)]
        [Range(0.0f, 1.0f)]
        public float _MaxVolume;
        public static float MaxVolume => Instance._MaxVolume;

        public float _VolumeChangeRateStrength;
        public static float VolumeChangeRateStrength => Instance._VolumeChangeRateStrength;

        [FoldoutGroup("Player AudioSource", true)]
        public AudioClip _PlayerFootstepsSound;
        public static AudioClip PlayerFootsteps => Instance._PlayerFootstepsSound;

        public AudioClip _PlayerDeathSound;
        public static AudioClip PlayerDeathSound => Instance._PlayerDeathSound;

        public AudioClip _PlayerHitSound;
        public static AudioClip PlayerHitSound => Instance._PlayerHitSound;

        public AudioClip _PlayerJumpSound;
        public static AudioClip PlayerJumpSound => Instance._PlayerJumpSound;

        public AudioClip _PlayerDashSound;
        public static AudioClip PlayerDashSound => Instance._PlayerDashSound;

        public AudioClip _MonsterSound;
        public static AudioClip MonsterSound => Instance._MonsterSound;

        public AudioClip _PlayerPickupOrb;
        public static AudioClip PlayerPickupOrb => Instance._PlayerPickupOrb;

        [FoldoutGroup("Player", true)]
        public float _Life = 100.0f;
        public static float Life => Instance._Life;

        [FoldoutGroup("Player", true)]
        public float _MovementSpeed = 1000.0f;
        public static float MovementSpeed => Instance._MovementSpeed;

        [FoldoutGroup("Player", true)]
        public float _RotationSensitivity = 1.5f;
        public static float RotationSensitivity => Instance._RotationSensitivity;

        [FoldoutGroup("Player", true)]
        public float _JumpStrength = 10.0f;
        public static float JumpStrength => Instance._JumpStrength;

        [FoldoutGroup("Player", true)]
        public float _Friction = 1.0f;
        public static float Friction => Instance._Friction;

        [FoldoutGroup("Player", true)]
        public float _MaxFriction = 1.0f;
        public static float MaxFriction => Instance._MaxFriction;

        [FoldoutGroup("Player", true)]
        public float _AirSpeed = 0.25f;
        public static float AirSpeed => Instance._AirSpeed;

        [FoldoutGroup("Player", true)]
        public float _SpeedThreshold = 5.0f;
        public static float SpeedThreshold => Instance._SpeedThreshold;

        [FoldoutGroup("Player", true)]
        public float _DashStrength = 100.0f;
        public static float DashStrength => Instance._DashStrength;

        [FoldoutGroup("Player", true)]
        public float _RotationToGroundNormal = 5.0f;
        public static float RotationToGroundNormal => Instance._RotationToGroundNormal;

        public float _LifeTimeChangeSpeed = 3.0f;
        public static float LifeTimeChangeSpeed => Instance._LifeTimeChangeSpeed;

        [FoldoutGroup("Start Animation", true)]
        [Range(0.0f, 200.0f)]
        public float _AnimationSpeed;
        public static float AnimationSpeed => Instance._AnimationSpeed;

        [FoldoutGroup("Monster settings", true)]
        [Range(0.0f, 30.0f)]
        public float _MonsterSpeed;
        public static float MonsterSpeed => Instance._MonsterSpeed;

        [FoldoutGroup("Player death settings", true)]
        public float _DeathHeighOffset;
        public static float DeathHeighOffset => Instance._DeathHeighOffset;

        [FoldoutGroup("Resolution change")]
        public float _ResolutionChangeStrength;
        public static float ResolutionChangeStrength => Instance._ResolutionChangeStrength;
    }
}