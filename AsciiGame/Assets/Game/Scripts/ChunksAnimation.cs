namespace Krakjam
{
    using UnityEngine;

    public class ChunksAnimation : MonoBehaviour
    {
        public SplitScreenToChunks Controller;

        public AnimationCurve Curve;
        public Vector2Int StartCount;
        public Vector2Int EndCount;
        public float Period;

        public void Update()
        {
            _Timer += Time.deltaTime;
            var value = Vector2.Lerp(StartCount, EndCount, _Timer / Period);

            Controller.Resize((int)value.x, (int)value.y);
            Controller.Exposure = Mathf.Min(1.0f, 0.5f * _Timer / Period);
        }

        private float _Timer;
    }
}