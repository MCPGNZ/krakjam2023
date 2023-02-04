namespace Krakjam
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class MonsterController : MonoBehaviour
    {
        public PlayerController Player;

        public Vector3 Begin;
        public Vector3 End;
        public AnimationCurve Curve;

        [Button]
        public void SetBegin()
        {
            Begin = transform.localPosition;
        }
        [Button]
        public void SetEnd()
        {
            End = transform.localPosition;
        }

        [Button]
        public void MoveToBegin()
        {
            transform.localPosition = Begin;
        }
        [Button]
        public void MoveToEnd()
        {
            transform.localPosition = End;
        }

        private void Update()
        {
            var t = Player.Life / GameBalance.Life;
            var c = Curve.Evaluate(t);
            transform.localPosition = Vector3.Lerp(End, Begin, c);
        }
    }
}