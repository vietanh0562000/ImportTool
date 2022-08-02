namespace DefaultNamespace
{
    using Sirenix.OdinInspector;
    using UnityEditor;

    public abstract class TypeTexture
    {
        [HideInEditorMode] public TextureImporterType _textureType;
    }
}