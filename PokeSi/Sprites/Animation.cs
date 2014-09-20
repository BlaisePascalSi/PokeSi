using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeSi.Sprites
{
    public class Animation
    {
        public SpriteSheet SpriteSheet { get; protected set; }
        public float FrameTime { get; set; }
        public int FrameCount { get; set; }
        public int XBase { get; set; }
        public int YBase { get; set; }
        public int XMult { get; set; }
        public int YMult { get; set; }
        public bool IsLooping { get; set; }

        public Animation(SpriteSheet sheet, float frameTime, int frameCount, int xBase = 0, int yBase = 0, int xMult = 1, int yMult = 0, bool isLooping = true)
        {
            SpriteSheet = sheet;
            FrameTime = frameTime;
            FrameCount = frameCount;
            XBase = xBase;
            YBase = yBase;
            XMult = xMult;
            YMult = yMult;
            IsLooping = isLooping;
        }
    }
}
