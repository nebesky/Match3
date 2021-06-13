namespace Match3.structs
{
    //structs for texture packer
    public class SpriteInfo
    {
        public string filename;
        public SpriteRectangle frame;
        public bool rotated;
        public bool trimmed;
        public SpriteRectangle spriteSourceSize;
        public SpriteSize sourceSize;
        public SpriteVector pivot;
    }

    public struct SpriteSize
    {
        public int w;
        public int h;
    }

    public struct SpriteVector
    {
        public float x;
        public float y;
    }

    public struct SpriteRectangle
    {
        public int x;
        public int y;
        public int w;
        public int h;
    }
}