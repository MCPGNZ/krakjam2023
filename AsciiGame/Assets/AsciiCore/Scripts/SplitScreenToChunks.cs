namespace Krakjam
{
    using System;
    using System.Runtime.InteropServices;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [ExecuteInEditMode]
    public sealed class SplitScreenToChunks : MonoBehaviour
    {
        #region Public Variables
        [ShowInInspector]
        public float Exposure
        {
            get => _Exposure;
            set
            {
                _Exposure = value;
                _GenerateAsciiTexture.SetFloat("_Exposure", _Exposure);
            }
        }
        #endregion Public Variables

        #region Public Methods
        [Button]
        public void Resize(int chunkX, int chunkY)
        {
            if (chunkX == _ChunkSizeX && chunkY == _ChunkSizeY)
            {
                return;
            }

            _ChunkSizeX = Mathf.Max(chunkX, 8);
            _ChunkSizeY = Mathf.Max(chunkY, 8);
            CreateChunkTextures(_ChunkSizeX, _ChunkSizeY);
        }

        public int ChunkSizeX => _ChunkSizeX;
        public int ChunkSizeY => _ChunkSizeY;
        #endregion Public Methods

        #region Inspector Variables
        [Header("Shaders")]
        [SerializeField] private ComputeShader _PrepareTextureToGenerateAsciiTexture;

        [Header("Materials")]
        [SerializeField] private Material _CreateChunksFromScreenSpaceTexture;
        [SerializeField] private Material _GenerateAsciiTexture;

        [Header("Necassary Objects")]
        [SerializeField] private SymbolList _SymbolList;

        [Header("Variables")]
        [Range(2, 4096)]
        [SerializeField] private int _ChunkSizeX = 12;
        [Range(2, 4096)]
        [SerializeField] private int _ChunkSizeY = 12;
        #endregion Inspector Variables

        #region Unity Methods
        private void Awake()
        {
            _MainCamera = GetComponent<Camera>();

            _CreateChunksFromScreenSpaceTexture = new Material(Shader.Find("Krakjam/ScreenSpaceChunks"));
            _GenerateAsciiTexture = new Material(Shader.Find("Krakjam/AsciiTexture"));

            _KernelId = _PrepareTextureToGenerateAsciiTexture.FindKernel("PrepareToCreateAsciiTexture");
        }
        private void Start()
        {
            _SplitScreenTexture = new RenderTexture(_MainCamera.pixelWidth, _MainCamera.pixelHeight, 24, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };
            _SplitScreenTexture.Create();

            _ResultHighQuality = new RenderTexture(_MainCamera.pixelWidth, _MainCamera.pixelHeight, 24, RenderTextureFormat.ARGBFloat)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };
            _ResultHighQuality.Create();

            _AsciiTexture = new RenderTexture(_MainCamera.pixelWidth, _MainCamera.pixelHeight, 24, RenderTextureFormat.ARGB32)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };
            _AsciiTexture.Create();

            CreateChunkTextures(_ChunkSizeX, _ChunkSizeY);

            CreateTextureArray();
            _CurrentSymbolsDefinition = new SymbolDefs[_SymbolList.Definitions.Count];

            FillSymbolsDefinition();
            _SymbolDefinitionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _SymbolList.Definitions.Count, 5 * sizeof(float));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            _CreateChunksFromScreenSpaceTexture.SetInt(_ChunkSizeXId, _ChunkSizeX);
            _CreateChunksFromScreenSpaceTexture.SetInt(_ChunkSizeYId, _ChunkSizeY);
            Graphics.Blit(source, _SplitScreenTexture, _CreateChunksFromScreenSpaceTexture);
            Graphics.Blit(_SplitScreenTexture, _SplitScreenTextureLessQuality);

            _PrepareTextureToGenerateAsciiTexture.SetTexture(_KernelId, _ResultTextureId, _ResultTexture);
            _PrepareTextureToGenerateAsciiTexture.SetTexture(_KernelId, _ScreenSpaceChunkTextureId, _SplitScreenTextureLessQuality);

            _SymbolDefinitionBuffer.SetData(_CurrentSymbolsDefinition);

            _PrepareTextureToGenerateAsciiTexture.SetBuffer(_KernelId, _SimpleDefinitionId, _SymbolDefinitionBuffer);
            _PrepareTextureToGenerateAsciiTexture.SetInt(_TextureSizeId, _SymbolList.Definitions.Count);
            _PrepareTextureToGenerateAsciiTexture.Dispatch(_KernelId, _ResultTexture.width, _ResultTexture.height, 1);

            Graphics.Blit(_ResultTexture, _ResultHighQuality);

            _GenerateAsciiTexture.SetTexture(_CharacterTextureArrayId, _Texture2DArray);
            _GenerateAsciiTexture.SetInt(_ChunkSizeXId, _ChunkSizeX);
            _GenerateAsciiTexture.SetInt(_ChunkSizeYId, _ChunkSizeY);

            Graphics.Blit(_ResultHighQuality, _AsciiTexture, _GenerateAsciiTexture);
            Graphics.Blit(_AsciiTexture, destination);
        }

        private void OnDestroy()
        {
            SafeDestroy(ref _CreateChunksFromScreenSpaceTexture);
            SafeDestroy(ref _GenerateAsciiTexture);

            SafeDestroy(ref _Texture2DArray);

            SafeDestroy(ref _SplitScreenTextureLessQuality);
            SafeDestroy(ref _SplitScreenTexture);

            SafeDestroy(ref _ResultTexture);
            SafeDestroy(ref _ResultHighQuality);
            SafeDestroy(ref _AsciiTexture);
        }
        #endregion Unity Methods

        #region Private Types
        [StructLayout(LayoutKind.Sequential)]
        private struct SymbolDefs
        {
            public float left;
            public float right;
            public float top;
            public float bottom;

            public float avarage;

            public SymbolDefs(float l, float r, float b, float t, float a)
            {
                left = l;
                right = r;
                bottom = b;
                top = t;
                avarage = a;
            }
        };
        #endregion Private Types

        #region Private Variables
        private Camera _MainCamera;

        private RenderTexture _SplitScreenTexture;
        private RenderTexture _SplitScreenTextureLessQuality;

        private RenderTexture _ResultTexture;
        private RenderTexture _ResultHighQuality;
        private RenderTexture _AsciiTexture;
        private Texture2DArray _Texture2DArray;

        private static readonly int _ChunkSizeXId = Shader.PropertyToID("_ChunkSizeX");
        private static readonly int _ChunkSizeYId = Shader.PropertyToID("_ChunkSizeY");

        private static readonly int _ScreenSpaceChunkTextureId = Shader.PropertyToID("_ScreenSpaceTexutre");
        private static readonly int _ResultTextureId = Shader.PropertyToID("_ResultTexture");
        private static readonly int _SimpleDefinitionId = Shader.PropertyToID("_SimpleDefinition");
        private static readonly int _TextureSizeId = Shader.PropertyToID("_TextureSize");

        private static readonly int _CharacterTextureArrayId = Shader.PropertyToID("_CharacterTextureArray");
        #endregion Private Variables

        #region Private Methods
        private SymbolDefs[] _CurrentSymbolsDefinition;
        private GraphicsBuffer _SymbolDefinitionBuffer;

        private int _KernelId;
        private float _Exposure;

        private void CreateTextureArray()
        {
            // Create Texture2DArray
            var characterTexture = _SymbolList.Definitions[0].Texture;

            var texture2DArray = new
                Texture2DArray(characterTexture.width,
                    characterTexture.height, _SymbolList.Definitions.Count,
                    TextureFormat.RGBA32, true, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Repeat
            };

            // Apply settings
            // Loop through ordinary textures and copy pixels to the
            // Texture2DArray
            for (int i = 0; i < _SymbolList.Definitions.Count; i++)
            {
                var texture = _SymbolList.Definitions[i].Texture;
                texture2DArray.SetPixels(texture.GetPixels(0),
                    i, 0);
            }
            // Apply our changes
            texture2DArray.Apply();

            _Texture2DArray = texture2DArray;
        }
        private void FillSymbolsDefinition()
        {
            for (int i = 0; i < _SymbolList.Definitions.Count; ++i)
            {
                _CurrentSymbolsDefinition[i].left = _SymbolList.Definitions[i].Left;
                _CurrentSymbolsDefinition[i].right = _SymbolList.Definitions[i].Right;
                _CurrentSymbolsDefinition[i].top = _SymbolList.Definitions[i].Top;
                _CurrentSymbolsDefinition[i].bottom = _SymbolList.Definitions[i].Bottom;
                _CurrentSymbolsDefinition[i].avarage = _SymbolList.Definitions[i].Average;
            }
        }

        private void CreateChunkTextures(int chunksX, int chunksY)
        {
            var chunksCount = new Vector2Int(_MainCamera.pixelWidth / chunksX, _MainCamera.pixelHeight / chunksY);
            var lowerQualityResolution = new Vector2Int(chunksCount.x * 2, chunksCount.y * 2);

            if (_SplitScreenTextureLessQuality != null) { _SplitScreenTextureLessQuality.Release(); }
            _SplitScreenTextureLessQuality = new RenderTexture(lowerQualityResolution.x, lowerQualityResolution.y, 24, RenderTextureFormat.ARGBHalf)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };
            _SplitScreenTextureLessQuality.Create();

            if (_ResultTexture != null) { _ResultTexture.Release(); }
            _ResultTexture = new RenderTexture(lowerQualityResolution.x, lowerQualityResolution.y, 24, RenderTextureFormat.ARGBHalf)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };
            _ResultTexture.Create();
        }

        private static void SafeDestroy(ref Material material)
        {
            if (material == null) { return; }

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                DestroyImmediate(material);
                material = null;
                return;
            }
#endif
            Destroy(material);
            material = null;
        }
        private static void SafeDestroy(ref Texture2DArray array)
        {
            if (array == null) { return; }

#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                DestroyImmediate(array);
                array = null;
                return;
            }
#endif
            Destroy(array);
            array = null;
        }
        private static void SafeDestroy(ref RenderTexture array)
        {
            if (array == null) { return; }

            array.Release();
            array = null;
        }
        #endregion Private Methods
    }
}