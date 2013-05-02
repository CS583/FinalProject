using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace _3D_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Instructions : Microsoft.Xna.Framework.DrawableGameComponent
    {
        string Title = "Instructions";
        string ReturnToMenu = "Press Enter to return to the menu";
        LinkedList<string> controls = new LinkedList<string>();
        SpriteFont spriteFont;
        SpriteFont secondarySpriteFont;
        SpriteBatch spriteBatch;
        Game1.GameState currentGameState;

        protected override void LoadContent()
        {

            //Laod fonts
            spriteFont = Game.Content.Load<SpriteFont>(@"fonts\SplashScreenFontLarge");
            secondarySpriteFont = Game.Content.Load<SpriteFont>(@"fonts\SplashScreenFont");

            //Create sprite batch
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.LoadContent();
        }

        public Instructions(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            controls.AddFirst("Left-Click to fire bullets");
            controls.AddLast("Right-Click to fire missilse (limited capacity)");
            controls.AddLast("Don't let any enemies through!");
            controls.AddLast("Shoot Powerups to regain health and missiles");


            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //Did the player hi Enter Key
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                ((Game1)Game).ChangeGameState(Game1.GameState.MENU, 0);

            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, Title,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - spriteFont.MeasureString(Title).X / 2 - 10,
                        20),
                        Color.Gold);

            int y_val = 100;
            foreach (string current in controls)
            {
                spriteBatch.DrawString(secondarySpriteFont, current,
                    new Vector2(50,
                        y_val),
                        Color.Green);
                y_val += 50;
            }

            spriteBatch.DrawString(spriteFont, ReturnToMenu,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - spriteFont.MeasureString(ReturnToMenu).X / 2 - 10,
                        Game.Window.ClientBounds.Height - 250),
                        Color.Gold);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
