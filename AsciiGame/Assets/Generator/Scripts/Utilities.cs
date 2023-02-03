namespace Krakjam
{
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;

    public static class TextureUtilities
    {
        public static Texture2D ToTexture2D(this RenderTexture renderTexture, GraphicsFormat format = GraphicsFormat.R8G8B8A8_UNorm)
        {
            var result = new Texture2D(renderTexture.width, renderTexture.height, format,
                TextureCreationFlags.None)
            {
                name = renderTexture.name,
                filterMode = renderTexture.filterMode,
            };

            RenderTexture.active = renderTexture;
            result.ReadPixels(new Rect(0, 0, result.width, result.height), 0, 0);
            result.Apply(true, false);

            return result;
        }
    };
}