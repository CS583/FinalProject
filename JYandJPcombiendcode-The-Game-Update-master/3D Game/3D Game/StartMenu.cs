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
    public class StartMenu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        String Title = "SPACE SHOOTER";
        string Begin = "Begin";
        string About = "About";
        string Instructions = "Instructions";
        string Exit = "Exit";
        string menuDirections = "(use the arrow keys to navigate the menu)";
        SpriteFont spriteFont;
        SpriteFont secondarySpriteFont;
        SpriteBatch spriteBatch;
        Game1.GameState currentGameState;
        int selection = 1;
        Color defaultColor = Color.LightBlue;
        Color currentSelection = Color.BlanchedAlmond;
        bool keyRelease;
        Texture2D menuBackground;

        protected override void LoadContent()
        {
            //Laod fonts
            spriteFont = Game.Content.Load<SpriteFont>(@"fonts\SplashScreenFontLarge");
            secondarySpriteFont = Game.Content.Load<SpriteFont>(@"fonts\SplashScreenFont");

            //Create sprite batch
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            menuBackground = Game.Content.Load<Texture2D>(@"Textures\herschel_s_swan");

            base.LoadContent();
        }

        public StartMenu(Game game)
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
                if(selection == 1)
                    ((Game1)Game).ChangeGameState(Game1.GameState.START, 0);

                if (selection == 2)
                    ((Game1)Game).ChangeGameState(Game1.GameState.ABOUT, 0);
                 
                // If we are in end game exit
                if (selection == 3)
                {
                    ((Game1)Game).ChangeGameState(Game1.GameState.INSTRUCTIONS,0);
                }

                if (selection == 4)
                {
                    Game.Exit();
                }
            }

            //Did the player hit either arrow key?
            if (keyRelease)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    selection--;
                    keyRelease = false;
                    if (selection < 1)
                        selection = 4;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    selection++;
                    keyRelease = false;
                    if (selection > 4)
                        selection = 1;
                }
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.Down))
            {
                keyRelease = true;
            }


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(menuBackground, new Rectangle(0, 0,
                   Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height),
                   null, Color.White, 0, Vector2.Zero,
                   SpriteEffects.None, 0);

            // Draw main text
            spriteBatch.DrawString(spriteFont, Title,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - spriteFont.MeasureString(Title).X / 2,
                        Game.Window.ClientBounds.Height / 2 - 150),
                        Color.Red);
            
            spriteBatch.DrawString(secondarySpriteFont, menuDirections,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - spriteFont.MeasureString(menuDirections).X / 2 + 50,
                        Game.Window.ClientBounds.Height / 2 - 100),
                        Color.Red);

            if (selection == 1)
            {
                spriteBatch.DrawString(secondarySpriteFont, Begin,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - spriteFont.MeasureString(Begin).X / 2,
                        Game.Window.ClientBounds.Height / 2 - 0),
                        currentSelection);
            }
            else
            {
                spriteBatch.DrawString(secondarySpriteFont, Begin,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - spriteFont.MeasureString(Begin).X / 2,
                        Game.Window.ClientBounds.Height / 2 - 0),
                        defaultColor);
            }

            if (selection == 2)
            {
                spriteBatch.DrawString(secondarySpriteFont, About,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - secondarySpriteFont.MeasureString(
                        About).X / 2,
                        Game.Window.ClientBounds.Height / 2 + 50),
                        currentSelection);
            }
            else
            {
                spriteBatch.DrawString(secondarySpriteFont, About,
                    new Vector2(Game.Window.ClientBounds.Width / 2
                        - secondarySpriteFont.MeasureString(
                        About).X / 2,
                        Game.Window.ClientBounds.Height / 2 + 50),
                        defaultColor);
            }

            if (selection == 3)
            {
                spriteBatch.DrawString(secondarySpriteFont, Instructions,
                new Vector2(Game.Window.ClientBounds.Width / 2
                    - secondarySpriteFont.MeasureString(
                    Instructions).X / 2,
                    Game.Window.ClientBounds.Height / 2 + 100),
                    currentSelection);
            }
            else
            {
                spriteBatch.DrawString(secondarySpriteFont, Instructions,
                new Vector2(Game.Window.ClientBounds.Width / 2
                    - secondarySpriteFont.MeasureString(
                    Instructions).X / 2,
                    Game.Window.ClientBounds.Height / 2 + 100),
                    defaultColor);
            }


            if (selection == 4)
            {
                spriteBatch.DrawString(secondarySpriteFont, Exit,
                new Vector2(Game.Window.ClientBounds.Width / 2
                    - secondarySpriteFont.MeasureString(
                    About).X / 2,
                    Game.Window.ClientBounds.Height / 2 + 150),
                    currentSelection);
            }
            else
            {
                spriteBatch.DrawString(secondarySpriteFont, Exit,
                new Vector2(Game.Window.ClientBounds.Width / 2
                    - secondarySpriteFont.MeasureString(
                    About).X / 2,
                    Game.Window.ClientBounds.Height / 2 + 150),
                    defaultColor);
            }
            
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}