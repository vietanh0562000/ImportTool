using System;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Sprite = DefaultNamespace.Sprite;

public class TextureProcessor : AssetPostprocessor
{
    private ToTiny _tiny = Object.FindObjectOfType<ToTiny>();

    void OnPreprocessTexture()
    {
        SetImageSetting();
    }

    public void SetImageSetting()
    {
        ImportToolEditor.FolderAddressList = JsonConvert.DeserializeObject<Dictionary<string, TypeTexture>>(
            File.ReadAllText(ImportToolEditor._jsonDataFilePath));
        foreach (var folder in ImportToolEditor.FolderAddressList)
        {
            if (assetPath.Contains(folder.Key))
            {
                // TODO if needed
                TextureImporter importer = assetImporter as TextureImporter;

                if (importer != null)
                {
                    importer.textureType  = folder.Value._textureType;

                    switch (@importer.textureType)
                    {
                        case TextureImporterType.Default:
                            var type = folder.Value as Default;
                            
                            importer.textureShape = type._textureShape;
                            importer.sRGBTexture  = type._sRGB;
                            importer.alphaSource  = type._alphaSource;
                            importer.wrapMode     = (TextureWrapMode) type.WrapMode;
                            importer.filterMode   = type.FilterMode;
                            importer.anisoLevel   = type.AnisoLevel;
                            break;
                        case TextureImporterType.Sprite:
                            var type1 = folder.Value as Sprite;
                            
                            importer.textureType      = type1._textureType;
                            importer.textureShape     = type1._textureShape;
                            importer.spriteImportMode = type1.SpriteImportMode;
                            importer.wrapMode         = (TextureWrapMode) type1.WrapMode;
                            importer.filterMode       = type1.FilterMode;
                            importer.anisoLevel       = type1.AnisoLevel;
                            break;
                        case TextureImporterType.NormalMap:
                            var type2 = folder.Value as NormalMap;
                            
                            importer.textureType    = type2._textureType;
                            importer.textureShape   = type2._textureShape;
                            importer.ignorePngGamma = type2.ignorePNGFileGamma;
                    
                            importer.wrapMode   = (TextureWrapMode) type2.WrapMode;
                            importer.filterMode = type2.FilterMode;
                            importer.anisoLevel = type2.AnisoLevel;
                            break;
                    }
                }
            }   
        }
    }

    private void OnPostprocessTexture(Texture2D texture)
    {
        foreach (var folder in ImportToolEditor.FolderAddressList)
        {
            if (assetPath.Contains(folder.Key))
            {
                int lastCross = assetPath.LastIndexOf("/", StringComparison.Ordinal);
                texture.name = assetPath.Substring(lastCross + 1);
                var dir = assetPath.Substring(0, lastCross + 1);
                _tiny.GetTinyPNG(texture, dir);
            }
        }
    }
}
