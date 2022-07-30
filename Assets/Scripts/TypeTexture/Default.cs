namespace DefaultNamespace
{
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    public class Default 
    {
        [HideInEditorMode] public TextureImporterType        _textureType  = TextureImporterType.Default;
        public                    TextureImporterShape       _textureShape = TextureImporterShape.Texture2D;
        [Space] public            bool                       _sRGB         = false;
        public                    TextureImporterAlphaSource _alphaSource;
        [Space] public            WrapMode                   WrapMode;
        public                    FilterMode                 FilterMode;
        [Range(0, 16)] public     int                        AnisoLevel;
    }
}