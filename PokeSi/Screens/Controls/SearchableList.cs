using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Screens.Controls
{
    public class SearchableList : Control
    {
        public int SelectedIndex { get; protected set; }
        public string SelectedItem { get; protected set; }
        public List<string> Items { get { return list.Select(x => x.Text).ToList(); } }

        private List<TextBlock> list;
        private TextBox searchBar;
        private Stack stack;

        private Sprite background;
        private SpriteFont font;

        public SearchableList(Screen screen, Rectangle bound, Sprite background, Sprite[] searchSprite, SpriteFont font)
            : base(screen)
        {
            list = new List<TextBlock>();
            Bound = bound;
            searchBar = new TextBox(Screen, new Rectangle(0, 0, Bound.Width - 10, 25), searchSprite) { Parent = this };
            stack = new Stack(Screen) { Bound = new Rectangle(0, 30, Bound.Width - 10, Bound.Height - 30), Parent = this };
            SelectedItem = null;
            this.background = background;
            this.font = font;
        }

        public void AddToList(string value)
        {
            list.Add(new TextBlock(Screen, new Vector2(0, (list.Count + 1) * 25 + 5), value) { Parent = this });
        }

        public void RemoveFromList(string value)
        {
            list.RemoveAll(x => x.Text == value);
        }
        public void RemoveFromList(int index)
        {
            list.RemoveAt(index);
        }

        public void RemoveAllFromList()
        {
            list = new List<TextBlock>();
            SelectedItem = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            searchBar.Update(gameTime);
            stack.Update(gameTime);

            if (DestinationRect.Contains(Input.X, Input.Y))
            {
                if (Input.LeftButton.Pressed)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].DestinationRect.Contains(Input.X, Input.Y) && stack.Elements.Contains(list[i]))
                        {
                            SelectedItem = list[i].Text;
                            SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            spriteBatch.Draw(background.Sheet.Texture, DestinationRect, background.SourceRect, Color.White);

            searchBar.Draw(gameTime, spriteBatch);

            List<TextBlock> toDraw = list.FindAll(x => x.Text.StartsWith(searchBar.Text));
            stack.Elements = toDraw.ToList<Control>();

            Rectangle itemRectangle = DestinationRect;
            itemRectangle.Height = 30;
            if (toDraw.Count > 0 && SelectedItem != null)
            {
                TextBlock textBlock = list[SelectedIndex];
                if (textBlock != null && toDraw.Contains(textBlock) && stack.IsElementDisplayed(stack.Elements.IndexOf(textBlock)))
                    spriteBatch.Draw(background.Sheet.Texture, textBlock.DestinationRect, background.SourceRect, Color.White);
                else
                    SelectedItem = null;
            }
            stack.Draw(gameTime, spriteBatch);
        }
    }
}
