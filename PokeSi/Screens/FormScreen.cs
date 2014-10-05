using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using PokeSi.Map;
using PokeSi.Map.Entities;
using PokeSi.Screens.Controls;
using PokeSi.Sprites;

namespace PokeSi.Screens
{
    public class FormScreen : Screen
    {
        public Form Form { get; protected set; }
        public bool IsSubmitted { get; protected set; }

        private SpriteFont font;
        private World World;

        private Button submitButton;
        private Panel panel;

        public FormScreen(ScreenManager manager, Form baseForm, World world)
            : base(manager)
        {
            OpeningTransition = new NoneTransition(Transition.Types.Opening);
            ClosingTransition = new NoneTransition(Transition.Types.Closing);

            World = world;
            Form = baseForm;
            IsSubmitted = false;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Sprite buttonSprite = World.Resources.GetSprite("button_idle");
            Sprite[] buttonSpriteTab = new Sprite[] { buttonSprite, World.Resources.GetSprite("button_over"), World.Resources.GetSprite("button_pressed") };
            font = Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
            panel = new Panel(this, new Rectangle(0, 0, Manager.Width, Manager.Height), buttonSprite);

            Rectangle nextRectangle = new Rectangle(100, 0, 160, 20);
            foreach (KeyValuePair<string, object> pair in Form.Datas)
            {
                panel.AddControl(pair.Key + "T", new TextBlock(this, new Vector2(0, nextRectangle.Y), pair.Key));
                if (pair.Value is string || pair.Value is int || pair.Value is float)
                {
                    panel.AddControl(pair.Key, new TextBox(this, nextRectangle, buttonSpriteTab));
                    ((TextBox)panel.SubControls[pair.Key]).Text = pair.Value.ToString();
                }
                else if (pair.Value is bool)
                {
                    panel.AddControl(pair.Key, new ToggleButton(this, nextRectangle, buttonSpriteTab));
                    ((ToggleButton)panel.SubControls[pair.Key]).SetState((bool)pair.Value);
                }
                else if (pair.Value is Enum)
                {
                    Type enumType = pair.Value.GetType();
                    ListButton control = new ListButton(this, nextRectangle, buttonSprite, new List<string>(Enum.GetNames(enumType)));
                    control.SelectValue(Enum.GetName(enumType, pair.Value));
                    panel.AddControl(pair.Key, control);
                }
                else if (pair.Value is Controller)
                {
                    ListButton control = new ListButton(this, nextRectangle, buttonSprite, new List<string>(Entity.Controllers.GetAll()));
                    control.SelectValue(Entity.Controllers.GetName((Controller)pair.Value));
                    panel.AddControl(pair.Key, control);
                }
                else if (pair.Value is Sprite)
                {
                    Rectangle rect = nextRectangle;
                    rect.Right -= 30;
                    ListButton control = new ListButton(this, rect, buttonSprite, new List<string>(World.Resources.Sprites.Keys));
                    control.SelectValue(World.Resources.GetName((Sprite)pair.Value));
                    panel.AddControl(pair.Key, control);
                    rect.Left = rect.Right;
                    rect.Right += 30;
                    rect.Left += 10;
                    Button button = new Button(this, rect, buttonSpriteTab);
                    button.Text = "...";
                    panel.AddControl("addSprite_" + pair.Key, button);
                }
                else if (pair.Value is Animation)
                {
                    Rectangle rect = nextRectangle;
                    rect.Right -= 30;
                    ListButton control = new ListButton(this, rect, buttonSprite, new List<string>(World.Resources.Animations.Keys));
                    control.SelectValue(World.Resources.GetName((Animation)pair.Value));
                    panel.AddControl(pair.Key, control);
                    rect.Left = rect.Right;
                    rect.Right += 30;
                    rect.Left += 10;
                    Button button = new Button(this, rect, buttonSpriteTab);
                    button.Text = "...";
                    panel.AddControl("addAnimation_" + pair.Key, button);
                }
                nextRectangle.Y += 30;
            }
            submitButton = new Button(this, nextRectangle, buttonSpriteTab);
            submitButton.Text = "Submit";
            panel.AddControl("submitButton", submitButton);
            panel.Pading = 10;

            Resize(Manager.Game.Viewport.Width, Manager.Game.Viewport.Height);
        }

