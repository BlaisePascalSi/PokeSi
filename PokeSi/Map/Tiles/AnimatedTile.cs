using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Map.Tiles
{
    public class AnimatedTile : Tile
    {
        public Animation Animation { get; protected set; }
        public AnimationPlayer AnimationPlayer { get; protected set; }

        public AnimatedTile(World world, Animation animation)
            : base(world)
        {
            Animation = animation;

            if (Animation == null)
                Animation = world.Resources.GetAnimation("tile_flower");

            AnimationPlayer = new AnimationPlayer();
            AnimationPlayer.PlayAnimation(Animation);
        }

        public override void Load(XmlDocument doc, XmlElement parent, World world)
        {
            Animation = world.Resources.GetAnimation((string)XmlHelper.GetSimpleNodeContent<string>("Animation", parent, "tile_flower"));
            AnimationPlayer = new AnimationPlayer();
            AnimationPlayer.PlayAnimation(Animation);
        }

        public override void Save(XmlDocument doc, XmlElement parent, World world)
        {
            parent.AppendChild(XmlHelper.CreateSimpleNode("Animation", world.Resources.GetName(Animation), doc));
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, int x, int y, Rectangle destinationRect)
        {
            AnimationPlayer.Draw(gameTime, spriteBatch, destinationRect, Color.White, GetDepth(x, y));
        }

        public override Rectangle GetDestinationRect(int x, int y)
        {
            return DrawHelper.ExtendToContain(new Rectangle(x * Tile.Width - AnimationPlayer.CurrentSprite.Origin.X,
                                                            y * Tile.Height - AnimationPlayer.CurrentSprite.Origin.Y,
                                                            AnimationPlayer.CurrentSprite.Width,
                                                            AnimationPlayer.CurrentSprite.Height),
                                            new Rectangle(0, 0, Tile.Width, Tile.Height),
                                            AnimationPlayer.CurrentSprite.Origin.X, AnimationPlayer.CurrentSprite.Origin.Y);
        }

        public override Form GetEditingForm()
        {
            Form result = base.GetEditingForm();
            result.Datas.Add("Animation", Animation);
            return result;
        }

        public override void SubmitForm(Form form)
        {
            base.SubmitForm(form);
            Animation = (Animation)form.Datas["Animation"];
            AnimationPlayer.PlayAnimation(Animation);
        }
    }
}
