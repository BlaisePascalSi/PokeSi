﻿using System;
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

        private Panel entityPanel;
        private ListButton entityListButton;
        private Button entityAddButton;
        private Form entityDataForm;
        private Panel entityDataPanel;
        private Button entityDataSubmitButton;
        private Button entityRemoveButton;

        private Panel locTilePanel;
        private ListButton locTileListButton;
        private Button locTileAddButton;
        private Form locTileDataForm;
        private Panel locTileDataPanel;
        private Button locTileDataSubmitButton;
        private Button locTileRemoveButton;

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

            entityPanel = new Panel(this, Rectangle.Empty, buttonSprite) { Pading = 5 };
            TextBlock entityListButtonLabel = new TextBlock(this, Vector2.Zero, "Type");
            entityPanel.AddControl("entityListButtonLabel", entityListButtonLabel);
            entityListButton = new ListButton(this, new Rectangle(50, 0, 100, 25), buttonSprite, new List<string>(Entity.GetEntityTypes()));
            entityPanel.AddControl("entityListButton", entityListButton);
            entityAddButton = new Button(this, new Rectangle(5, 30, 40, 25), buttonSpriteTab) { Text = "Add" };
            entityPanel.AddControl("entityAddButton", entityAddButton);
            entityDataPanel = new Panel(this, Rectangle.Empty, null);
            entityPanel.AddControl("entityDataPanel", entityDataPanel);
            tabPanel.AddPanel("Entity", entityPanel, 52);

            locTilePanel = new Panel(this, Rectangle.Empty, buttonSprite) { Pading = 5 };
            TextBlock locTileListButtonLabel = new TextBlock(this, Vector2.Zero, "Type");
            locTilePanel.AddControl("locTileListButtonLabel", locTileListButtonLabel);
            locTileListButton = new ListButton(this, new Rectangle(50, 0, 100, 25), buttonSprite, new List<string>(Tile.GetLocTileTypes()));
            locTilePanel.AddControl("locTileListButton", locTileListButton);
            locTileAddButton = new Button(this, new Rectangle(5, 30, 40, 25), buttonSpriteTab) { Text = "Add" };
            locTilePanel.AddControl("locTileAddButton", locTileAddButton);
            locTileDataPanel = new Panel(this, Rectangle.Empty, null);
            locTilePanel.AddControl("locTileDataPanel", locTileDataPanel);
            tabPanel.AddPanel("LocTile", locTilePanel, 65);

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

            worldBound = new Rectangle(0, 0, (viewport.Width - 300) / Tile.Width * Tile.Width, viewport.Height);

            tabPanel.Bound = new Rectangle(worldBound.Right, 0, viewport.Width - worldBound.Right, viewport.Height);

            tileList.Bound = new Rectangle(0, 0, tilePanel.Bound.Width - tilePanel.Pading * 2, tilePanel.Bound.Height / 2 - 25);
            tileAddButton.Bound = new Rectangle(0, tileList.Bound.Bottom, 40, 25);
            tileRemoveButton.Bound = new Rectangle(tileAddButton.Bound.Right, tileAddButton.Bound.Top, 75, 25);
            tileDataPanel.Bound = new Rectangle(tileAddButton.Bound.Left, tileAddButton.Bound.Bottom, tilePanel.Bound.Width - tilePanel.Pading * 2, tilePanel.Bound.Height - tilePanel.Pading * 2 - tileAddButton.Bound.Bottom);

            entityDataPanel.Bound = new Rectangle(0, 0, entityPanel.Bound.Width - entityPanel.Pading * 2, entityPanel.Bound.Height - entityPanel.Pading * 2);

            locTileDataPanel.Bound = new Rectangle(0, 0, locTilePanel.Bound.Width - locTilePanel.Pading * 2, locTilePanel.Bound.Height - locTilePanel.Pading * 2);
        }

        private void UpdateDatas()
        {
            tileList.RemoveAllFromList();
            foreach (string key in Tile.UnLocatedTile.Keys)
                tileList.AddToList(key);
        }

        #region Tile Data Edition

        private void ReconstructTileDataPanel()
        {
            tileDataSubmitButton = null;
            tileDataPanel.RemoveAllControl();
            if (tileList.SelectedItem != null)
            {
                tileDataPanel.AddControl("title", new TextBlock(this, Vector2.Zero, tileList.SelectedItem));
                Tile selectedTile = Tile.UnLocatedTile[tileList.SelectedItem];

                tileDataForm = selectedTile.GetEditingForm();

                Sprite buttonSprite = World.Resources.GetSprite("button_idle");
                Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, World.Resources.GetSprite("button_over"), World.Resources.GetSprite("button_pressed") };
                Rectangle nextRectangle = new Rectangle(120, 25, tileDataPanel.Bound.Width - 120 - 5, 25);
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
                nextRectangle.Width = 65;
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

        #endregion

        #region Entity Data Edition

        private void ReconstructEntityDataPanel()
        {
            entityDataSubmitButton = null;
            entityRemoveButton = null;
            entityDataPanel.RemoveAllControl();
            if (selectedEntity != null && selectedEntity is IEditable)
            {
                IEditable ent = (IEditable)selectedEntity;
                entityDataForm = ent.GetEditingForm();

                Sprite buttonSprite = World.Resources.GetSprite("button_idle");
                Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, World.Resources.GetSprite("button_over"), World.Resources.GetSprite("button_pressed") };

                Rectangle nextRectangle = new Rectangle(120, entityAddButton.Bound.Bottom + 5, entityDataPanel.Bound.Width - 120 - 5, 25);
                foreach (KeyValuePair<string, object> pair in entityDataForm.Datas)
                {
                    entityDataPanel.AddControl(pair.Key + "T", new TextBlock(this, new Vector2(0, nextRectangle.Y), pair.Key));
                    if (pair.Value is string || pair.Value is int || pair.Value is float)
                    {
                        entityDataPanel.AddControl(pair.Key, new TextBox(this, nextRectangle, buttonSpriteTab));
                        ((TextBox)entityDataPanel.SubControls[pair.Key]).Text = pair.Value.ToString();
                    }
                    else if (pair.Value is bool)
                    {
                        entityDataPanel.AddControl(pair.Key, new ToggleButton(this, nextRectangle, buttonSpriteTab));
                        ((ToggleButton)entityDataPanel.SubControls[pair.Key]).SetState((bool)pair.Value);
                    }
                    else if (pair.Value is Point)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Width = nextRectangle.Width / 2 - 5;
                        entityDataPanel.AddControl(pair.Key, new TextBox(this, rect, buttonSpriteTab));
                        ((TextBox)entityDataPanel.SubControls[pair.Key]).Text = ((Point)(pair.Value)).X.ToString();
                        rect.X += rect.Width + 10;
                        entityDataPanel.AddControl(pair.Key + "_Y", new TextBox(this, rect, buttonSpriteTab));
                        ((TextBox)entityDataPanel.SubControls[pair.Key + "_Y"]).Text = ((Point)(pair.Value)).Y.ToString();
                    }
                    else if (pair.Value is Enum)
                    {
                        Type enumType = pair.Value.GetType();
                        ListButton control = new ListButton(this, nextRectangle, buttonSprite, new List<string>(Enum.GetNames(enumType)));
                        control.SelectValue(Enum.GetName(enumType, pair.Value));
                        entityDataPanel.AddControl(pair.Key, control);
                    }
                    else if (pair.Value is Controller)
                    {
                        ListButton control = new ListButton(this, nextRectangle, buttonSprite, new List<string>(Entity.Controllers.GetAll()));
                        control.SelectValue(Entity.Controllers.GetName((Controller)pair.Value));
                        entityDataPanel.AddControl(pair.Key, control);
                    }
                    else if (pair.Value is Sprite)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Right -= 30;
                        ListButton control = new ListButton(this, rect, buttonSprite, new List<string>(World.Resources.Sprites.Keys));
                        control.SelectValue(World.Resources.GetName((Sprite)pair.Value));
                        entityDataPanel.AddControl(pair.Key, control);
                        rect.Left = rect.Right;
                        rect.Right += 30;
                        rect.Left += 10;
                        Button button = new Button(this, rect, buttonSpriteTab);
                        button.Text = "...";
                        entityDataPanel.AddControl("addSprite_" + pair.Key, button);
                    }
                    else if (pair.Value is Animation)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Right -= 30;
                        ListButton control = new ListButton(this, rect, buttonSprite, new List<string>(World.Resources.Animations.Keys));
                        control.SelectValue(World.Resources.GetName((Animation)pair.Value));
                        entityDataPanel.AddControl(pair.Key, control);
                        rect.Left = rect.Right;
                        rect.Right += 30;
                        rect.Left += 10;
                        Button button = new Button(this, rect, buttonSpriteTab);
                        button.Text = "...";
                        entityDataPanel.AddControl("addAnimation_" + pair.Key, button);
                    }
                    nextRectangle.Y += 30;
                }
                nextRectangle.Width = 65;
                entityDataSubmitButton = new Button(this, nextRectangle, buttonSpriteTab);
                entityDataSubmitButton.Text = "Submit";
                entityDataPanel.AddControl("submitButton", entityDataSubmitButton);
                nextRectangle.X = 0;
                nextRectangle.Width = 75;
                entityRemoveButton = new Button(this, nextRectangle, buttonSpriteTab) { Text = "Remove" };
                entityDataPanel.AddControl("entityRemoveButton", entityRemoveButton);
            }
        }
        private void SubmitEntityData()
        {
            foreach (KeyValuePair<string, Control> pair in entityDataPanel.SubControls)
            {
                if (entityDataForm.Datas.ContainsKey(pair.Key))
                {
                    if (entityDataForm.Datas[pair.Key] is string)
                        entityDataForm.Datas[pair.Key] = ((TextBox)pair.Value).Text;
                    else if (entityDataForm.Datas[pair.Key] is int)
                        entityDataForm.Datas[pair.Key] = int.Parse(((TextBox)pair.Value).Text);
                    else if (entityDataForm.Datas[pair.Key] is float)
                        entityDataForm.Datas[pair.Key] = float.Parse(((TextBox)pair.Value).Text);
                    else if (entityDataForm.Datas[pair.Key] is Point)
                    {
                        Point point = new Point();
                        point.X = int.Parse(((TextBox)pair.Value).Text);
                        point.Y = int.Parse(((TextBox)entityDataPanel.SubControls[pair.Key + "_Y"]).Text);
                        entityDataForm.Datas[pair.Key] = point;
                    }
                    else if (entityDataForm.Datas[pair.Key] is Enum)
                    {
                        Type enumType = entityDataForm.Datas[pair.Key].GetType();
                        ListButton control = (ListButton)pair.Value;
                        entityDataForm.Datas[pair.Key] = Enum.Parse(enumType, control.List[control.CurrentIndex]);
                    }
                    else if (entityDataForm.Datas[pair.Key] is Controller)
                    {
                        ListButton control = (ListButton)pair.Value;
                        entityDataForm.Datas[pair.Key] = Entity.Controllers.Get(control.List[control.CurrentIndex]);
                    }
                    else if (entityDataForm.Datas[pair.Key] is Sprite)
                    {
                        ListButton control = (ListButton)pair.Value;
                        entityDataForm.Datas[pair.Key] = World.Resources.GetSprite(control.List[control.CurrentIndex]);
                    }
                    else if (entityDataForm.Datas[pair.Key] is Animation)
                    {
                        ListButton control = (ListButton)pair.Value;
                        entityDataForm.Datas[pair.Key] = World.Resources.GetAnimation(control.List[control.CurrentIndex]);
                    }
                }
            }

            if (selectedEntity != null && selectedEntity is IEditable)
            {
                IEditable ent = (IEditable)selectedEntity;
                ent.SubmitForm(entityDataForm);
            }
        }
        ListPresenterScreen<Animation> lastEntityDataOpenAnimationScreen = null;
        ListPresenterScreen<Sprite> lastEntityDataOpenSpriteScreen = null;
        string lastEntityDataButtonKey = null;
        private void UpdateEntityDataControls()
        {
            foreach (KeyValuePair<string, Control> pair in entityDataPanel.SubControls)
            {
                if (pair.Key.StartsWith("addAnimation_"))
                {
                    Button button = (Button)pair.Value;
                    if (button.IsPressed())
                    {
                        lastEntityDataOpenAnimationScreen = new ListPresenterScreen<Animation>(Manager, World.Resources, delegate() { return World.Resources.Animations; });
                        Manager.OpenScreen(lastEntityDataOpenAnimationScreen);
                        lastEntityDataButtonKey = pair.Key;
                        return;
                    }
                }
                if (pair.Key.StartsWith("addSprite_"))
                {
                    Button button = (Button)pair.Value;
                    if (button.IsPressed())
                    {
                        lastEntityDataOpenSpriteScreen = new ListPresenterScreen<Sprite>(Manager, World.Resources, delegate() { return World.Resources.Sprites; });
                        Manager.OpenScreen(lastEntityDataOpenSpriteScreen);
                        lastEntityDataButtonKey = pair.Key;
                        return;
                    }
                }
            }

            if (lastEntityDataOpenAnimationScreen != null && lastEntityDataOpenAnimationScreen.IsSubmitted && lastEntityDataButtonKey != null)
            {
                if (lastEntityDataOpenAnimationScreen.SelectedItem != null)
                {
                    ListButton listButton = (ListButton)entityDataPanel.SubControls[lastEntityDataButtonKey.Split('_').Last()];
                    listButton.SelectValue(lastEntityDataOpenAnimationScreen.SelectedItem);
                }
                lastEntityDataOpenAnimationScreen = null;
                lastEntityDataButtonKey = null;
            }
            if (lastEntityDataOpenSpriteScreen != null && lastEntityDataOpenSpriteScreen.IsSubmitted && lastEntityDataButtonKey != null)
            {
                if (lastEntityDataOpenSpriteScreen.SelectedItem != null)
                {
                    ListButton listButton = (ListButton)entityDataPanel.SubControls[lastEntityDataButtonKey.Split('_').Last()];
                    listButton.SelectValue(lastEntityDataOpenSpriteScreen.SelectedItem);
                }
                lastEntityDataOpenSpriteScreen = null;
                lastEntityDataButtonKey = null;
            }
        }

        #endregion

        #region LocTile Data Edition

        private void ReconstructLocTileDataPanel()
        {
            locTileDataSubmitButton = null;
            locTileRemoveButton = null;
            locTileDataPanel.RemoveAllControl();
            if (selectedLocTile != null && selectedLocTile is IEditable)
            {
                IEditable ent = (IEditable)selectedLocTile;
                locTileDataForm = ent.GetEditingForm();

                Sprite buttonSprite = World.Resources.GetSprite("button_idle");
                Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, World.Resources.GetSprite("button_over"), World.Resources.GetSprite("button_pressed") };

                Rectangle nextRectangle = new Rectangle(120, locTileAddButton.Bound.Bottom + 5, locTileDataPanel.Bound.Width - 120 - 5, 25);
                foreach (KeyValuePair<string, object> pair in locTileDataForm.Datas)
                {
                    locTileDataPanel.AddControl(pair.Key + "T", new TextBlock(this, new Vector2(0, nextRectangle.Y), pair.Key));
                    if (pair.Value is string || pair.Value is int || pair.Value is float)
                    {
                        locTileDataPanel.AddControl(pair.Key, new TextBox(this, nextRectangle, buttonSpriteTab));
                        ((TextBox)locTileDataPanel.SubControls[pair.Key]).Text = pair.Value.ToString();
                    }
                    else if (pair.Value is bool)
                    {
                        locTileDataPanel.AddControl(pair.Key, new ToggleButton(this, nextRectangle, buttonSpriteTab));
                        ((ToggleButton)locTileDataPanel.SubControls[pair.Key]).SetState((bool)pair.Value);
                    }
                    else if (pair.Value is Point)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Width = nextRectangle.Width / 2 - 5;
                        locTileDataPanel.AddControl(pair.Key, new TextBox(this, rect, buttonSpriteTab));
                        ((TextBox)locTileDataPanel.SubControls[pair.Key]).Text = ((Point)(pair.Value)).X.ToString();
                        rect.X += rect.Width + 10;
                        locTileDataPanel.AddControl(pair.Key + "_Y", new TextBox(this, rect, buttonSpriteTab));
                        ((TextBox)locTileDataPanel.SubControls[pair.Key + "_Y"]).Text = ((Point)(pair.Value)).Y.ToString();
                    }
                    else if (pair.Value is Enum)
                    {
                        Type enumType = pair.Value.GetType();
                        ListButton control = new ListButton(this, nextRectangle, buttonSprite, new List<string>(Enum.GetNames(enumType)));
                        control.SelectValue(Enum.GetName(enumType, pair.Value));
                        locTileDataPanel.AddControl(pair.Key, control);
                    }
                    else if (pair.Value is Controller)
                    {
                        ListButton control = new ListButton(this, nextRectangle, buttonSprite, new List<string>(Entity.Controllers.GetAll()));
                        control.SelectValue(Entity.Controllers.GetName((Controller)pair.Value));
                        locTileDataPanel.AddControl(pair.Key, control);
                    }
                    else if (pair.Value is Sprite)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Right -= 30;
                        ListButton control = new ListButton(this, rect, buttonSprite, new List<string>(World.Resources.Sprites.Keys));
                        control.SelectValue(World.Resources.GetName((Sprite)pair.Value));
                        locTileDataPanel.AddControl(pair.Key, control);
                        rect.Left = rect.Right;
                        rect.Right += 30;
                        rect.Left += 10;
                        Button button = new Button(this, rect, buttonSpriteTab);
                        button.Text = "...";
                        locTileDataPanel.AddControl("addSprite_" + pair.Key, button);
                    }
                    else if (pair.Value is Animation)
                    {
                        Rectangle rect = nextRectangle;
                        rect.Right -= 30;
                        ListButton control = new ListButton(this, rect, buttonSprite, new List<string>(World.Resources.Animations.Keys));
                        control.SelectValue(World.Resources.GetName((Animation)pair.Value));
                        locTileDataPanel.AddControl(pair.Key, control);
                        rect.Left = rect.Right;
                        rect.Right += 30;
                        rect.Left += 10;
                        Button button = new Button(this, rect, buttonSpriteTab);
                        button.Text = "...";
                        locTileDataPanel.AddControl("addAnimation_" + pair.Key, button);
                    }
                    nextRectangle.Y += 30;
                }
                nextRectangle.Width = 65;
                locTileDataSubmitButton = new Button(this, nextRectangle, buttonSpriteTab);
                locTileDataSubmitButton.Text = "Submit";
                locTileDataPanel.AddControl("submitButton", locTileDataSubmitButton);
                nextRectangle.X = 0;
                nextRectangle.Width = 75;
                locTileRemoveButton = new Button(this, nextRectangle, buttonSpriteTab) { Text = "Remove" };
                locTileDataPanel.AddControl("locTileRemoveButton", locTileRemoveButton);
            }
        }
        private void SubmitLocTileData()
        {
            foreach (KeyValuePair<string, Control> pair in locTileDataPanel.SubControls)
            {
                if (locTileDataForm.Datas.ContainsKey(pair.Key))
                {
                    if (locTileDataForm.Datas[pair.Key] is string)
                        locTileDataForm.Datas[pair.Key] = ((TextBox)pair.Value).Text;
                    else if (locTileDataForm.Datas[pair.Key] is int)
                        locTileDataForm.Datas[pair.Key] = int.Parse(((TextBox)pair.Value).Text);
                    else if (locTileDataForm.Datas[pair.Key] is float)
                        locTileDataForm.Datas[pair.Key] = float.Parse(((TextBox)pair.Value).Text);
                    else if (locTileDataForm.Datas[pair.Key] is Point)
                    {
                        Point point = new Point();
                        point.X = int.Parse(((TextBox)pair.Value).Text);
                        point.Y = int.Parse(((TextBox)locTileDataPanel.SubControls[pair.Key + "_Y"]).Text);
                        locTileDataForm.Datas[pair.Key] = point;
                    }
                    else if (locTileDataForm.Datas[pair.Key] is Enum)
                    {
                        Type enumType = locTileDataForm.Datas[pair.Key].GetType();
                        ListButton control = (ListButton)pair.Value;
                        locTileDataForm.Datas[pair.Key] = Enum.Parse(enumType, control.List[control.CurrentIndex]);
                    }
                    else if (locTileDataForm.Datas[pair.Key] is Controller)
                    {
                        ListButton control = (ListButton)pair.Value;
                        locTileDataForm.Datas[pair.Key] = Entity.Controllers.Get(control.List[control.CurrentIndex]);
                    }
                    else if (locTileDataForm.Datas[pair.Key] is Sprite)
                    {
                        ListButton control = (ListButton)pair.Value;
                        locTileDataForm.Datas[pair.Key] = World.Resources.GetSprite(control.List[control.CurrentIndex]);
                    }
                    else if (locTileDataForm.Datas[pair.Key] is Animation)
                    {
                        ListButton control = (ListButton)pair.Value;
                        locTileDataForm.Datas[pair.Key] = World.Resources.GetAnimation(control.List[control.CurrentIndex]);
                    }
                }
            }

            if (selectedLocTile != null && selectedLocTile is IEditable)
            {
                IEditable locTile = (IEditable)selectedLocTile;
                locTile.SubmitForm(locTileDataForm);
            }
        }
        ListPresenterScreen<Animation> lastLocTileDataOpenAnimationScreen = null;
        ListPresenterScreen<Sprite> lastLocTileDataOpenSpriteScreen = null;
        string lastLocTileDataButtonKey = null;
        private void UpdateLocTileDataControls()
        {
            foreach (KeyValuePair<string, Control> pair in locTileDataPanel.SubControls)
            {
                if (pair.Key.StartsWith("addAnimation_"))
                {
                    Button button = (Button)pair.Value;
                    if (button.IsPressed())
                    {
                        lastLocTileDataOpenAnimationScreen = new ListPresenterScreen<Animation>(Manager, World.Resources, delegate() { return World.Resources.Animations; });
                        Manager.OpenScreen(lastLocTileDataOpenAnimationScreen);
                        lastLocTileDataButtonKey = pair.Key;
                        return;
                    }
                }
                if (pair.Key.StartsWith("addSprite_"))
                {
                    Button button = (Button)pair.Value;
                    if (button.IsPressed())
                    {
                        lastLocTileDataOpenSpriteScreen = new ListPresenterScreen<Sprite>(Manager, World.Resources, delegate() { return World.Resources.Sprites; });
                        Manager.OpenScreen(lastLocTileDataOpenSpriteScreen);
                        lastLocTileDataButtonKey = pair.Key;
                        return;
                    }
                }
            }

            if (lastLocTileDataOpenAnimationScreen != null && lastLocTileDataOpenAnimationScreen.IsSubmitted && lastLocTileDataButtonKey != null)
            {
                if (lastLocTileDataOpenAnimationScreen.SelectedItem != null)
                {
                    ListButton listButton = (ListButton)locTileDataPanel.SubControls[lastLocTileDataButtonKey.Split('_').Last()];
                    listButton.SelectValue(lastLocTileDataOpenAnimationScreen.SelectedItem);
                }
                lastLocTileDataOpenAnimationScreen = null;
                lastLocTileDataButtonKey = null;
            }
            if (lastLocTileDataOpenSpriteScreen != null && lastLocTileDataOpenSpriteScreen.IsSubmitted && lastLocTileDataButtonKey != null)
            {
                if (lastLocTileDataOpenSpriteScreen.SelectedItem != null)
                {
                    ListButton listButton = (ListButton)locTileDataPanel.SubControls[lastLocTileDataButtonKey.Split('_').Last()];
                    listButton.SelectValue(lastLocTileDataOpenSpriteScreen.SelectedItem);
                }
                lastLocTileDataOpenSpriteScreen = null;
                lastLocTileDataButtonKey = null;
            }
        }

        #endregion

        int lastXTileSet = -1;
        int lastYTileSet = -1;
        Screen lastScreenOpen = null;
        string lastSelectedTile = null;
        bool leftButtonRealsed = true;
        Entity selectedEntity = null;
        Entity entityToPlace = null;
        LocatedTile selectedLocTile = null;
        LocatedTile locTileToPlace = null;
        IMoveable dragingElement = null;
        Vector2 dragingStartPos;
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
            UpdateEntityDataControls();
            UpdateLocTileDataControls();

            if (Input.MiddleButton.Down)
            {
                World.MoveView(-Input.XDelta, -Input.YDelta);
            }

            if (!Input.LeftButton.Down)
                leftButtonRealsed = true;
            if (Input.LeftButton.Down && leftButtonRealsed)
            {
                // Set tile
                if (tileList.SelectedItem != null && tabPanel.CurrentPanel == tilePanel)
                {
                    Tile selectedTile = Tile.UnLocatedTile[tileList.SelectedItem];
                    int x = (Input.X - worldBound.X + World.View.X) / Tile.Width;
                    int y = (Input.Y - worldBound.Y + World.View.Y) / Tile.Height;
                    if (worldBound.Contains(Input.X, Input.Y) && (lastXTileSet != x || lastYTileSet != y))
                        World.SetTile(x, y, selectedTile);
                    lastXTileSet = x;
                    lastYTileSet = y;
                }
            }
            if (Input.LeftButton.Pressed)
            {
                lastXTileSet = -1;
                lastYTileSet = -1;

                if (worldBound.Contains(Input.X, Input.Y))
                {
                    // Place entity
                    if (entityToPlace != null)
                    {
                        Vector2 pos = Vector2.Zero;
                        pos.X = Input.X - worldBound.X + World.View.X;
                        pos.Y = Input.Y - worldBound.Y + World.View.Y;
                        entityToPlace.Position = pos;
                        World.Entities.Add(World.GetNextEntityId(), entityToPlace);
                        entityToPlace = null;
                    }
                    // Place LocTile
                    if (locTileToPlace != null)
                    {
                        Vector2 pos = Vector2.Zero;
                        pos.X = (Input.X - worldBound.X + World.View.X) / Tile.Width;
                        pos.Y = (Input.Y - worldBound.Y + World.View.Y) / Tile.Height;
                        World.SetTile((int)pos.X, (int)pos.Y, locTileToPlace);
                        locTileToPlace = null;
                    }
                    // Start Drag and Drop
                    if (selectedEntity != null && selectedEntity is IMoveable && selectedEntity.DestinationRect.Contains(Input.X, Input.Y))
                    {
                        dragingElement = (IMoveable)selectedEntity;
                        dragingStartPos = selectedEntity.Position;
                    }
                    else if (selectedLocTile != null && selectedLocTile is IMoveable && selectedLocTile.Bound.Contains(Input.X, Input.Y))
                    {
                        dragingElement = (IMoveable)selectedLocTile;
                        dragingStartPos = selectedLocTile.Position;
                        World.SetTile((int)selectedLocTile.X, (int)selectedLocTile.Y, null);
                    }
                    // Select IBounded
                    bool foundOne = false;
                    int x = Input.X - worldBound.X;
                    int y = Input.Y - worldBound.Y;
                    foreach (Entity ent in World.Entities.Values)
                    {
                        if (ent is IBounded)
                        {
                            IBounded bounded = (IBounded)ent;
                            if (bounded.Bound.Contains(x, y))
                            {
                                selectedEntity = ent;
                                ReconstructEntityDataPanel();
                                foundOne = true;
                                break;
                            }
                        }
                    }
                    if (!foundOne)
                    {
                        selectedEntity = null;
                        ReconstructEntityDataPanel();
                    }
                    foundOne = false;
                    Tile tile = World.GetTile((x + World.View.X) / Tile.Width, (y + World.View.Y) / Tile.Height);
                    if (tile is LocatedTile && tile is IBounded)
                    {
                        IBounded bounded = (IBounded)tile;
                        if (bounded.Bound.Contains(x, y))
                        {
                            selectedLocTile = (LocatedTile)tile;
                            ReconstructLocTileDataPanel();
                            foundOne = true;
                        }
                    }
                    if (!foundOne && selectedLocTile != dragingElement)
                    {
                        selectedLocTile = null;
                        ReconstructLocTileDataPanel();
                    }
                }
            }
            if (Input.LeftButton.Released)
            {
                if (dragingElement != null)
                {
                    if (worldBound.Contains(Input.X, Input.Y))
                    {
                        if (dragingElement is Entity)
                        {
                            dragingElement.X = Input.X - worldBound.X + World.View.X;
                            dragingElement.Y = Input.Y - worldBound.Y + World.View.Y;
                            ReconstructEntityDataPanel();
                        }
                        if (dragingElement is LocatedTile)
                        {
                            LocatedTile tile = (LocatedTile)dragingElement;
                            World.SetTile((Input.X - worldBound.X + World.View.X) / Tile.Width, (Input.Y - worldBound.Y + World.View.Y) / Tile.Height, tile);
                            ReconstructLocTileDataPanel();
                        }
                        dragingElement = null;
                    }
                    else
                    {
                        dragingElement.X = dragingStartPos.X;
                        dragingElement.Y = dragingStartPos.Y;
                        dragingElement = null;
                    }
                }
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

            if (entityAddButton.IsPressed())
            {
                Type type = this.GetType().Assembly.GetType(typeof(Entity).Namespace + "." + entityListButton.List[entityListButton.CurrentIndex]);
                if (type != null)
                    entityToPlace = (Entity)type.GetConstructor(new Type[] { typeof(World) }).Invoke(new object[] { World });
            }
            if (entityRemoveButton != null && entityRemoveButton.IsPressed() && selectedEntity != null)
            {
                IEnumerable<KeyValuePair<int, Entity>> result = World.Entities.Where(pair => pair.Value == selectedEntity);
                if (result.Count() > 0)
                    World.Entities.Remove(result.ElementAt(0).Key);
                selectedEntity = null;
                ReconstructEntityDataPanel();
            }
            if (entityDataSubmitButton != null && entityDataForm != null && entityDataSubmitButton.IsPressed())
                SubmitEntityData();

            if (locTileAddButton.IsPressed())
            {
                Type type = this.GetType().Assembly.GetType(typeof(Tile).Namespace + "." + locTileListButton.List[locTileListButton.CurrentIndex]);
                if (type != null)
                    locTileToPlace = (LocatedTile)type.GetConstructor(new Type[] { typeof(World), typeof(int), typeof(int) }).Invoke(new object[] { World, 0, 0 });
            }
            if (locTileRemoveButton != null && locTileRemoveButton.IsPressed() && selectedLocTile != null)
            {
                World.SetTile((int)selectedLocTile.X, (int)selectedLocTile.Y, null);
                selectedLocTile = null;
                ReconstructLocTileDataPanel();
            }
            if (locTileDataSubmitButton != null && locTileDataForm != null && locTileDataSubmitButton.IsPressed())
                SubmitLocTileData();
        }

        public override void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, isInForeground, spriteBatch);

            World.Draw(gameTime, spriteBatch, worldBound);

            tabPanel.Draw(gameTime, spriteBatch);

            if (selectedEntity != null && selectedEntity is IBounded)
                DrawHelper.DrawRectangle(spriteBatch, ((IBounded)selectedEntity).Bound, Color.HotPink);
            if (selectedLocTile != null && selectedLocTile is IBounded)
                DrawHelper.DrawRectangle(spriteBatch, ((IBounded)selectedLocTile).Bound, Color.LawnGreen);

            // Draw tile to place
            if (tileList.SelectedItem != null && tabPanel.CurrentPanel == tilePanel && worldBound.Contains(Input.X, Input.Y))
            {
                Tile selectedTile = Tile.UnLocatedTile[tileList.SelectedItem];
                int x = (Input.X - worldBound.X + World.View.X) / Tile.Width;
                int y = (Input.Y - worldBound.Y + World.View.Y) / Tile.Height;
                selectedTile.Draw(gameTime, spriteBatch, x, y, selectedTile.GetDestinationRect(x, y, World.View.X, World.View.Y));
            }
            // Draw entity in placement
            if (entityToPlace != null)
            {
                entityToPlace.X = Input.X - worldBound.X + World.View.X;
                entityToPlace.Y = Input.Y - worldBound.Y + World.View.Y;
                entityToPlace.Draw(gameTime, spriteBatch);
            }
            // Draw LocTile to place
            if (locTileToPlace != null && worldBound.Contains(Input.X, Input.Y))
            {
                int x = (Input.X - worldBound.X + World.View.X) / Tile.Width;
                int y = (Input.Y - worldBound.Y + World.View.Y) / Tile.Height;
                locTileToPlace.MoveTo(x, y);
                locTileToPlace.Draw(gameTime, spriteBatch, x, y, locTileToPlace.GetDestinationRect(x, y, World.View.X, World.View.Y));
            }
            // Draw draging element
            if (dragingElement != null)
            {
                if (dragingElement is Entity)
                {
                    dragingElement.X = Input.X - worldBound.X + World.View.X;
                    dragingElement.Y = Input.Y - worldBound.Y + World.View.Y;
                    ((Entity)dragingElement).Draw(gameTime, spriteBatch);
                }
                if (dragingElement is LocatedTile)
                {
                    dragingElement.X = (Input.X - worldBound.X + World.View.X) / Tile.Width;
                    dragingElement.Y = (Input.Y - worldBound.Y + World.View.Y) / Tile.Height;
                    LocatedTile tile = (LocatedTile)dragingElement;
                    tile.Draw(gameTime, spriteBatch, tile.GetDestinationRect((int)tile.X, (int)tile.Y, World.View.X, World.View.Y));
                }
            }
        }
    }
}
