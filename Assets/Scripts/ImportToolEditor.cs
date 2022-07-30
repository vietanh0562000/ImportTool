using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class ImportToolEditor : OdinEditorWindow
{
    public static Dictionary<string, ImageSetting> FolderAddressList = new Dictionary<string, ImageSetting>();
    public static string _jsonDataFilePath = "Assets/Scripts/FolderSettingData.json";

    [MenuItem("Tools/Lovely/Lovely Importer")]
    private static void OpenWindow()
    {
        var window = GetWindow<ImportToolEditor>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
    }

    [HorizontalGroup("Path")]
    public string _pathFolder;
    
    [HorizontalGroup("Path")]
    [Button]
    void SelectFolder()
    {
        _pathFolder = EditorUtility.SaveFolderPanel("Select folder baker", "", "");
        int assetsAddressIndex = _pathFolder.IndexOf("/Assets/", StringComparison.Ordinal);
        _pathFolder = _pathFolder.Substring(assetsAddressIndex + 1);
    }
    
    public TextureImporterType _textureType = 0;
    public TextureImporterShape _textureShape = 0;
    public bool _sRGB = false;
    public TextureImporterAlphaSource _alpha;
    public bool _generateMipmaps;
    public WrapMode _wrapMode = WrapMode.Loop;
    public FilterMode _filterMode = FilterMode.Bilinear;
    
    [Button]
    void CookAsset()
    {
        //TODO: VA----- Rewrite setting for selected folder
        ImageSetting imageSetting = new ImageSetting((int) _textureType,(int) _textureShape, _sRGB);
        if (!FolderAddressList.ContainsKey(_pathFolder))
        {
            FolderAddressList.Add(_pathFolder, imageSetting);   
        }
        else
        {
            FolderAddressList[_pathFolder] = imageSetting;
        }
        
        File.WriteAllText(_jsonDataFilePath, JsonConvert.SerializeObject(FolderAddressList));
    }
}