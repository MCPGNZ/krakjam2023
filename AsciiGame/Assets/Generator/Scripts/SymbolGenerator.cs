namespace Krakjam
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEditor;
    using UnityEngine;

    public class SymbolGenerator : MonoBehaviour
    {
        public Camera Camera;
        public TMP_Text Text;

        [FolderPath(AbsolutePath = false)]
        public string TexturesPath;

        [FolderPath(AbsolutePath = false)]
        public string DefinitionPath;

        [Button]
        public List<SymbolDefinition> Generate(SymbolList list)
        {
            var result = new List<SymbolDefinition>();
            var characters = list.Characters.ToCharArray();
            foreach (var character in characters)
            {
                result.Add(Generate(character.ToString()));
            }

            list.Definitions = result;
            return result;
        }

        [Button]
        public SymbolDefinition Generate(string character)
        {
            var symbol = RenderCharacter(character);
            var definition = GenerateDefinition(character, symbol);

            return definition;
        }

        public Texture2D RenderCharacter(string character)
        {
            Text.SetText(character);
            Camera.Render();

            var renderTexture = Camera.targetTexture;
            var texture2D = renderTexture.ToTexture2D();

            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(TexturesPath + $"\\Symbol_{character}.asset");

            AssetDatabase.CreateAsset(texture2D, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = texture2D;
            return texture2D;
        }
        public SymbolDefinition GenerateDefinition(string character, Texture2D symbol)
        {
            var definition = (SymbolDefinition)ScriptableObject.CreateInstance(typeof(SymbolDefinition));
            definition.Character = character;
            definition.Texture = symbol;

            CalculateDefinition(definition);

            /* save assets */
            var uniquePath = AssetDatabase.GenerateUniqueAssetPath(DefinitionPath + $"\\{symbol.name}.asset");

            AssetDatabase.CreateAsset(definition, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = definition;
            return definition;
        }

        public static void CalculateDefinition(SymbolDefinition definition)
        {
            var texture = definition.Texture;
            if (texture == null)
            {
                throw new InvalidOperationException("Texture is null!");
            }

            var pixels = texture.GetPixels();
            var width = texture.width;
            var height = texture.height;

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    var index = x + y * width;

                    var pixel = pixels[index];
                    var luminosity = CalculateLuminance(pixel);

                    /* regions */
                    definition.Left += luminosity * IsLeft(x, y, width, height);
                    definition.Right += luminosity * IsRight(x, y, width, height);
                    definition.Top += luminosity * IsTop(x, y, width, height);
                    definition.Bottom += luminosity * IsBottom(x, y, width, height);
                    definition.Average += luminosity;
                }
            }

            var inverseArea = 100.0f / (width * height);
            var inverseHalfArea = 2.0f * inverseArea;

            definition.Left *= inverseHalfArea;
            definition.Right *= inverseHalfArea;
            definition.Top *= inverseHalfArea;
            definition.Bottom *= inverseHalfArea;
            definition.Average *= inverseArea;
        }

        private static int IsLeft(int x, int y, int width, int height)
        {
            return (x < (width * 0.5f)) ? 1 : 0;
        }
        private static int IsRight(int x, int y, int width, int height)
        {
            return (x > (width * 0.5f)) ? 1 : 0;
        }
        private static int IsTop(int x, int y, int width, int height)
        {
            return (y > (height * 0.5f)) ? 1 : 0;
        }
        private static int IsBottom(int x, int y, int width, int height)
        {
            return (y < (height * 0.5f)) ? 1 : 0;
        }

        private static float CalculateLuminance(Color color)
        {
            return (0.299f * color.r + 0.587f * color.g + 0.114f * color.b);
        }
    }
}