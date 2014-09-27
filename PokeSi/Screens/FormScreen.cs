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

        private Button submitButton;
        private Panel panel;
        //private Dictionary<string, Control> controls;

        public FormScreen(ScreenManager manager, Form baseForm)
            : base(manager)
        {
            OpeningTransition = new NoneTransition(Transition.Types.Opening);
            ClosingTransition = new NoneTransition(Transition.Types.Closing);

            Form = baseForm;
            IsSubmitted = false;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            SpriteSheet buttonSheet = new SpriteSheet(Manager.Game, "button.png", 100, 100);
            font = Manager.Game.Content.Load<SpriteFont>("Fonts/Hud");
            panel = new Panel(this, new Rectangle(0, 0, Manager.Width, Manager.Height), buttonSheet.GetSprite(0, 0));

            Rectangle nextRectangle = new Rectangle(100, 0, 120, 20);
            foreach (KeyValuePair<string, object> pair in Form.Datas)
            {
                panel.AddControl(pair.Key + "T", new TextBlock(this, new Vector2(0, nextRectangle.Y), pair.Key));
                if (pair.Value is string || pair.Value is int || pair.Value is float)
                {
                    panel.AddControl(pair.Key, new TextBox(this, nextRectangle, buttonSheet, 0));
                    ((TextBox)panel.SubControls[pair.Key]).Text = pair.Value.ToString();
                }
                else if (pair.Value is bool)
                {
                    panel.AddControl(pair.Key, new ToggleButton(this, nextRectangle, buttonSheet, 0));
                    ((ToggleButton)panel.SubControls[pair.Key]).SetState((bool)pair.Value);
                }
                else if (pair.Value is Enum)
                {
                    Type enumType = pair.Value.GetType();
                    ListButton control = new ListButton(this, nextRectangle, buttonSheet.GetSprite(0, 0), new List<string>(Enum.GetNames(enumType)));
                    control.SelectValue(Enum.GetName(enumType, pair.Value));
                    panel.AddControl(pair.Key, control);
                }
                else if(pair.Value is Controller)
                {
                    ListButton control = new ListButton(this, nextRectangle, buttonSheet.GetSprite(0, 0), new List<string>(Entity.Controllers.GetAll()));
                    control.SelectValue(Entity.Controllers.GetName((Controller)pair.Value));
                    panel.AddControl(pair.Key, control);
                }
                nextRectangle.Y += 30;
            }
            submitButton = new Button(this, nextRectangle, new SpriteSheet(Manager.Game, "button.png", 100, 100), 0);
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

        public override void Update(GameTime gameTime, bool isInForeground)
        {
            base.Update(gameTime, isInForeground);

            if (!isInForeground)
                return;

            panel.Update(gameTime);

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
                        else if(Form.Datas[pair.Key] is Controller)
                        {
                            ListButton control = (ListButton)pair.Value;
                            Form.Datas[pair.Key] = Entity.Controllers.Get(control.List[control.CurrentIndex]);
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
