using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace GamesProgrammingAssignment
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private int lvlMapNumber;
        private int lvlRoomNumber;
        private bool newFloor = true;

        private int xLeft;
        private int xRight;
        private int yTop;
        private int yBot;
        private int xDoor;
        private int yDoor;
        private int xStairs;
        private int yStairs;
        private float xPlayer;
        private float yPlayer;
        private int xPlayerGrid;
        private int yPlayerGrid;

        private Texture2D lvlFloor;
        private Texture2D lvlWall;
        private Texture2D lvlDoor;
        private Texture2D lvlStairs;
        private Texture2D lvlPlayer;

        private int[] lvlMap = new int[40 * 24];

        public Game1()
            : base()
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
            lvlMapNumber = 1;
            lvlRoomNumber = 1;

            for (int x = 0; x < 960; x++)
            {
                lvlMap[x] = 0;
            }

                base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            lvlFloor = Content.Load<Texture2D>("Floor");
            lvlWall = Content.Load<Texture2D>("Wall");
            lvlDoor = Content.Load<Texture2D>("Door");
            lvlStairs = Content.Load<Texture2D>("Stairs");
            lvlPlayer = Content.Load<Texture2D>("Player");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (newFloor)
            { 
                switch (lvlMapNumber)
                {
                    //setting the values level for the first floor (i.e. no prcedural generation)
                    case 1:
                        xLeft = 10;
                        xRight = 30;
                        yTop = 3;
                        yBot = 21;
                        xDoor = 19;
                        yDoor = 3;
                        xStairs = 20;
                        yStairs = 19;
                        xPlayer = 20;
                        yPlayer = 4;
                        break;
                }
                newFloor = false;
            }

            //assigning all values to the level array
            for (int x = 0; x < 40; x++)
            {
                for (int y = 0; y < 24; y++)
                {
                    if (lvlMap[40 * y + x] == 0)
                    {
                        //assigning values for walls 
                        if ((x >= xLeft && x <= xRight && y == yTop)
                            || (x >= xLeft && x <= xRight && y == yBot)
                            || (y > yTop && y < yBot && x == xLeft)
                            || (y > yTop && y < yBot && x == xRight))
                            lvlMap[40 * y + x] = 1;

                        //assigning values for floors
                        if (x > xLeft && x < xRight && y > yTop && y < yBot)
                            lvlMap[40 * y + x] = 2;

                        //assigning values for door (if any)
                        if (x == xDoor && y == yDoor)
                        {
                            lvlMap[40 * y + x] = 3;
                            lvlMap[(40 * y + x) + 1] = -1;
                            lvlMap[(40 * y + x) + 2] = -1;
                        }

                        //assigning values for stairs
                        if (x == xStairs && y == yStairs)
                        {
                            lvlMap[40 * y + x] = 4;
                        }
                    }
                }
            }

            if (xPlayer % 1 > 0.5)
                xPlayerGrid = (int)xPlayer + 1;
            else
                xPlayerGrid = (int)xPlayer;

            if (yPlayer % 1 > 0.5)
                yPlayerGrid = (int)yPlayer + 1;
            else
                yPlayerGrid = (int)yPlayer;

            KeyboardState State = Keyboard.GetState();

            if (State.IsKeyDown(Keys.W))
            {
                if (lvlMap[40 * (yPlayerGrid - 1) + xPlayerGrid] != 1)
                    yPlayer += -0.2f;
            }

            if (State.IsKeyDown(Keys.A))
            {
                if (lvlMap[40 * yPlayerGrid + (xPlayerGrid - 1)] != 1)
                    xPlayer += -0.2f;
            }

            if (State.IsKeyDown(Keys.S))
            {
                if (lvlMap[40 * (yPlayerGrid + 1) + xPlayerGrid] != 1)
                    yPlayer += 0.2f;
            }

            if (State.IsKeyDown(Keys.D))
            {
                if (lvlMap[40 * yPlayerGrid + (xPlayerGrid + 1)] != 1)
                xPlayer += 0.2f;
            }

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            spriteBatch.Begin();

           for (int x = 0; x < 40; x++)
                    {
                        for (int y = 0; y < 24; y++)
                        {
                            if (lvlMap[40*y+x] == 1)
                                spriteBatch.Draw(lvlWall, new Rectangle(x * 20, y * 20, 20, 20), Color.White);

                            if (lvlMap[40*y+x] == 2)
                                spriteBatch.Draw(lvlFloor, new Rectangle(x * 20, y * 20, 20, 20), Color.White);

                            if (lvlMap[40 * y + x] == 3)
                                spriteBatch.Draw(lvlDoor, new Rectangle(x * 20, y * 20, 60, 20), Color.White);

                            if (lvlMap[40 * y + x] == 4)
                                spriteBatch.Draw(lvlStairs, new Rectangle(x * 20, y * 20, 20, 20), Color.White);

                            //if (x == xPlayer && y == yPlayer)
                            //    spriteBatch.Draw(lvlPlayer, new Rectangle(x * 20, y * 20, 20, 20), Color.White);
                        }    
                    }

           spriteBatch.Draw(lvlPlayer, new Rectangle((int)(xPlayer * 20), (int)(yPlayer * 20), 20, 20), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
