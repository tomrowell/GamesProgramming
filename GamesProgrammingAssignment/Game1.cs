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

        private int count;

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

        private int[] lvlMap = new int[80 * 48];

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 960;
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

            for (int x = 0; x < 80 * 48; x++)
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
                        xLeft = 20;
                        xRight = 60;
                        yTop = 6;
                        yBot = 42;
                        xDoor = 39;
                        yDoor = 6;
                        xStairs = 40;
                        yStairs = 38;
                        xPlayer = 40;
                        yPlayer = 8;

                        lvlMapAssign();
                        
                        xDoor = -1;
                        yDoor = -1;
                        xStairs = -1;
                        yStairs = -1;
                        break;
                    default:
                        for (int x = 0; x < 80 * 48; x++)
                        {
                            lvlMap[x] = 0;
                        }
                        if (lvlMapNumber < 6) //checks if the level number is less than 6
                        {
                            count = 0;
                            Random rand = new Random();
                            //generates the same amount of rooms as the level number
                            do
                            {
                                //randomly generate a small room
                                xLeft = rand.Next(3,54);
                                xRight = xLeft + rand.Next(5,17);
                                yTop = rand.Next(3,22);
                                yBot = yTop + rand.Next(5,17);

                                //if the room does not overlap with another room, draws the room and count increases by one
                                lvlMapAssign();
                                count += 1;
                            }
                            while (count != lvlMapNumber);

                            xLeft = -1;
                            xRight = -1;
                            yTop = -1;
                            yBot = -1;

                            //randomly selects a floorspace in a room to replace with stairs
                            do
                            {
                                xStairs = rand.Next(0, 80);
                                yStairs = rand.Next(0, 48);
                            }
                            while (lvlMap[80 * yStairs + xStairs] != 2);
                            lvlMapAssign();

                        }
                        //else
                        break;
                }
                newFloor = false;
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
                if (lvlMap[80 * (yPlayerGrid - 1) + xPlayerGrid] != 1 && lvlMap[80 * (yPlayerGrid - 1) + xPlayerGrid] != 3 && lvlMap[80 * (yPlayerGrid - 1) + xPlayerGrid] != -1) //checks to see if there is a wall or door in the direction the player is moving
                    yPlayer += -0.2f;
                else if (yPlayer - Math.Truncate(yPlayer) > 0.2) //if there is a wall adjacent to the player, checks that they are not too close
                    yPlayer += -0.2f;
            }

            if (State.IsKeyDown(Keys.A))
            {
                if (lvlMap[80 * yPlayerGrid + (xPlayerGrid - 1)] != 1)
                    xPlayer += -0.2f;
                else if (xPlayer - Math.Truncate(xPlayer) > 0.2)
                    xPlayer += -0.2f;
            }

            if (State.IsKeyDown(Keys.S))
            {
                if (lvlMap[80 * (yPlayerGrid + 1) + xPlayerGrid] != 1)
                    yPlayer += 0.2f;
                else if (yPlayer - Math.Truncate(yPlayer) < 0.8)
                    yPlayer += 0.2f;
            }

            if (State.IsKeyDown(Keys.D))
            {
                if (lvlMap[80 * yPlayerGrid + (xPlayerGrid + 1)] != 1)
                    xPlayer += 0.2f;
                else if (xPlayer - Math.Truncate(xPlayer) < 0.8)
                    xPlayer += 0.2f;
            }

            //detects if player walks over the stairs
            if (lvlMap[80 * yPlayerGrid + xPlayerGrid] == 4)
            {
                lvlMap[80 * yPlayerGrid + xPlayerGrid] = 2;
                lvlMapNumber += 1;
                newFloor = true;
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

            for (int x = 0; x < 80; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    if (lvlMap[80 * y + x] == 1)
                        spriteBatch.Draw(lvlWall, new Rectangle(x * 20, y * 20, 20, 20), Color.White);

                    if (lvlMap[80 * y + x] == 2)
                        spriteBatch.Draw(lvlFloor, new Rectangle(x * 20, y * 20, 20, 20), Color.White);

                    if (lvlMap[80 * y + x] == 3)
                        spriteBatch.Draw(lvlDoor, new Rectangle(x * 20, y * 20, 60, 20), Color.White);

                    if (lvlMap[80 * y + x] == 4)
                        spriteBatch.Draw(lvlStairs, new Rectangle(x * 20, y * 20, 20, 20), Color.White);
                }
            }

            spriteBatch.Draw(lvlPlayer, new Rectangle((int)(xPlayer * 20), (int)(yPlayer * 20), 20, 20), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        private void lvlMapAssign()
        {
            //assigning all values to the level array
            for (int x = 0; x < 80; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    if (lvlMap[80 * y + x] == 0)
                    {
                        //assigning values for walls 
                        if ((x >= xLeft && x <= xRight && y == yTop)
                            || (x >= xLeft && x <= xRight && y == yBot)
                            || (y > yTop && y < yBot && x == xLeft)
                            || (y > yTop && y < yBot && x == xRight))
                            lvlMap[80 * y + x] = 1;

                        //assigning values for floors
                        if (x > xLeft && x < xRight && y > yTop && y < yBot)
                            lvlMap[80 * y + x] = 2;

                        //assigning values for door (if any)
                        if (x == xDoor && y == yDoor && lvlMapNumber == 1)
                        {
                            lvlMap[80 * y + x] = 3;
                            lvlMap[(80 * y + x) + 1] = -1;
                            lvlMap[(80 * y + x) + 2] = -1;
                        }

                        //assigning values for stairs
                        if (x == xStairs && y == yStairs)
                        {
                            lvlMap[80 * y + x] = 4;
                        }
                    }
                }
            }
        }
    }
}
