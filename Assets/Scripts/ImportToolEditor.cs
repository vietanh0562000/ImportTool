using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DefaultNamespace;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using Sprite = DefaultNamespace.Sprite;

public enum TextureType
{
    Default,
    NormalMap,
    Sprite
}

public class ImportToolEditor : OdinEditorWindow
{
    public static Dictionary<string, TypeTexture> FolderAddressList        = new Dictionary<string, TypeTexture>();
    public static Dictionary<string, Sprite>      FolderSpriteAddressList  = new Dictionary<string, Sprite>();
    public static Dictionary<string, Default>     FolderDefaultAddressList = new Dictionary<string, Default>();
    public static Dictionary<string, NormalMap>   FolderNMAddressList      = new Dictionary<string, NormalMap>();
    public static string                          _jsonDataFilePath        = "Assets/Scripts/FolderSettingData.json";

    [MenuItem("Tools/3DX Importer/Lovely Importer")]
    private static void OpenWindow()
    {
        var window = GetWindow<ImportToolEditor>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
    }

    [HorizontalGroup("Path")] public string _pathFolder;

    [HorizontalGroup("Path")]
    [Button]
    void SelectFolder()
    {
        _pathFolder = EditorUtility.SaveFolderPanel("Select folder baker", "", "");
        int assetsAddressIndex = _pathFolder.IndexOf("/Assets/", StringComparison.Ordinal);
        _pathFolder = _pathFolder.Substring(assetsAddressIndex + 1);
    }

    [Space] public        TextureType _textureType;

    [Space] [ShowIf("_textureType", TextureType.Default)]
    public Default Default = new Default();

    [ShowIf("_textureType", TextureType.Sprite)]
    public Sprite Sprite = new Sprite();

    [ShowIf("_textureType", TextureType.NormalMap)]
    public NormalMap NormalMap = new NormalMap();

    [Button]
    void CookAsset()
    {
        //TODO: VA----- Rewrite setting for selected folder
        SetImageSetting(_textureType);
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
        
        File.WriteAllText(_jsonDataFilePath, JsonConvert.SerializeObject(FolderAddressList));
    }

    private void SetTypeDefault()
    {
        if (!FolderAddressList.ContainsKey(_pathFolder))
        {
            FolderAddressList.Add(_pathFolder, Default);
        }
        else
        {
            FolderAddressList[_pathFolder] = Default;
        }
    }
    private void SetTypeNormalMap()
    {
        if (!FolderAddressList.ContainsKey(_pathFolder))
        {
            FolderAddressList.Add(_pathFolder, NormalMap);
        }
        else
        {
            FolderAddressList[_pathFolder] = NormalMap;
        }
    }
    private void SetTypeSprite()
    {
        if (!FolderAddressList.ContainsKey(_pathFolder))
        {
            FolderAddressList.Add(_pathFolder, Sprite);
        }
        else
        {
            FolderAddressList[_pathFolder] = Sprite;
        }
    }
}