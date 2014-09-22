using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace PokeSi.Sprites
{
    public class Sprite
    {
        public SpriteSheet Sheet { get; protected set; }
        public Rectangle SourceRect { get; protected set; }
        public int Width { get { return SourceRect.Width; } }
        public int Height { get { return SourceRect.Height; } }

        public Sprite(SpriteSheet sheet, Rectangle sourceRect)
        {
            Sheet = sheet;
            SourceRect = sourceRect;
        }
    }
}
