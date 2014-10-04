using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Sprites;

namespace PokeSi.Screens.Controls
{
    public class FileTree : Control
    {
        public class TreeElement
        {
            public TreeElement Parent { get; internal set; }
            public List<TreeElement> Childs { get; internal set; }
            internal bool isDirectory;
            public bool IsDirectory { get { return isDirectory; } }
            public bool IsFile { get { return !isDirectory; } }
            public string Path { get; internal set; }

            internal TreeElement()
            {
                Childs = new List<TreeElement>();
                isDirectory = false;
            }
        }

        public Sprite Background { get; protected set; }
        public TreeElement Root { get; protected set; }
        public TreeElement SelectedElement { get; protected set; }

        private Dictionary<TextBlock, TreeElement> elements;

        private SpriteFont font;

        public int PreferableWidth
        {
            get
            {
                int max = 0;
                foreach (TextBlock block in elements.Keys)
                    max = Math.Max(max, block.Bound.X + block.Bound.Width);
                return max + 2 * Pading;
            }
        }

        public int PreferableHeight
        {
            get
            {
                int max = 0;
                foreach (TextBlock block in elements.Keys)
                    max = Math.Max(max, block.Bound.Y + block.Bound.Height);
                return max + 2 * Pading;
            }
        }

        public FileTree(Screen screen, Rectangle bound, Sprite background, string root, string extensionFilter = "")
            : base(screen)
        {
            Bound = bound;
            Background = background;
            elements = new Dictionary<TextBlock, TreeElement>();
            font = screen.Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");

            Root = new TreeElement() { Path = root, isDirectory = true };
            AddToTreeAllChilds(Root, extensionFilter);
        }

        private Vector2 nextPos = Vector2.Zero;
        private void AddToTreeAllChilds(TreeElement parent, string extensionFilter)
        {
            string root = parent.Path;
            foreach (string dir in Directory.GetDirectories(root))
            {
                TreeElement child = new TreeElement() { Path = dir, isDirectory = true, Parent = parent };
                elements.Add(new TextBlock(Screen, nextPos, dir.Split('\\').Last()) { Parent = this }, child);
                nextPos.X += 20;
                nextPos.Y += 30;
                AddToTreeAllChilds(child, extensionFilter);
                parent.Childs.Add(child);
            }
            foreach (string file in Directory.GetFiles(root))
            {
                if (file.EndsWith(extensionFilter))
                {
                    TreeElement child = new TreeElement() { Path = file, isDirectory = false, Parent = parent };
                    parent.Childs.Add(child);
                    elements.Add(new TextBlock(Screen, nextPos, file.Split('\\').Last()) { Parent = this }, child);
                    nextPos.Y += 30;
                }
            }
            nextPos.X -= 20;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Input.LeftButton.Pressed)
            {
                Point clickPos = new Point(Input.X, Input.Y);
                if (DestinationRect.Contains(clickPos))
                {
                    foreach (TextBlock textBlock in elements.Keys)
                    {
                        if (textBlock.DestinationRect.Contains(clickPos))
                            SelectedElement = elements[textBlock];
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if (Background != null)
                spriteBatch.Draw(Background.Sheet.Texture, DestinationRect, Background.SourceRect, Color.White);

            foreach (TextBlock textBlock in elements.Keys)
                textBlock.Draw(gameTime, spriteBatch);
        }
    }
}
