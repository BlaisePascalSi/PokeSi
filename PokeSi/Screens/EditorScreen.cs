using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Map;
using PokeSi.Map.Tiles;
using PokeSi.Map.Entities;
using PokeSi.Sprites;
using PokeSi.Screens.Controls;

namespace PokeSi.Screens
{
    public class EditorScreen : Screen
    {
        private SpriteFont font;

        public World World { get; protected set; }
        private Rectangle worldBound;

        private TabPanels tabPanel;
        private Panel tilePanel;
        private SearchableList tileList;
        private Button tileAddButton;
        private Button tileRemoveButton;
        private Form tileDataForm;
        private Panel tileDataPanel;
        private Button tileDataSubmitButton;
        private Panel testPanel;

        public EditorScreen(ScreenManager manager)
            : base(manager)
        {
            OpeningTransition = new NoneTransition(Transition.Types.Opening);
            ClosingTransition = new NoneTransition(Transition.Types.Closing);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            XmlDocument doc = new XmlDocument();
            doc.Load("save.xml");

            XmlElement worldElem = (XmlElement)doc.GetElementsByTagName("World").Item(0);
            World = new World(this, doc, worldElem);

            Sprite buttonSprite = World.Resources.GetSprite("button_idle");
            Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, World.Resources.GetSprite("button_over"), World.Resources.GetSprite("button_pressed") };
            font = Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");

            tabPanel = new TabPanels(this, Rectangle.Empty, buttonSpriteTab);
            tilePanel = new Panel(this, Rectangle.Empty, buttonSprite) { Pading = 5 };
            tileList = new SearchableList(this, Rectangle.Empty, buttonSprite, buttonSpriteTab, font);
            tilePanel.AddControl("tileList", tileList);
            tileAddButton = new Button(this, Rectangle.Empty, buttonSpriteTab) { Text = "Add" };
            tilePanel.AddControl("tileAddButton", tileAddButton);
            tileRemoveButton = new Button(this, Rectangle.Empty, buttonSpriteTab) { Text = "Remove" };
            tilePanel.AddControl("tileRemoveButton", tileRemoveButton);
            tileDataPanel = new Panel(this, Rectangle.Empty, null);
            tilePanel.AddControl("tileDataPanel", tileDataPanel);
            tabPanel.AddPanel("Tile", tilePanel, 35);

            testPanel = new Panel(this, Rectangle.Empty, buttonSprite);
            tabPanel.AddPanel("Test", testPanel, 50);

            Resize(Manager.Game.Viewport.Width, Manager.Game.Viewport.Height);
            UpdateDatas();
        }

        public override void Close()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement world = doc.CreateElement("World");
            World.Save(doc, world);
            doc.AppendChild(world);
            doc.Save("save.xml");

