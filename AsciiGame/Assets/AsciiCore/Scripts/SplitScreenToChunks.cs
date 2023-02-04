namespace Krakjam
{
    using System;
    using UnityEngine;

    public sealed class SplitScreenToChunks : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Shaders")]
        [SerializeField] private ComputeShader _PrepareTextureToGenerateAsciiTexture;

        [Header("Materials")]
        [SerializeField] private Material _CreateChunksFromScreenSpaceTexture;
        [SerializeField] private Material _GenerateAsciiTexture;

        [Header("Necassary Objects")]
        [SerializeField] private SymbolList _SymbolList;
        [SerializeField] private SymbolDefinition[] _ChoosedSymbolDefinition;
        [SerializeField] private Camera _MainCamera;

        [Header("Variables")]
        [Range(0, 4096)]
        [SerializeField] private int _ChunkSizeX;
        [Range(0, 4096)]
        [SerializeField] private int _ChunkSizeY;

        [Header("Textures")]
        [SerializeField] private RenderTexture _ResultTexture;

        [SerializeField] private RenderTexture _ResultHighQuality;

        [SerializeField] private RenderTexture _AsciiTexture;

        [SerializeField] private Texture2DArray _Texture2DArray;
        #endregion Inspector Variables

        #region Unity Methods

        private void Start()
        {
            var chunksCount = new Vector2(_MainCamera.pixelWidth / _ChunkSizeX, _MainCamera.pixelHeight / _ChunkSizeY);

            _LessQualityResolution = new Vector2(chunksCount.x * 2, chunksCount.y * 2);

            _CreateChunksFromScreenSpaceTexture = new Material(Shader.Find("Krakjam/ScreenSpaceChunks"));
            _GenerateAsciiTexture = new Material(Shader.Find("Krakjam/AsciiTexture"));

            _SplitScreenTexture = new RenderTexture(_MainCamera.pixelWidth, _MainCamera.pixelHeight, 24, RenderTextureFormat.ARGBFloat)
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

            _AsciiTexture = new RenderTexture(_MainCamera.pixelWidth, _MainCamera.pixelHeight, 24, RenderTextureFormat.ARGBFloat)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                enableRandomWrite = true
            };

            _AsciiTexture.Create();

            CreateTextureArray();

            _CurrentSymbolsDefinition = new SymbolDefs[_SymbolList.Definitions.Count];

            FillSymbolsDefinition();

            _SymbolDefinitionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _SymbolList.Definitions.Count, 5 * sizeof(float));

            _KernelId = _PrepareTextureToGenerateAsciiTexture.FindKernel("PrepareToCreateAsciiTexture");
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
        #endregion Unity Methods

        #region Private Types
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
        private RenderTexture _SplitScreenTexture;
        private RenderTexture _SplitScreenTextureLessQuality;

        private Vector2 _LessQualityResolution;

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

        private void CreateTextureArray()
        {
            // Create Texture2DArray
            var characterTexture = _SymbolList.Definitions[0].Texture;

            Texture2DArray texture2DArray = new
                Texture2DArray(characterTexture.width,
                characterTexture.height, _SymbolList.Definitions.Count,
                TextureFormat.RGBA32, true, false);
            // Apply settings
            texture2DArray.filterMode = FilterMode.Bilinear;
            texture2DArray.wrapMode = TextureWrapMode.Repeat;
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

        #endregion Private Methods
    }
}