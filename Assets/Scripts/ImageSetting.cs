public class ImageSetting
{
    public int textureType;
    public int textureShape;
    public bool sRGB;

    public ImageSetting(int textureType, int textureShape, bool sRGB)
    {
        this.textureType = textureType;
        this.textureShape = textureShape;
        this.sRGB = sRGB;
    }
}