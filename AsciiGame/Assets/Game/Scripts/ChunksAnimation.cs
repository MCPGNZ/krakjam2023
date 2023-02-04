namespace Krakjam
{
    using UnityEngine;

    public class ChunksAnimation : MonoBehaviour
    {
        public SplitScreenToChunks Controller;

        public AnimationCurve Curve;
        public AnimationCurve ExposureCurve;

        public Vector2Int StartCount;
        public Vector2Int EndCount;
        public float Period;

        public void Update()
        {
            _Timer += Time.deltaTime;
            var value = Vector2.Lerp(StartCount, EndCount, Curve.Evaluate(_Timer / Period));

            Controller.Resize((int)value.x, (int)value.y);
            Controller.Exposure = ExposureCurve.Evaluate(_Timer / Period);
        }

        private float _Timer;
    }
}