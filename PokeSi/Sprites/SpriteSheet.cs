using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace PokeSi.Sprites
{
    public class SpriteSheet
    {
        public Texture2D Sheet { get; protected set; }
        private Rectangle[,] spritesRect;

        public SpriteSheet(PokeSiGame game, string fileName, int spriteWidth, int spriteHeight, int offsetX = 0, int offsetY = 0)
        {
            Sheet = game.Content.Load<Texture2D>(fileName);

            int nbSpriteX = Sheet.Width / (spriteWidth + offsetX);
            int nbSpriteY = Sheet.Height / (spriteHeight + offsetY);

            spritesRect = new Rectangle[nbSpriteX, nbSpriteY];
            for (int y = 0; y < nbSpriteY; y++)
                for (int x = 0; x < nbSpriteX; x++)
                    spritesRect[x, y] = new Rectangle(x * (spriteWidth + offsetX), y * (spriteHeight + offsetY), spriteWidth, spriteHeight);
        }

        public Rectangle GetSpriteRect(int x, int y)
        {
            return spritesRect[x, y];
        }
    }
}
