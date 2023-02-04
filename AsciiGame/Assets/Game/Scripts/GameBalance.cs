namespace Krakjam
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Krakjam/Game Balance")]
    public class GameBalance : ScriptableObjectSingleton<GameBalance>
    {
        [FoldoutGroup("Player", true)]
        public float _Life = 100.0f;
        public static float Life => Instance._Life;

        [FoldoutGroup("Player", true)]
        public float _MovementSpeed = 1000.0f;
        public static float MovementSpeed => Instance._MovementSpeed;

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
    }
}