using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class TextureProcessor : AssetPostprocessor
{
    private ToTiny _tiny = Object.FindObjectOfType<ToTiny>();

    void OnPreprocessTexture()
    {
        ImportToolEditor.FolderAddressList = JsonConvert.DeserializeObject<Dictionary<string, ImageSetting>>(
            File.ReadAllText(ImportToolEditor._jsonDataFilePath));
        foreach (var folder in ImportToolEditor.FolderAddressList)
        {
            if (assetPath.Contains(folder.Key))
            {
                // TODO if needed
                TextureImporter importer = assetImporter as TextureImporter;

                importer.textureType = (TextureImporterType) folder.Value.textureType;
                //importer.textureShape = (TextureImporterShape) folder.Value.textureShape;
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