namespace DefaultNamespace
{
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    public class NormalMap : TypeTexture
    {
        public NormalMap() { _textureType = TextureImporterType.NormalMap; }

        public                    TextureImporterShape _textureShape = TextureImporterShape.Texture2D;

        [Space] public bool ignorePNGFileGamma;
        public         bool createfromGrayscale;

        [Space] public        WrapMode   WrapMode;
        public                FilterMode FilterMode;
        [Range(0, 16)] public int        AnisoLevel;
    }
}