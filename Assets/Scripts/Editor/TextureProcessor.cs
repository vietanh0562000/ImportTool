using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class TextureProcessor : AssetPostprocessor
{
    private const string TEXTURE_PATTERN = @"/Texture/";
    private const string SPRITE_PATTERN = @"/Sprite/";
    private ToTiny _tiny = Object.FindObjectOfType<ToTiny>();

    void OnPreprocessTexture()
    {
        if (assetPath.Contains(SPRITE_PATTERN))
        {
            // TODO if needed
            TextureImporter importer = assetImporter as TextureImporter;

            importer.textureType = TextureImporterType.Default;
            importer.npotScale = TextureImporterNPOTScale.None;           // Do not modify texture size.
            importer.isReadable = true;

            importer.mipmapEnabled = false;
            importer.anisoLevel    = 0;                                  // RTS game do need anisoLevel
        }
    }

    private void OnPostprocessTexture(Texture2D texture)
    {
        int lastCross = assetPath.LastIndexOf("/", StringComparison.Ordinal);
        texture.name = assetPath.Substring(lastCross + 1);
        var dir = Application.dataPath + assetPath.Substring(0, lastCross + 1);
        _tiny.GetTinyPNG(texture);
    }
    
    
}