            base.Close();
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);

            Rectangle viewport = Manager.Game.Viewport;

            worldBound = new Rectangle(0, 0, viewport.Width * 4 / 5 / Tile.Width * Tile.Width, viewport.Height);

            tabPanel.Bound = new Rectangle(worldBound.Right, 0, viewport.Width - worldBound.Right, viewport.Height);

            tileList.Bound = new Rectangle(0, 0, tilePanel.Bound.Width - tilePanel.Pading * 2, tilePanel.Bound.Height / 2 - 25);
            tileAddButton.Bound = new Rectangle(0, tileList.Bound.Bottom, 40, 25);
            tileRemoveButton.Bound = new Rectangle(tileAddButton.Bound.Right, tileAddButton.Bound.Top, 75, 25);
            tileDataPanel.Bound = new Rectangle(tileAddButton.Bound.Left, tileAddButton.Bound.Bottom, tilePanel.Bound.Width - tilePanel.Pading * 2, tilePanel.Bound.Height - tilePanel.Pading * 2 - tileAddButton.Bound.Bottom);
        }

        private void UpdateDatas()
        {
            tileList.RemoveAllFromList();
            foreach (string key in Tile.UnLocatedTile.Keys)
                tileList.AddToList(key);
        }

        private void ReconstructTileDataPanel()
        {
            tileDataPanel.RemoveAllControl();
            if (tileList.SelectedItem != null)
            {
                tileDataPanel.AddControl("title", new TextBlock(this, Vector2.Zero, tileList.SelectedItem));
                Tile selectedTile = Tile.UnLocatedTile[tileList.SelectedItem];

                tileDataForm = selectedTile.GetEditingForm();

                Sprite buttonSprite = World.Resources.GetSprite("button_idle");
                Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, World.Resources.GetSprite("button_over"), World.Resources.GetSprite("button_pressed") };
                Rectangle nextRectangle = new Rectangle(0, 25, tileDataPanel.Bound.Width, 25);
                foreach (KeyValuePair<string, object> pair in tileDataForm.Datas)
                {
                    tileDataPanel.AddControl(pair.Key + "T", new TextBlock(this, new Vector2(0, nextRectangle.Y), pair.Key));
                    if (pair.Value is string || pair.Value is int || pair.Value is float)
                    {
                        tileDataPanel.AddControl(pair.Key, new TextBox(this, nextRectangle, buttonSpriteTab));
                        ((TextBox)tileDataPanel.SubControls[pair.Key]).Text = pair.Value.ToString();
                    }
                    else if (pair.Value is bool)
                    {
                        tileDataPanel.AddControl(pair.Key, new ToggleButton(this, nextRectangle, buttonSpriteTab));
                        ((ToggleButton)tileDataPanel.SubControls[pair.Key]).SetState((bool)pair.Value);
                    }
                    else if (pair.Value is Point)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Width = nextRectangle.Width / 2 - 5;
                        tileDataPanel.AddControl(pair.Key, new TextBox(this, rect, buttonSpriteTab));
                        ((TextBox)tileDataPanel.SubControls[pair.Key]).Text = ((Point)(pair.Value)).X.ToString();
                        rect.X += rect.Width + 10;
                        tileDataPanel.AddControl(pair.Key + "_Y", new TextBox(this, rect, buttonSpriteTab));
                        ((TextBox)tileDataPanel.SubControls[pair.Key + "_Y"]).Text = ((Point)(pair.Value)).Y.ToString();
                    }
                    else if (pair.Value is Enum)
                    {
                        Type enumType = pair.Value.GetType();
                        ListButton control = new ListButton(this, nextRectangle, buttonSprite, new List<string>(Enum.GetNames(enumType)));
                        control.SelectValue(Enum.GetName(enumType, pair.Value));
                        tileDataPanel.AddControl(pair.Key, control);
                    }
                    else if (pair.Value is Controller)
                    {
                        ListButton control = new ListButton(this, nextRectangle, buttonSprite, new List<string>(Entity.Controllers.GetAll()));
                        control.SelectValue(Entity.Controllers.GetName((Controller)pair.Value));
                        tileDataPanel.AddControl(pair.Key, control);
                    }
                    else if (pair.Value is Sprite)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Right -= 30;
                        ListButton control = new ListButton(this, rect, buttonSprite, new List<string>(World.Resources.Sprites.Keys));
                        control.SelectValue(World.Resources.GetName((Sprite)pair.Value));
                        tileDataPanel.AddControl(pair.Key, control);
                        rect.Left = rect.Right;
                        rect.Right += 30;
                        rect.Left += 10;
                        Button button = new Button(this, rect, buttonSpriteTab);
                        button.Text = "...";
                        tileDataPanel.AddControl("addSprite_" + pair.Key, button);
                    }
                    else if (pair.Value is Animation)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Right -= 30;
                        ListButton control = new ListButton(this, rect, buttonSprite, new List<string>(World.Resources.Animations.Keys));
                        control.SelectValue(World.Resources.GetName((Animation)pair.Value));
                        tileDataPanel.AddControl(pair.Key, control);
                        rect.Left = rect.Right;
                        rect.Right += 30;
                        rect.Left += 10;
                        Button button = new Button(this, rect, buttonSpriteTab);
                        button.Text = "...";
                        tileDataPanel.AddControl("addAnimation_" + pair.Key, button);
                    }
                    nextRectangle.Y += 30;
                }
                tileDataSubmitButton = new Button(this, nextRectangle, buttonSpriteTab);
                tileDataSubmitButton.Text = "Submit";
                tileDataPanel.AddControl("submitButton", tileDataSubmitButton);
            }
        }
        private void SubmitTileData()
        {
            foreach (KeyValuePair<string, Control> pair in tileDataPanel.SubControls)
            {
                if (tileDataForm.Datas.ContainsKey(pair.Key))
                {
                    if (tileDataForm.Datas[pair.Key] is string)
                        tileDataForm.Datas[pair.Key] = ((TextBox)pair.Value).Text;
                    else if (tileDataForm.Datas[pair.Key] is int)
                        tileDataForm.Datas[pair.Key] = int.Parse(((TextBox)pair.Value).Text);
                    else if (tileDataForm.Datas[pair.Key] is float)
                        tileDataForm.Datas[pair.Key] = float.Parse(((TextBox)pair.Value).Text);
                    else if (tileDataForm.Datas[pair.Key] is Point)
                    {
                        Point point = new Point();
                        point.X = int.Parse(((TextBox)pair.Value).Text);
                        point.Y = int.Parse(((TextBox)tileDataPanel.SubControls[pair.Key + "_Y"]).Text);
                        tileDataForm.Datas[pair.Key] = point;
                    }
                    else if (tileDataForm.Datas[pair.Key] is Enum)
                    {
                        Type enumType = tileDataForm.Datas[pair.Key].GetType();
                        ListButton control = (ListButton)pair.Value;
                        tileDataForm.Datas[pair.Key] = Enum.Parse(enumType, control.List[control.CurrentIndex]);
                    }
                    else if (tileDataForm.Datas[pair.Key] is Controller)
                    {
                        ListButton control = (ListButton)pair.Value;
                        tileDataForm.Datas[pair.Key] = Entity.Controllers.Get(control.List[control.CurrentIndex]);
                    }
                    else if (tileDataForm.Datas[pair.Key] is Sprite)
                    {
                        ListButton control = (ListButton)pair.Value;
                        tileDataForm.Datas[pair.Key] = World.Resources.GetSprite(control.List[control.CurrentIndex]);
                    }
                    else if (tileDataForm.Datas[pair.Key] is Animation)
                    {
                        ListButton control = (ListButton)pair.Value;
                        tileDataForm.Datas[pair.Key] = World.Resources.GetAnimation(control.List[control.CurrentIndex]);
                    }
                }
            }

            if (tileList.SelectedItem != null)
            {
                Tile selectedTile = Tile.UnLocatedTile[tileList.SelectedItem];
                selectedTile.SubmitForm(tileDataForm);
            }
        }
        ListPresenterScreen<Animation> lastTileDataOpenAnimationScreen = null;
        ListPresenterScreen<Sprite> lastTileDataOpenSpriteScreen = null;
        string lastTileDataButtonKey = null;
        private void UpdateTileDataControls()
        {
            foreach (KeyValuePair<string, Control> pair in tileDataPanel.SubControls)
            {
                if (pair.Key.StartsWith("addAnimation_"))
                {
                    Button button = (Button)pair.Value;
                    if (button.IsPressed())
                    {
                        lastTileDataOpenAnimationScreen = new ListPresenterScreen<Animation>(Manager, World.Resources, delegate() { return World.Resources.Animations; });
                        Manager.OpenScreen(lastTileDataOpenAnimationScreen);
                        lastTileDataButtonKey = pair.Key;
                        return;
                    }
                }
                if (pair.Key.StartsWith("addSprite_"))
                {
                    Button button = (Button)pair.Value;
                    if (button.IsPressed())
                    {
                        lastTileDataOpenSpriteScreen = new ListPresenterScreen<Sprite>(Manager, World.Resources, delegate() { return World.Resources.Sprites; });
                        Manager.OpenScreen(lastTileDataOpenSpriteScreen);
                        lastTileDataButtonKey = pair.Key;
                        return;
                    }
                }
            }

            if (lastTileDataOpenAnimationScreen != null && lastTileDataOpenAnimationScreen.IsSubmitted && lastTileDataButtonKey != null)
            {
                if (lastTileDataOpenAnimationScreen.SelectedItem != null)
                {
                    ListButton listButton = (ListButton)tileDataPanel.SubControls[lastTileDataButtonKey.Split('_').Last()];
                    listButton.SelectValue(lastTileDataOpenAnimationScreen.SelectedItem);
                }
                lastTileDataOpenAnimationScreen = null;
                lastTileDataButtonKey = null;
            }
            if (lastTileDataOpenSpriteScreen != null && lastTileDataOpenSpriteScreen.IsSubmitted && lastTileDataButtonKey != null)
            {
                if (lastTileDataOpenSpriteScreen.SelectedItem != null)
                {
                    ListButton listButton = (ListButton)tileDataPanel.SubControls[lastTileDataButtonKey.Split('_').Last()];
                    listButton.SelectValue(lastTileDataOpenSpriteScreen.SelectedItem);
                }
                lastTileDataOpenSpriteScreen = null;
                lastTileDataButtonKey = null;
            }
        }

        int lastXTileSet = -1;
        int lastYTileSet = -1;
        Screen lastScreenOpen = null;
        string lastSelectedTile = null;
        bool leftButtonRealsed = true;
        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);

            if (!isInForeground)
            {
                leftButtonRealsed = false;
                return;
            }

            if (lastScreenOpen != null)
            {
                if (lastScreenOpen is AddTileScreen)
                    UpdateDatas();
                lastScreenOpen = null;
            }

            World.Update(gameTime);

            tabPanel.Update(gameTime);
            UpdateTileDataControls();

            if (Input.LeftButton.Pressed)
            {
                lastXTileSet = -1;
                lastYTileSet = -1;
            }
            if (!Input.LeftButton.Down)
                leftButtonRealsed = true;
            if (Input.LeftButton.Down && leftButtonRealsed && tileList.SelectedItem != null && tabPanel.CurrentPanel == tilePanel)
            {
                Tile selectedTile = Tile.UnLocatedTile[tileList.SelectedItem];
                int x = (Input.X - worldBound.X) / Tile.Width;
                int y = (Input.Y - worldBound.Y) / Tile.Height;
                if (worldBound.Contains(Input.X, Input.Y) && (lastXTileSet != x || lastYTileSet != y))
                    World.SetTile(x, y, selectedTile);
                lastXTileSet = x;
                lastYTileSet = y;
            }
            if (tileList.SelectedItem != lastSelectedTile)
            {
                lastSelectedTile = tileList.SelectedItem;
                ReconstructTileDataPanel();
            }

            if (tileAddButton.IsPressed())
            {
                lastScreenOpen = new AddTileScreen(Manager, World.Resources);
                Manager.OpenScreen(lastScreenOpen);
            }
            if (tileRemoveButton.IsPressed())
            {
                if (tileList.SelectedItem != null)
                {
                    Tile.UnLocatedTile.Remove(tileList.SelectedItem);
                    UpdateDatas();
                }
            }
            if (tileDataSubmitButton != null && tileDataForm != null && tileDataSubmitButton.IsPressed())
                SubmitTileData();
        }

        public override void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, isInForeground, spriteBatch);

            World.Draw(gameTime, spriteBatch, worldBound);

            tabPanel.Draw(gameTime, spriteBatch);
        }
    }
}
