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

namespace EECS481Homework2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        KeyboardState oldState;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
         
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        // This is a texture we can render.
        Texture2D princess, FacingFront, FacingRight, FacingLeft;
        Texture2D spaceship, bomb, coin, puppy_right, puppy_left;
        SpriteFont font;

        // Set the coordinates to draw the sprite at.
        Vector2 spritePosition = Vector2.Zero;
        Vector2 spriteSpeed = new Vector2(0f, 0f);

        Vector2 spaceshipPosition = Vector2.Zero;
        Vector2 spaceshipSpeed = new Vector2(50.0f, 50.0f);
        float spaceshipTimeOut = 0;

        int numberBombs = 8;
        int bombsDropped = 0;
        Vector2[] bombPosition = new Vector2[8];
        Vector2[] bombSpeed = new Vector2[8];

        int numberCoins = 4;
        int bombsPerCoin = 3;
        int bombsDroppedSinceLastCoin = 0;
        int coinScore = 0;
        Vector2[] coinPosition = new Vector2[4];
        Vector2[] coinSpeed = new Vector2[4];

        int gameState = 0;
        const int introMenu = 0, gameRunning = 1, gameOver = 2;

        Random rand = new Random();
     

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            FacingFront = Content.Load<Texture2D>("front");
            FacingLeft = Content.Load<Texture2D>("left");
            FacingRight = Content.Load<Texture2D>("right");
            spaceship = Content.Load<Texture2D>("front");
            bomb = Content.Load<Texture2D>("puppy_front");
            puppy_right = Content.Load<Texture2D>("puppy_right");
            puppy_left = Content.Load<Texture2D>("puppy_left");
            spaceship = Content.Load<Texture2D>("spaceship");
            coin = Content.Load<Texture2D>("cake");
            font = Content.Load<SpriteFont>("SpriteFont1");

            princess = FacingFront;

            spritePosition.X = 0;
            spritePosition.Y = graphics.GraphicsDevice.Viewport.Height - princess.Height;

            for (int i = 0; i < numberBombs; i++)
            {
                bombPosition[i].X = -1.0f;
                bombPosition[i].Y = -1.0f;
            }
            for (int i=0; i < numberCoins; i++)
            {
                coinPosition[i].X = -1.0f;
                coinPosition[i].Y = -1.0f;
            }
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            switch (gameState)
            {
                case (gameRunning):
                    /********** Princess position *****************************/
                    //Choose sprite
                    if (newState.IsKeyDown(Keys.Right) && !newState.IsKeyDown(Keys.Left))
                    {
                        princess = FacingRight;
                    }
                    else if (!newState.IsKeyDown(Keys.Right) && newState.IsKeyDown(Keys.Left))
                    {
                        princess = FacingLeft;
                    }
                    else
                    {
                        princess = FacingFront;
                    }

                    //Set Limits
                    int MaxX = graphics.GraphicsDevice.Viewport.Width - princess.Width;
                    int MinX = 0;
                    int MaxY = graphics.GraphicsDevice.Viewport.Height - princess.Height;
                    int MinY = 0;

                    int StepLength = 2;
                    float jumpVelocity = -150.0f;
                    float gravity = 3.0f;



                    // Right Movement
                    if (newState.IsKeyDown(Keys.Right))
                    {
                        if (spritePosition.X >= MaxX)
                        {
                            spritePosition.X = MaxX;
                        }
                        else
                        {
                            spritePosition.X += StepLength;
                        }
                    }
                    // Left Movement
                    if (newState.IsKeyDown(Keys.Left))
                    {
                        if (spritePosition.X <= MinX)
                        {
                            spritePosition.X = MinX;
                        }
                        else
                        {
                            spritePosition.X -= StepLength;
                        }
                    }

                    //Jump Movement
                    if (newState.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space))
                    {
                        if (spritePosition.Y == MaxY)
                        {
                            spriteSpeed.Y = jumpVelocity;
                            spritePosition.Y = MaxY - 1;
                        }
                    }
                    if (spritePosition.Y < MaxY)
                    {
                        spritePosition.Y += spriteSpeed.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        spriteSpeed.Y += gravity;
                    }
                    else
                    {
                        spritePosition.Y = MaxY;
                        spriteSpeed.Y = 0;
                    }
                    /*************************************************************/

                    /*************** Space ship physics ***************************/
                    MaxX = graphics.GraphicsDevice.Viewport.Width - spaceship.Width;
                    MaxY = graphics.GraphicsDevice.Viewport.Height / 2 - spaceship.Height;

                    spaceshipTimeOut -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (spaceshipTimeOut <= 0)
                    {
                        spaceshipSpeed.X = (float)(rand.Next(0, 100) - 50);
                        spaceshipSpeed.Y = (float)(rand.Next(0, 100) - 50);
                        spaceshipTimeOut = 1.5f;

                        //Initiate bomb
                        if (bombsDroppedSinceLastCoin < bombsPerCoin)
                        {
                            for (int i = 0; i < numberBombs; i++)
                            {
                                if (bombPosition[i].X == -1.0f)
                                {
                                    bombPosition[i].X = spaceshipPosition.X;
                                    bombPosition[i].Y = spaceshipPosition.Y;
                                    bombSpeed[i] = Vector2.Zero;
                                    bombsDroppedSinceLastCoin++;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < numberCoins; i++)
                            {
                                if (coinPosition[i].X == -1.0f)
                                {
                                    coinPosition[i].X = spaceshipPosition.X;
                                    coinPosition[i].Y = spaceshipPosition.Y;
                                    coinSpeed[i] = Vector2.Zero;
                                    bombsDroppedSinceLastCoin = 0; ;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        spaceshipPosition += spaceshipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // Check for bounce.
                        if (spaceshipPosition.X > MaxX)
                        {
                            spaceshipSpeed.X *= -1;
                            spaceshipPosition.X = MaxX;
                        }

                        else if (spaceshipPosition.X < MinX)
                        {
                            spaceshipSpeed.X *= -1;
                            spaceshipPosition.X = MinX;
                        }

                        if (spaceshipPosition.Y > MaxY)
                        {
                            spaceshipSpeed.Y *= -1;
                            spaceshipPosition.Y = MaxY;
                        }

                        else if (spaceshipPosition.Y < MinY)
                        {
                            spaceshipSpeed.Y *= -1;
                            spaceshipPosition.Y = MinY;
                        }
                    }
                    /************************************************************/

                    /**************** Bomb physics ******************************/
                    MaxX = graphics.GraphicsDevice.Viewport.Width - bomb.Width;
                    MaxY = graphics.GraphicsDevice.Viewport.Height - bomb.Height;
                    for (int i = 0; i < numberBombs; i++)
                    {
                        if (bombPosition[i].X != -1.0f)
                        {
                            if (bombPosition[i].Y >= MaxY)
                            {
                                bombSpeed[i].Y = 0;
                                bombPosition[i].Y = MaxY;

                                if ((i % 2) == 1)
                                    bombSpeed[i].X = -40.0f;
                                else
                                    bombSpeed[i].X = 40.0f;
                            }
                            else
                            {
                                bombSpeed[i].Y += gravity;
                                bombSpeed[i].X = 0.0f;
                            }

                            bombPosition[i] += bombSpeed[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (bombPosition[i].X >= MaxX || bombPosition[i].X <= MinX)
                            {
                                bombPosition[i].X = -1.0f;
                                bombPosition[i].Y = -1.0f;
                            }

                            if (Math.Abs(bombPosition[i].X - spritePosition.X) <= (princess.Width/2  + bomb.Width/2 ))
                            {
                                if (Math.Abs(bombPosition[i].Y - spritePosition.Y) <= (princess.Height/2  + bomb.Height/2 ))
                                {
                                    gameState = gameOver;

                                }
                            }
                        }
                    }
                    /**********************************************************/


                    /**************** Coin physics ******************************/
                    MaxX = graphics.GraphicsDevice.Viewport.Width - coin.Width;
                    MaxY = graphics.GraphicsDevice.Viewport.Height - coin.Height;
                    for (int i = 0; i < numberCoins; i++)
                    {
                        if (coinPosition[i].X != -1.0f)
                        {
                            if (coinPosition[i].Y >= MaxY)
                            {
                                coinSpeed[i].Y = 0;
                                coinPosition[i].Y = MaxY;

                                if ((i % 2) == 1)
                                    coinSpeed[i].X = -20.0f;
                                else
                                    coinSpeed[i].X = 20.0f;
                            }
                            else
                            {
                                coinSpeed[i].Y += gravity;
                                coinSpeed[i].X = 0.0f;
                            }

                            coinPosition[i] += coinSpeed[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (coinPosition[i].X >= MaxX || coinPosition[i].X <= MinX)
                            {
                                coinPosition[i].X = -1.0f;
                                coinPosition[i].Y = -1.0f;
                            }

                            if (Math.Abs(coinPosition[i].X - spritePosition.X) <= (princess.Width / 2 + coin.Width / 2))
                            {
                                if (Math.Abs(coinPosition[i].Y - spritePosition.Y) <= (princess.Height / 2 + coin.Height / 2))
                                {
                                    coinScore++;
                                    coinPosition[i].Y = -1.0f;
                                    coinPosition[i].X = -1.0f;
                                }
                            }

                            for (int j = 0; j < numberBombs; j++)
                            {
                                if (Math.Abs(coinPosition[i].X - bombPosition[j].X) <= (princess.Width / 2 + coin.Width / 2))
                                {
                                    if (Math.Abs(coinPosition[i].Y - bombPosition[j].Y) <= (princess.Height / 2 + coin.Height / 2))
                                    {
                                        coinPosition[i].Y = -1.0f;
                                        coinPosition[i].X = -1.0f;
                                    }
                                }
                            }
                        }
                    }
                    /**********************************************************/


                    //Platform physics

                    
                    break;

                case (introMenu):
                    if (newState.IsKeyDown(Keys.Space))
                    {
                        gameState = gameRunning;
                    }
                    break;

                case (gameOver):
                    if (newState.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space) )
                    {
                        gameState = gameRunning;
                        princess = FacingFront;
                        coinScore = 0;

                        spritePosition.X = 0;
                        spritePosition.Y = graphics.GraphicsDevice.Viewport.Height - princess.Height;

                        for (int i = 0; i < numberBombs; i++)
                        {
                            bombPosition[i].X = -1.0f;
                            bombPosition[i].Y = -1.0f;
                        }
                        for (int i = 0; i < numberCoins; i++)
                        {
                            coinPosition[i].X = -1.0f;
                            coinPosition[i].Y = -1.0f;
                        }
                    }
                    break;
            }
            oldState = newState;
            base.Update(gameTime);
        }

        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            switch (gameState)
            {
                case (gameRunning):
                    GraphicsDevice.Clear(Color.White);

                    // Draw the sprite.
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    spriteBatch.DrawString(font, "Your Score: " + coinScore + " cakes", new Vector2(0, 0), Color.DarkGoldenrod);
                    spriteBatch.Draw(princess, spritePosition, Color.White);

                    spriteBatch.Draw(spaceship, spaceshipPosition, Color.White);
                    for (int i = 0; i < numberBombs; i++)
                    {
                        if (bombPosition[i].X != -1.0f)
                        {
                            if(bombSpeed[i].X > 0)
                                spriteBatch.Draw(puppy_right, bombPosition[i], Color.White);
                            else if(bombSpeed[i].X < 0)
                                spriteBatch.Draw(puppy_left, bombPosition[i], Color.White);
                            else
                                spriteBatch.Draw(bomb, bombPosition[i], Color.White);
                        }
                    }

                    for (int i = 0; i < numberCoins; i++)
                    {
                        if (coinPosition[i].X != -1.0f)
                            spriteBatch.Draw(coin, coinPosition[i], Color.White);
                    }

                    spriteBatch.End();
                    base.Draw(gameTime);
                    break;

                case (introMenu):
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    spriteBatch.DrawString(font, "Cake-Hungry Alien Puppy Invaders!\nCollect as much cake as you can before the puppies do!\nBut don't run into the puppies.\n\nLeft/Right to move.\nSpace to jump\n\nPress Space To Play", new Vector2(0, 0), Color.White);
                    spriteBatch.End();
                    break;

                case (gameOver):
                    GraphicsDevice.Clear(Color.Tomato);
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    spriteBatch.DrawString(font, "Whoops. You were viciously mauled by a puppy.\nYour total score is " + coinScore+" cakes.\n Press Space To Play Again", new Vector2(0, 0), Color.White);
                    spriteBatch.End();
                    break;
            }
        }
    }
}