        public override void Resize(int width, int height)
        {
            base.Resize(width, height);

            int x = (int)(Manager.Width / 2f - panel.PreferableWidth / 2f);
            int y = (int)(Manager.Height / 2f - panel.PreferableHeight / 2f);
            panel.Bound = new Rectangle(x, y, panel.PreferableWidth, panel.PreferableHeight);
        }

        private string lastButtonKey = null;
        private ListPresenterScreen<Animation> lastAnimSelectScreen = null;
        private ListPresenterScreen<Sprite> lastSpriteSelectScreen = null;
        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);

            if (!isInForeground)
                return;

            panel.Update(gameTime);

            foreach (KeyValuePair<string, Control> pair in panel.SubControls)
            {
                if (pair.Key.StartsWith("addAnimation_"))
                {
                    Button button = (Button)pair.Value;
                    if (button.IsPressed())
                    {
                        lastAnimSelectScreen = new ListPresenterScreen<Animation>(Manager, World.Resources, delegate() { return World.Resources.Animations; });
                        Manager.OpenScreen(lastAnimSelectScreen);
                        lastButtonKey = pair.Key;
                        return;
                    }
                }
                if (pair.Key.StartsWith("addSprite_"))
                {
                    Button button = (Button)pair.Value;
                    if (button.IsPressed())
                    {
                        lastSpriteSelectScreen = new ListPresenterScreen<Sprite>(Manager, World.Resources, delegate() { return World.Resources.Sprites; });
                        Manager.OpenScreen(lastSpriteSelectScreen);
                        lastButtonKey = pair.Key;
                        return;
                    }
                }
            }

            if (lastAnimSelectScreen != null && lastAnimSelectScreen.IsSubmitted && lastButtonKey != null)
            {
                if (lastAnimSelectScreen.SelectedItem != null)
                {
                    ListButton listButton = (ListButton)panel.SubControls[lastButtonKey.Split('_').Last()];
                    listButton.SelectValue(lastAnimSelectScreen.SelectedItem);
                }
                lastAnimSelectScreen = null;
                lastButtonKey = null;
            }
            if (lastSpriteSelectScreen != null && lastSpriteSelectScreen.IsSubmitted && lastButtonKey != null)
            {
                if (lastSpriteSelectScreen.SelectedItem != null)
                {
                    ListButton listButton = (ListButton)panel.SubControls[lastButtonKey.Split('_').Last()];
                    listButton.SelectValue(lastSpriteSelectScreen.SelectedItem);
                }
                lastSpriteSelectScreen = null;
                lastButtonKey = null;
            }

            if (submitButton.IsPressed())
            {
                foreach (KeyValuePair<string, Control> pair in panel.SubControls)
                {
                    if (Form.Datas.ContainsKey(pair.Key))
                    {
                        if (Form.Datas[pair.Key] is string)
                            Form.Datas[pair.Key] = ((TextBox)pair.Value).Text;
                        else if (Form.Datas[pair.Key] is int)
                            Form.Datas[pair.Key] = int.Parse(((TextBox)pair.Value).Text);
                        else if (Form.Datas[pair.Key] is float)
                            Form.Datas[pair.Key] = float.Parse(((TextBox)pair.Value).Text);
                        else if (Form.Datas[pair.Key] is Enum)
                        {
                            Type enumType = Form.Datas[pair.Key].GetType();
                            ListButton control = (ListButton)pair.Value;
                            Form.Datas[pair.Key] = Enum.Parse(enumType, control.List[control.CurrentIndex]);
                        }
                        else if (Form.Datas[pair.Key] is Controller)
                        {
                            ListButton control = (ListButton)pair.Value;
                            Form.Datas[pair.Key] = Entity.Controllers.Get(control.List[control.CurrentIndex]);
                        }
                        else if (Form.Datas[pair.Key] is Sprite)
                        {
                            ListButton control = (ListButton)pair.Value;
                            Form.Datas[pair.Key] = World.Resources.GetSprite(control.List[control.CurrentIndex]);
                        }
                        else if (Form.Datas[pair.Key] is Animation)
                        {
                            ListButton control = (ListButton)pair.Value;
                            Form.Datas[pair.Key] = World.Resources.GetAnimation(control.List[control.CurrentIndex]);
                        }
                    }
                }
                IsSubmitted = true;
                Manager.CloseScreen();
            }
        }

        public override void Draw(GameTime gameTime, bool isInForeground, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, isInForeground, spriteBatch);

            panel.Draw(gameTime, spriteBatch);
        }
    }
}
