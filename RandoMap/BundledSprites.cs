using UE = UnityEngine;
using IO = System.IO;
using Collections = System.Collections.Generic;

namespace RandoMap
{
    internal static class BundledSprites
    {
        private static Collections.Dictionary<string, UE.Texture2D> textures = new();
        private static Collections.Dictionary<(string, float), UE.Sprite> sprites = new();

        public static UE.Sprite Get(string name, float ppu = 100)
        {
            if (sprites.TryGetValue((name, ppu), out var sprite))
            {
                return sprite;
            }
            var tex = LoadTexture(name);
            sprite = UE.Sprite.Create(tex, new UE.Rect(0, 0, tex.width, tex.height), new UE.Vector2(.5f, .5f), ppu);
            sprites[(name, ppu)] = sprite;
            return sprite;
        }

        private static UE.Texture2D LoadTexture(string name)
        {
            if (textures.TryGetValue(name, out var tex))
            {
                return tex;
            }
            var loc = IO.Path.Combine(IO.Path.GetDirectoryName(typeof(BundledSprites).Assembly.Location), name);
            var imageData = IO.File.ReadAllBytes(loc);
            tex = new UE.Texture2D(1, 1, UE.TextureFormat.RGBA32, false);
            UE.ImageConversion.LoadImage(tex, imageData, true);
            tex.filterMode = UE.FilterMode.Point;
            textures[name] = tex;
            return tex;
        }
    }
}