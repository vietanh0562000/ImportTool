namespace DefaultNamespace
{
    using Sirenix.OdinInspector;
    using UnityEditor;
    using UnityEngine;

    public class Sprite 
    {
        [HideInEditorMode]    public TextureImporterType  _textureType     = TextureImporterType.Sprite;
        [DisableInEditorMode] public TextureImporterShape _textureShape    = TextureImporterShape.Texture2D;
        [Space]               public SpriteImportMode     SpriteImportMode = SpriteImportMode.Single;
        [Space]               public WrapMode             WrapMode;
        public                       FilterMode           FilterMode;
        [Range(0, 16)] public        int                  AnisoLevel;
    }
}