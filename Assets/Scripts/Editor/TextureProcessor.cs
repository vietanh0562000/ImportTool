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
        SetImageSetting(ImportToolEditor.textureType);
    }

    public void SetImageSetting(TextureType textureType)
    {
        switch (textureType)
        {
            case TextureType.Default:
                SetTypeDefault();
                break;
            case TextureType.NormalMap:
                SetTypeNormalMap();
                break;
            case TextureType.Sprite:
                SetTypeSprite();
                break;
        }
    }

    private void SetTypeDefault()
    {
        ImportToolEditor.FolderDefaultAddressList = JsonConvert.DeserializeObject<Dictionary<string, Default>>(
            File.ReadAllText(ImportToolEditor._jsonDataFilePath));
        foreach (var folder in ImportToolEditor.FolderDefaultAddressList)
        {
            if (assetPath.Contains(folder.Key))
            {
                // TODO if needed
                TextureImporter importer = assetImporter as TextureImporter;

                if (importer != null)
                {
                    importer.textureType  = folder.Value._textureType;
                    importer.textureShape = folder.Value._textureShape;
                    importer.sRGBTexture  = folder.Value._sRGB;
                    importer.alphaSource  = folder.Value._alphaSource;
                    importer.wrapMode     = (TextureWrapMode) folder.Value.WrapMode;
                    importer.filterMode   = folder.Value.FilterMode;
                    importer.anisoLevel   = folder.Value.AnisoLevel;
                }
            }   
        }
    }
    private void SetTypeNormalMap()
    {
        ImportToolEditor.FolderNMAddressList = JsonConvert.DeserializeObject<Dictionary<string, NormalMap>>(
            File.ReadAllText(ImportToolEditor._jsonDataFilePath));
        foreach (var folder in ImportToolEditor.FolderNMAddressList)
        {
            if (assetPath.Contains(folder.Key))
            {
                // TODO if needed
                TextureImporter importer = assetImporter as TextureImporter;

                if (importer != null)
                {
                    importer.textureType    = folder.Value._textureType;
                    importer.textureShape   = folder.Value._textureShape;
                    importer.ignorePngGamma = folder.Value.ignorePNGFileGamma;
                    /*importer.grayscaleToAlpha = folder.Value.createfromGrayscale;*/
                    
                    importer.wrapMode     = (TextureWrapMode) folder.Value.WrapMode;
                    importer.filterMode   = folder.Value.FilterMode;
                    importer.anisoLevel   = folder.Value.AnisoLevel;
                }
            }   
        }
    }
    private void SetTypeSprite()
    {
        ImportToolEditor.FolderSpriteAddressList = JsonConvert.DeserializeObject<Dictionary<string, Sprite>>(
            File.ReadAllText(ImportToolEditor._jsonDataFilePath));
        foreach (var folder in ImportToolEditor.FolderSpriteAddressList)
        {
            if (assetPath.Contains(folder.Key))
            {
                // TODO if needed
                TextureImporter importer = assetImporter as TextureImporter;

                if (importer != null)
                {
                    importer.textureType  = folder.Value._textureType;
                    importer.textureShape = folder.Value._textureShape;
                    importer.spriteImportMode  = folder.Value.SpriteImportMode;
                    importer.wrapMode     = (TextureWrapMode) folder.Value.WrapMode;
                    importer.filterMode   = folder.Value.FilterMode;
                    importer.anisoLevel   = folder.Value.AnisoLevel;
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
