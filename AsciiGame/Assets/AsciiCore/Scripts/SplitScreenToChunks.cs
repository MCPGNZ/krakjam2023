namespace Krakjam
{
    using System;
    using UnityEngine;

    public sealed class SplitScreenToChunks : MonoBehaviour
    {
        #region Inspector Variables
        [Range(0, 4096)]
        [SerializeField] private int _ChunkSizeX;
        [Range(0, 4096)]
        [SerializeField] private int _ChunkSizeY;
        [SerializeField] private Material _CreateChunksFromScreenSpaceTexture;

        [SerializeField] private Camera _MainCamera;

        [SerializeField] private ComputeShader _GenerateAsciiForScreen;

        [SerializeField] private RenderTexture _ResultTexture;

        [SerializeField] private SymbolDef[] _SymbolDefinitionBuffer;

        [SerializeField] private bool _IsChanged = true;
        #endregion Inspector Variables

        #region Unity Methods

        private void Start()
        {
            _CreateChunksFromScreenSpaceTexture = new Material(Shader.Find("Krakjam/ScreenSpaceChunks"));

            _KernelId = _GenerateAsciiForScreen.FindKernel("ScreenSpaceAsciiGenerator");
            _GenerateAsciiForScreen.SetVector(_ScreenSpaceResolutionId, _Resolution);

            _SplitScreenTexture = new RenderTexture(_MainCamera.pixelWidth, _MainCamera.pixelHeight, 24, RenderTextureFormat.ARGBFloat)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };

            _SplitScreenTexture.Create();

            _ChunksCount = new Vector2(_MainCamera.pixelWidth / _ChunkSizeX, _MainCamera.pixelHeight / _ChunkSizeY);

            _LessQualityResolution = new Vector2(_ChunksCount.x * 4, _ChunksCount.y * 4);

            _SymbolDefinitionBuffer = new SymbolDef[(int)_ChunksCount.x * (int)_ChunksCount.y];

            _SplitScreenTextureLessQuality = new RenderTexture((int)_LessQualityResolution.x, (int)_LessQualityResolution.y, 24, RenderTextureFormat.ARGBFloat)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };

            _SplitScreenTextureLessQuality.Create();

            _ResultTexture = new RenderTexture((int)_LessQualityResolution.x, (int)_LessQualityResolution.y, 24, RenderTextureFormat.ARGBFloat)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };

            _ResultTexture.Create();

            _Resolution = new Vector2(_LessQualityResolution.x, _LessQualityResolution.y);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _CreateChunksFromScreenSpaceTexture.SetInt(_ChunkSizeXId, _ChunkSizeX);
            _CreateChunksFromScreenSpaceTexture.SetInt(_ChunkSizeYId, _ChunkSizeY);
            Graphics.Blit(source, _SplitScreenTexture, _CreateChunksFromScreenSpaceTexture);
            Graphics.Blit(_SplitScreenTexture, _SplitScreenTextureLessQuality);
            Graphics.Blit(source, destination);

            _GenerateAsciiForScreen.SetTexture(_KernelId, _ChunkScreenTextureId, _SplitScreenTextureLessQuality);
            _GenerateAsciiForScreen.SetTexture(_KernelId, _ResultTextureId, _ResultTexture);

            _GenerateAsciiForScreen.SetInt(_ChunkSizeXId, _MainCamera.pixelWidth / _ChunkSizeX);

            _GenerateAsciiForScreen.Dispatch(_KernelId, (int)_LessQualityResolution.x, (int)_LessQualityResolution.y, 1);

            Texture2D result = toTexture2D(_ResultTexture);
            var pixels = result.GetPixels32();
            int id = 0;
            for (int y = 0; y < result.height; ++y)
            {
                for (int x = 0; x < result.width; ++x)
                {
                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        var index = x + y * result.width;
                        var pixel = pixels[index];
                        var left = (pixel.r + pixel.a) * 0.5f;
                        var right = (pixel.g + pixel.a) * 0.5f;
                        var top = (pixel.b + pixel.a) * 0.5f;
                        var bottom = (pixel.r + pixel.g) * 0.5f;
                        var avarage = (pixel.r + pixel.g + pixel.b + pixel.a) * 0.25f;

                        _SymbolDefinitionBuffer[id].left = left;
                        _SymbolDefinitionBuffer[id].right = right;
                        _SymbolDefinitionBuffer[id].top = top;
                        _SymbolDefinitionBuffer[id].bottom = bottom;
                        _SymbolDefinitionBuffer[id].avarage = avarage;
                        id++;
                    }
                }
            }
        }
        #endregion Unity Methods

        #region Private Types
        [Serializable]
        private struct SymbolDef
        {
            public float left;
            public float right;
            public float top;
            public float bottom;
            public float avarage;

            public SymbolDef(float lef, float rig, float tp, float bot, float avg)
            {
                left = lef;
                right = rig;
                top = tp;
                bottom = bot;
                avarage = avg;
            }
        }
        #endregion Private Types

        #region Private Variables
        private RenderTexture _SplitScreenTexture;
        private RenderTexture _SplitScreenTextureLessQuality;
        private Vector2 _Resolution;

        private int _KernelId;

        private Vector2 _ChunksCount;

        private Vector2 _LessQualityResolution;

        private static readonly int _ChunkSizeXId = Shader.PropertyToID("_ChunkSizeX");
        private static readonly int _ChunkSizeYId = Shader.PropertyToID("_ChunkSizeY");

        private static readonly int _ChunkScreenTextureId = Shader.PropertyToID("_ScreenSpaceTexutre");
        private static readonly int _ScreenSpaceResolutionId = Shader.PropertyToID("_ScreenResolution");
        private static readonly int _ResultTextureId = Shader.PropertyToID("_ResultTexture");
        #endregion Private Variables

        #region Private Variables
        private Texture2D toTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            // ReadPixels looks at the active RenderTexture.
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }
        #endregion Private Variables
    }
}