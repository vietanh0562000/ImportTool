using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;
using TinifyAPI;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;


public class ToTiny : MonoBehaviour
{
    [SerializeField] private Texture2D texture;
    private const string API_KEY = "OmpDV1EzZktrWjBLVmxZM25CTHgxUXZ4anRWSFZjOVRj";

    private string _dir = "";
    
    public void GetTinyPNG(Texture2D texture2D)
    {
        texture = new Texture2D(texture2D.width, texture2D.height, TextureFormat.RGBA32, false);
        texture.SetPixels(texture2D.GetPixels());
        texture.Apply();
        texture.name = texture2D.name.ToString();
        Debug.Log(texture.name);
        StartCoroutine(GetRequest("https://api.tinify.com/shrink"));
    }

    IEnumerator GetRequest(string uri)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        unityWebRequest.SetRequestHeader("Authorization","Basic " + API_KEY);
        
        var data = texture.EncodeToPNG();
        unityWebRequest.uploadHandler = new UploadHandlerRaw(data);

        unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
    
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError)
        {
            Debug.Log("Error");
        }
        else
        {
            var url = GetDataResponse(unityWebRequest.downloadHandler.text, "url", DataType.String);
            
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
            
            yield return uwr.SendWebRequest();
            
            SaveImage(uwr.downloadHandler.data);
        }   
    }

    enum DataType
    {
        String, Number
    }
    string GetDataResponse(string str, string key, DataType dataType)
    {
        var pos = str.IndexOf(key, StringComparison.Ordinal) + key.Length;
        switch (dataType)
        {
            case DataType.String:
                pos += 3;
                break;
            case DataType.Number:
                pos += 2;
                break;
        }
        string result = "";
        for (int i = pos; i < str.Length; i++)
        {
            result += str[i];
            
            if ((str[i + 1] == '"' && dataType == DataType.String) || (str[i + 1] == ',' && dataType == DataType.Number))
            {
                break;
            }
        }
        return result;
    }
    
    void SaveImage(byte[] data)
    {
        var dirPath = Application.dataPath + "/Sprite/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }

        Debug.Log(dirPath);
        File.WriteAllBytes(dirPath + texture.name, data);
    }
    
    [Serializable]
    public class Data
    {
        public Dictionary<string, string> source;

        public Data(Dictionary<string, string> source)
        {
            this.source = source;
        }
    }
}
