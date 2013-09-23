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
        Texture2D spaceship, puppy, cake, puppy_right, puppy_left;
        SpriteFont font;

        // Princess parameters
        int StepLength = 2;
        float jumpVelocity = -150.0f;
        float gravity = 3.0f;
        Vector2 princessPosition = Vector2.Zero;
        Vector2 princessSpeed = new Vector2(0f, 0f);

        // Spaceship parameters
        Vector2 spaceshipPosition = Vector2.Zero;
        Vector2 spaceshipSpeed = new Vector2(50.0f, 50.0f);
        float spaceshipTimeOut = 0;
        Random rand = new Random();

        // Puppy parameters
        int numberPuppys = 8; 
        int puppysPerCake = 3;
        int puppysDroppedSinceLastCake = 0;
        Vector2[] puppyPosition = new Vector2[8];
        Vector2[] puppySpeed = new Vector2[8];

        // Cake parameters
        int numberCakes = 4;
        int cakeScore = 0;
        Vector2[] cakePosition = new Vector2[4];
        Vector2[] cakeSpeed = new Vector2[4];

        //Game state conditions
        int gameState = 0;
        const int introMenu = 0, gameRunning = 1, gameOver = 2;


     

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load sprites from file
            FacingFront = Content.Load<Texture2D>("front");
            FacingLeft = Content.Load<Texture2D>("left");
            FacingRight = Content.Load<Texture2D>("right");
            spaceship = Content.Load<Texture2D>("front");
            puppy = Content.Load<Texture2D>("puppy_front");
            puppy_right = Content.Load<Texture2D>("puppy_right");
            puppy_left = Content.Load<Texture2D>("puppy_left");
            spaceship = Content.Load<Texture2D>("spaceship");
            cake = Content.Load<Texture2D>("cake");
            font = Content.Load<SpriteFont>("SpriteFont1");

            //Set default princess
            princess = FacingFront;
            princessPosition.X = 0;
            princessPosition.Y = graphics.GraphicsDevice.Viewport.Height - princess.Height;

            //Initialize puppy and cake positions
            for (int i = 0; i < numberPuppys; i++)
            {
                puppyPosition[i].X = -1.0f;
                puppyPosition[i].Y = -1.0f;
            }
            for (int i=0; i < numberCakes; i++)
            {
                cakePosition[i].X = -1.0f;
                cakePosition[i].Y = -1.0f;
            }
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Nothing to be done here
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
                    /********** Princess physics *****************************/
                    //Choose sprite
                    if (newState.IsKeyDown(Keys.Right) && !newState.IsKeyDown(Keys.Left))
                        princess = FacingRight;
                    else if (!newState.IsKeyDown(Keys.Right) && newState.IsKeyDown(Keys.Left))
                        princess = FacingLeft;
                    else
                        princess = FacingFront;
                    
                    //Set Limits
                    int MaxX = graphics.GraphicsDevice.Viewport.Width - princess.Width;
                    int MinX = 0;
                    int MaxY = graphics.GraphicsDevice.Viewport.Height - princess.Height;
                    int MinY = 0;

                    // Right Movement
                    if (newState.IsKeyDown(Keys.Right))
                    {
                        if (princessPosition.X >= MaxX)
                            princessPosition.X = MaxX;
                        else
                            princessPosition.X += StepLength;
                    }
                    // Left Movement
                    if (newState.IsKeyDown(Keys.Left))
                    {
                        if (princessPosition.X <= MinX)
                            princessPosition.X = MinX;
                        else
                            princessPosition.X -= StepLength;
                    }

                    //Jump Movement
                    if (newState.IsKeyDown(Keys.Space) && !oldState.IsKeyDown(Keys.Space))
                    {
                        if (princessPosition.Y == MaxY)
                        {
                            princessSpeed.Y = jumpVelocity;
                            princessPosition.Y = MaxY - 1;
                        }
                    }
                    if (princessPosition.Y < MaxY)
                    {
                        princessPosition.Y += princessSpeed.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        princessSpeed.Y += gravity;
                    }
                    else
                    {
                        princessPosition.Y = MaxY;
                        princessSpeed.Y = 0;
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

                        //Initiate puppy
                        if (puppysDroppedSinceLastCake < puppysPerCake)
                        {
                            for (int i = 0; i < numberPuppys; i++)
                            {
                                if (puppyPosition[i].X == -1.0f)
                                {
                                    puppyPosition[i].X = spaceshipPosition.X;
                                    puppyPosition[i].Y = spaceshipPosition.Y;
                                    puppySpeed[i] = Vector2.Zero;
                                    puppysDroppedSinceLastCake++;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < numberCakes; i++)
                            {
                                if (cakePosition[i].X == -1.0f)
                                {
                                    cakePosition[i].X = spaceshipPosition.X;
                                    cakePosition[i].Y = spaceshipPosition.Y;
                                    cakeSpeed[i] = Vector2.Zero;
                                    puppysDroppedSinceLastCake = 0; ;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        spaceshipPosition += spaceshipSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        // keep in top half of screen
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

                    /**************** Puppy physics ******************************/
                    MaxX = graphics.GraphicsDevice.Viewport.Width - puppy.Width;
                    MaxY = graphics.GraphicsDevice.Viewport.Height - puppy.Height;
                    for (int i = 0; i < numberPuppys; i++)
                    {
                        if (puppyPosition[i].X != -1.0f)
                        {
                            if (puppyPosition[i].Y >= MaxY)
                            {
                                puppySpeed[i].Y = 0;
                                puppyPosition[i].Y = MaxY;

                                if ((i % 2) == 1)
                                    puppySpeed[i].X = -40.0f;
                                else
                                    puppySpeed[i].X = 40.0f;
                            }
                            else
                            {
                                puppySpeed[i].Y += gravity;
                                puppySpeed[i].X = 0.0f;
                            }

                            puppyPosition[i] += puppySpeed[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (puppyPosition[i].X >= MaxX || puppyPosition[i].X <= MinX)
                            {
                                puppyPosition[i].X = -1.0f;
                                puppyPosition[i].Y = -1.0f;
                            }

                            if (Math.Abs(puppyPosition[i].X - princessPosition.X) <= (princess.Width/2  + puppy.Width/2 ))
                            {
                                if (Math.Abs(puppyPosition[i].Y - princessPosition.Y) <= (princess.Height/2  + puppy.Height/2 ))
                                {
                                    gameState = gameOver;

                                }
                            }
                        }
                    }
                    /**********************************************************/


                    /**************** Cake physics ******************************/
                    MaxX = graphics.GraphicsDevice.Viewport.Width - cake.Width;
                    MaxY = graphics.GraphicsDevice.Viewport.Height - cake.Height;
                    for (int i = 0; i < numberCakes; i++)
                    {
                        if (cakePosition[i].X != -1.0f)
                        {
                            if (cakePosition[i].Y >= MaxY)
                            {
                                cakeSpeed[i].Y = 0;
                                cakePosition[i].Y = MaxY;

                                if ((i % 2) == 1)
                                    cakeSpeed[i].X = -20.0f;
                                else
                                    cakeSpeed[i].X = 20.0f;
                            }
                            else
                            {
                                cakeSpeed[i].Y += gravity;
                                cakeSpeed[i].X = 0.0f;
                            }

                            cakePosition[i] += cakeSpeed[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (cakePosition[i].X >= MaxX || cakePosition[i].X <= MinX)
                            {
                                cakePosition[i].X = -1.0f;
                                cakePosition[i].Y = -1.0f;
                            }

                            if (Math.Abs(cakePosition[i].X - princessPosition.X) <= (princess.Width / 2 + cake.Width / 2))
                            {
                                if (Math.Abs(cakePosition[i].Y - princessPosition.Y) <= (princess.Height / 2 + cake.Height / 2))
                                {
                                    cakeScore++;
                                    cakePosition[i].Y = -1.0f;
                                    cakePosition[i].X = -1.0f;
                                }
                            }

                            for (int j = 0; j < numberPuppys; j++)
                            {
                                if (Math.Abs(cakePosition[i].X - puppyPosition[j].X) <= (princess.Width / 2 + cake.Width / 2))
                                {
                                    if (Math.Abs(cakePosition[i].Y - puppyPosition[j].Y) <= (princess.Height / 2 + cake.Height / 2))
                                    {
                                        cakePosition[i].Y = -1.0f;
                                        cakePosition[i].X = -1.0f;
                                    }
                                }
                            }
                        }
                    }
                    /**********************************************************/
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
                        cakeScore = 0;

                        //Check for collision with puppies and princess
                        princessPosition.X = 0;
                        princessPosition.Y = graphics.GraphicsDevice.Viewport.Height - princess.Height;

                        for (int i = 0; i < numberPuppys; i++)
                        {
                            puppyPosition[i].X = -1.0f;
                            puppyPosition[i].Y = -1.0f;
                        }
                        for (int i = 0; i < numberCakes; i++)
                        {
                            cakePosition[i].X = -1.0f;
                            cakePosition[i].Y = -1.0f;
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

                    // Draw the scene
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    spriteBatch.DrawString(font, "Your Score: " + cakeScore + " cakes", new Vector2(0, 0), Color.DarkGoldenrod);
                    spriteBatch.Draw(princess, princessPosition, Color.White);

                    spriteBatch.Draw(spaceship, spaceshipPosition, Color.White);
                    for (int i = 0; i < numberPuppys; i++)
                    {
                        if (puppyPosition[i].X != -1.0f)
                        {
                            if(puppySpeed[i].X > 0)
                                spriteBatch.Draw(puppy_right, puppyPosition[i], Color.White);
                            else if(puppySpeed[i].X < 0)
                                spriteBatch.Draw(puppy_left, puppyPosition[i], Color.White);
                            else
                                spriteBatch.Draw(puppy, puppyPosition[i], Color.White);
                        }
                    }

                    for (int i = 0; i < numberCakes; i++)
                    {
                        if (cakePosition[i].X != -1.0f)
                            spriteBatch.Draw(cake, cakePosition[i], Color.White);
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
                    spriteBatch.DrawString(font, "Whoops. You were viciously mauled by a puppy.\nYour total score is " + cakeScore+" cakes.\n Press Space To Play Again", new Vector2(0, 0), Color.White);
                    spriteBatch.End();
                    break;
            }
        }
    }
}
