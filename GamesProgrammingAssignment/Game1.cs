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

        Camera PlayerCamera = new Camera(0, 0);

        private int lvlMapNumber;
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
        private int xChest;
        private int yChest;
        private int typeChest;
        private Random randChest;
        private Random randCharges;

        int xEnemy;
        int yEnemy;

        private int xCorridor;
        private int yCorridor;
        private int xySelect;
        private int lengthCorridor;
        private bool completedCorridor;

        private Texture2D lvlFloor;
        private Texture2D lvlWall;
        private Texture2D lvlDoor;
        private Texture2D lvlStairs;
        private Texture2D lvlPlayer;
        private Texture2D lvlEnemy;
        private Texture2D lvlPlayerSword;
        private Texture2D lvlPlayerShield;
        private Texture2D lvlPlayerSwordShield;
        private Texture2D lvlChest;

        private Texture2D uiDeath;

        //creates an array to store the map grid
        private int[] lvlMap = new int[80 * 48];

        //creates an instance of the Player class
        Player Player1 = new Player(40, 8, 10);
        Enemy[] Enemies;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SynchronizeWithVerticalRetrace = true;
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
            lvlEnemy = Content.Load<Texture2D>("Enemy");
            lvlPlayerSword = Content.Load<Texture2D>("PlayerSword");
            lvlPlayerShield = Content.Load<Texture2D>("PlayerShield");
            lvlPlayerSwordShield = Content.Load<Texture2D>("PlayerSwordShield");
            lvlChest = Content.Load<Texture2D>("Chest");

            uiDeath = Content.Load<Texture2D>("DeathScreen");
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

            //generates a new floor when needed
            if (newFloor)
            {
                lvlMapGenerate();
                newFloor = false;
            }

            //updates the x and y coordinates for the player
            Player1.xyUpdate();


            //updates the coordinates for the enemies and makes them chase the player 
            for (int c = 0; c < lvlMapNumber - 1; c++)
            {
                if (Enemies[c].deadGet() == false)
                {
                    Enemies[c].xyUpdate(Player1.xGridGet(), Player1.yGridGet());

                    if (Enemies[c].seenGet() && Enemies[c].attackedGet() == false)
                    {
                        if (Enemies[c].xGridGet() > Player1.xGridGet())
                        {
                            if (Enemies[c].xCentreGet() % 1 > 0.1 || lvlMap[80 * Enemies[c].yGridGet() + (Enemies[c].xGridGet() - 1)] == 2)
                                Enemies[c].xPosSet(Enemies[c].xPosGet() + (-2f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                        }
                        if (Enemies[c].xGridGet() < Player1.xGridGet())
                        {
                            if (Enemies[c].xCentreGet() % 1 < 0.9 || lvlMap[80 * Enemies[c].yGridGet() + (Enemies[c].xGridGet() + 1)] == 2)
                                Enemies[c].xPosSet(Enemies[c].xPosGet() + (2f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                        }
                        if (Enemies[c].yGridGet() > Player1.yGridGet())
                        {
                            if (Enemies[c].yCentreGet() % 1 > 0.1 || lvlMap[80 * (Enemies[c].yGridGet() - 1) + Enemies[c].xGridGet()] == 2)
                            Enemies[c].yPosSet(Enemies[c].yPosGet() + (-2f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                        }
                        if (Enemies[c].yGridGet() < Player1.yGridGet())
                        {
                            if (Enemies[c].yCentreGet() % 1 < 0.9 || lvlMap[80 * (Enemies[c].yGridGet() + 1) + Enemies[c].xGridGet()] == 2)
                            Enemies[c].yPosSet(Enemies[c].yPosGet() + (2f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                        }

                        //checks to see if there was a collision between the player and the enemy
                        if (Enemies[c].xGridGet() == Player1.xGridGet() && Enemies[c].yGridGet() == Player1.yGridGet())
                        {
                            //checks to see if the player had a shield to block the damage
                            if (Player1.shieldGet())
                            {
                                Player1.shieldChargesSet(-1);
                                if (Player1.shieldChargesGet() == 0)
                                    Player1.shieldSet(false);
                            }
                            else
                            {
                                Player1.healthSet(Player1.healthGet() - 1);
                                if (Player1.healthGet() == 0)
                                    Player1.dead();
                            }

                            //checks to see if the player had a sword to kill the enemy with
                            if (Player1.swordGet())
                            {
                                Enemies[c].healthSet(1);
                                Player1.swordChargesSet(-1);
                                if (Player1.swordChargesGet() == 0)
                                    Player1.swordSet(false);

                                if (Enemies[c].healthGet() == 0)
                                    Enemies[c].dead();
                            }

                            //triggers a timer to stop the enemy acting for 1 second after attacking the player
                            Enemies[c].playerAttacked();
                        }
                    }
                }
            }

            //reads the current state of the keyboard
            KeyboardState State = Keyboard.GetState();

            //checks to see if the player is dead
            //if the player is dead, they can press enter to exit or backspace to restart
            if (Player1.deadGet())
            {
                if (State.IsKeyDown(Keys.Back))
                {
                    Exit();
                    Game1 game = new Game1();
                    game.Run();
                }

                if (State.IsKeyDown(Keys.Enter))
                {
                    Exit();
                }
            }
            //if the player is not dead, allows them to move with W A S D
            else
            {
                if (State.IsKeyDown(Keys.W))
                {
                    //checks to make sure the player can move upwards
                    if (Player1.yCentreGet() % 1 > 0.1 || lvlMap[80 * (Player1.yGridGet() - 1) + Player1.xGridGet()] == 2 || lvlMap[80 * (Player1.yGridGet() - 1) + Player1.xGridGet()] == 4 || lvlMap[80 * (Player1.yGridGet() - 1) + Player1.xGridGet()] == 5)
                        //moves the player upwards in real time
                        Player1.yPosSet(Player1.yPosGet() + (-5f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }

                if (State.IsKeyDown(Keys.A))
                {
                    //checks to make sure the player can move left
                    if (Player1.xCentreGet() % 1 > 0.1 || lvlMap[80 * Player1.yGridGet() + (Player1.xGridGet() - 1)] == 2 || lvlMap[80 * Player1.yGridGet() + (Player1.xGridGet() - 1)] == 4 || lvlMap[80 * Player1.yGridGet() + (Player1.xGridGet() - 1)] == 5)
                        //moves the player left in real time
                        Player1.xPosSet(Player1.xPosGet() + (-5f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }

                if (State.IsKeyDown(Keys.S))
                {
                    //checks to make sure the player can move downwards
                    if (Player1.yCentreGet() % 1 < 0.9 || lvlMap[80 * (Player1.yGridGet() + 1) + Player1.xGridGet()] == 2 || lvlMap[80 * (Player1.yGridGet() + 1) + Player1.xGridGet()] == 4 || lvlMap[80 * (Player1.yGridGet() + 1) + Player1.xGridGet()] == 5)
                        //moves the player downwards in real time
                        Player1.yPosSet(Player1.yPosGet() + (5f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }

                if (State.IsKeyDown(Keys.D))
                {
                    //checks to make sure the player can move right
                    if (Player1.xCentreGet() % 1 < 0.9 || lvlMap[80 * Player1.yGridGet() + (Player1.xGridGet() + 1)] == 2 || lvlMap[80 * Player1.yGridGet() + (Player1.xGridGet() + 1)] == 4 || lvlMap[80 * Player1.yGridGet() + (Player1.xGridGet() + 1)] == 5)
                        //moves the player right in real time
                        Player1.xPosSet(Player1.xPosGet() + (5f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }

                //inputs the player's new position into the camera position
                PlayerCamera.cameraMove(Player1.xPosGet(), Player1.yPosGet());
            }

            //detects if the player walks over the chest
            if (lvlMap[80 * Player1.yGridGet() + Player1.xGridGet()] == 5)
            {
                randChest = new Random();
                randCharges = new Random();
                typeChest = randChest.Next(1, 10);
                //randomly selects whether to give the player a sword or a shield
                if (typeChest % 2 == 1)
                {
                    //checks if the player already has a sword, and either gives them one with charges, or adds to their current charges
                    if (Player1.swordGet() == false)
                        Player1.swordSet(true);
                    Player1.swordChargesSet(randCharges.Next(1, 5));
                }
                else
                {
                    //checks if the player already has a shield, and either gives them one with charges, or adds to their current charges
                    if (Player1.shieldGet() == false)
                        Player1.shieldSet(true);
                    Player1.shieldChargesSet(randCharges.Next(1, 5));
                }

                //removes the chest
                xChest = -1;
                yChest = -1;
                lvlMap[80 * Player1.yGridGet() + Player1.xGridGet()] = 2;
            }

            //detects if the player walks over the stairs and moves onto the next level
            if (lvlMap[80 * Player1.yGridGet() + Player1.xGridGet()] == 4)
            {
                lvlMap[80 * Player1.yGridGet() + Player1.xGridGet()] = 2;
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

            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, PlayerCamera.cameraTransform(graphics));

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

                    if (lvlMap[80 * y + x] == 5)
                        spriteBatch.Draw(lvlChest, new Rectangle(x * 20, y * 20, 20, 20), Color.White);
                }
            }

            if (newFloor != true)
            {
                for (int c = 0; c < lvlMapNumber - 1; c++)
                {
                    spriteBatch.Draw(lvlEnemy, new Rectangle((int)(Enemies[c].xPosGet() * 20), (int)(Enemies[c].yPosGet() * 20), 20, 20), Color.White);
                }
            }

            if (Player1.swordGet() && Player1.shieldGet())
                spriteBatch.Draw(lvlPlayerSwordShield, new Rectangle((int)(Player1.xPosGet() * 20), (int)(Player1.yPosGet() * 20), 20, 20), Color.White);
            else if (Player1.swordGet())
                spriteBatch.Draw(lvlPlayerSword, new Rectangle((int)(Player1.xPosGet() * 20), (int)(Player1.yPosGet() * 20), 20, 20), Color.White);
            else if (Player1.shieldGet())
                spriteBatch.Draw(lvlPlayerShield, new Rectangle((int)(Player1.xPosGet() * 20), (int)(Player1.yPosGet() * 20), 20, 20), Color.White);
            else
                spriteBatch.Draw(lvlPlayer, new Rectangle((int)(Player1.xPosGet() * 20), (int)(Player1.yPosGet() * 20), 20, 20), Color.White);

            if (Player1.deadGet())
                spriteBatch.Draw(uiDeath, new Rectangle((int)(Player1.xPosGet() * 20) - 200, (int)(Player1.yPosGet() * 20) - 112, 400, 225), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void lvlMapGenerate()
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
                    yStairs = 24;
                    xChest = -1;
                    yChest = -1;

                    //assigns the values given above to the map grid
                    lvlMapRoomAssign();

                    xDoor = -1;
                    yDoor = -1;
                    break;
                default:
                    for (int x = 0; x < 80 * 48; x++)
                    {
                        lvlMap[x] = 0;
                    }
                    count = 0;
                    Random rand = new Random();
                    Random rand2 = new Random(rand.Next(0, 100));
                    Random rand3 = new Random(rand2.Next(0, 100));
                    Random rand4 = new Random(rand3.Next(0, 100));
                    Random rand5 = new Random(rand4.Next(0, 100));
                    Random rand6 = new Random(rand5.Next(0, 100));
                    Random rand7 = new Random(rand6.Next(0, 100));
                    Random rand8 = new Random(rand7.Next(0, 100));
                    Random rand9 = new Random(rand8.Next(0, 100));
                    Random rand10 = new Random(rand9.Next(0, 100));
                    Random rand11 = new Random(rand10.Next(0, 100));
                    Random rand12 = new Random(rand11.Next(0, 100));
                    Random rand13 = new Random(rand12.Next(0, 100));
                    Random rand14 = new Random(rand13.Next(0, 100));

                    //generates the same amount of rooms as the level number
                    do
                    {
                        //randomly generate a small room
                        xLeft = rand.Next(3, 57);
                        xRight = xLeft + rand2.Next(5, 22);
                        yTop = rand3.Next(3, 25);
                        yBot = yTop + rand4.Next(5, 22);

                        lvlMapRoomAssign();
                        count += 1;
                    }
                    while (count < lvlMapNumber && count < 15);

                    xLeft = -1;
                    xRight = -1;
                    yTop = -1;
                    yBot = -1;

                    //randomly selects a floorspace in a room to replace with stairs
                    do
                    {
                        xStairs = rand5.Next(0, 80);
                        yStairs = rand6.Next(0, 48);
                    }
                    while (lvlMap[80 * yStairs + xStairs] != 2);
                    lvlMapRoomAssign();

                    //randomly places the player on an available floorspace in a room
                    do
                    {
                        Player1.xGridSet(rand7.Next(0, 80));
                        Player1.yGridSet(rand8.Next(0, 48));
                    }
                    while (lvlMap[80 * Player1.yGridGet() + Player1.xGridGet()] != 2
                        || lvlMap[80 * Player1.yGridGet() + (Player1.xGridGet() + 1)] != 2
                        || lvlMap[80 * Player1.yGridGet() + (Player1.xGridGet() - 1)] != 2
                        || lvlMap[80 * (Player1.yGridGet() + 1) + Player1.xGridGet()] != 2
                        || lvlMap[80 * (Player1.yGridGet() - 1) + Player1.xGridGet()] != 2
                        || Player1.xGridGet() == xStairs && Player1.yGridGet() == yStairs);
                    Player1.xPosSet(Player1.xGridGet());
                    Player1.yPosSet(Player1.yGridGet());

                    do
                    {
                        xChest = rand13.Next(0, 80);
                        yChest = rand14.Next(0, 48);
                    }
                    while (lvlMap[80 * yChest + xChest] != 2);
                    lvlMapRoomAssign();

                    //creates a path from the player to the stairs
                    completedCorridor = false;
                    //reads in the player's position as the current point
                    xCorridor = Player1.xGridGet();
                    yCorridor = Player1.yGridGet();
                    //picks either the x or y axis
                    xySelect = rand9.Next(1, 2);
                    do
                    {
                        if (xySelect == 1)
                        {
                            //randomly chooses a distance between 1 and the total distance between the current point and the stairs in the x axis
                            //then moves the current point to the end of the corridor
                            if (xCorridor > xStairs)
                            {
                                lengthCorridor = xCorridor - xStairs;
                                xCorridor -= lengthCorridor;
                            }
                            else if (xCorridor < xStairs)
                            {
                                lengthCorridor = xStairs - xCorridor;
                                xCorridor += lengthCorridor;
                            }
                            else
                                lengthCorridor = 0;

                            //switches the selected axis
                            xySelect = 2;
                        }
                        else
                        {
                            //randomly chooses a distance between 1 and the total distance between the current point and the stairs in the y axis
                            //then moves the current point to the end of the corridor
                            if (yCorridor > yStairs)
                            {
                                //lengthCorridor = rand10.Next(1, yCorridor - yStairs);
                                lengthCorridor = yCorridor - yStairs;
                                yCorridor -= lengthCorridor;
                            }
                            else if (yCorridor < yStairs)
                            {
                                //lengthCorridor = rand10.Next(1, yStairs - yCorridor);
                                lengthCorridor = yStairs - yCorridor;
                                yCorridor += lengthCorridor;
                            }
                            else
                                lengthCorridor = 0;

                            //switches the selected axis
                            xySelect = 1;
                        }

                        //draws a corridor of the chosen length from the current point
                        if (lengthCorridor > 0)
                            lvlMapCorridorAssign();
                    }
                    while (completedCorridor == false);
                    //repeats until the stairs are reached

                    //creates an amount of enemies relative to the floor number
                    Enemies = new Enemy[lvlMapNumber - 1];
                    for (int c = 0; c < lvlMapNumber - 1; c++)
                    {
                        //randomly places enemies on any available floorspace that is not adjacent to the player
                        do
                        {
                        xEnemy = rand11.Next(0, 80);
                        yEnemy = rand12.Next(0, 48);
                        }
                        while (lvlMap[80 * yEnemy + xEnemy] != 2
                        || xEnemy == Player1.xGridGet() && yEnemy == Player1.yGridGet()
                        || xEnemy + 1 == Player1.xGridGet() && yEnemy == Player1.yGridGet()
                        || xEnemy - 1 == Player1.xGridGet() && yEnemy == Player1.yGridGet()
                        || xEnemy == Player1.xGridGet() && yEnemy + 1 == Player1.yGridGet()
                        || xEnemy == Player1.xGridGet() && yEnemy - 1 == Player1.yGridGet()
                        || xEnemy == xStairs && yEnemy == yStairs);

                        //creates an instance of the Enemy class for each enemy
                        Enemies[c] = new Enemy(xEnemy, yEnemy, 1);
                    }
                    break;
            }
            //clears the previous set of stairs and chest
            xStairs = -1;
            yStairs = -1;
            xChest = -1;
            yChest = -1;
        }

        private void lvlMapRoomAssign()
        {
            //assigning all values to the level array
            for (int x = 0; x < 80; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    //assigning values for room walls using xLeft, xRight, yTop and yBottom as boundaries
                    //also will not place a wall if a floor tile is already there (to merge any overlapping rooms)
                    if ((x >= xLeft && x <= xRight && y == yTop) && (lvlMap[80 * y + x] != 2)
                        || (x >= xLeft && x <= xRight && y == yBot) && (lvlMap[80 * y + x] != 2)
                        || (y > yTop && y < yBot && x == xLeft) && (lvlMap[80 * y + x] != 2)
                        || (y > yTop && y < yBot && x == xRight) && (lvlMap[80 * y + x] != 2))
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

                    //assigning values for chest
                    if (x == xChest && y == yChest)
                    {
                        lvlMap[80 * y + x] = 5;
                    }
                }
            }
        }

        private void lvlMapCorridorAssign()
        {
            for (int x = 0; x < 80; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    //checks to see which axis the corridor is being drawn along
                    switch (xySelect)
                    {

                        case 1:
                            //checks if selected tile is on the corridor
                            if ((Player1.yGridGet() > yStairs && y <= yCorridor + lengthCorridor && y >= yStairs && x == xCorridor)
                                || (Player1.yGridGet() < yStairs && y >= yCorridor - lengthCorridor && y <= yStairs && x == xCorridor))
                            {
                                //checks to see if the stairs have been reached
                                if (x == xStairs && y == yStairs)
                                {
                                    completedCorridor = true;
                                }
                                //draws the path tile if it is not the stairs
                                if (lvlMap[80 * y + x] != 4)
                                    lvlMap[80 * y + x] = 2;
                                //draws walls next to the path if there is nothing there already
                                if (lvlMap[80 * y + (x + 1)] == 0)
                                    lvlMap[80 * y + (x + 1)] = 1;
                                if (lvlMap[80 * y + (x - 1)] == 0)
                                    lvlMap[80 * y + (x - 1)] = 1;
                            }
                            break;

                        default:
                            //draws the path from along the x axis
                            if ((Player1.xGridGet() > xStairs && x <= xCorridor + lengthCorridor && x >= xStairs && y == yCorridor)
                                || (Player1.xGridGet() < xStairs && x >= xCorridor - lengthCorridor && x <= xStairs && y == yCorridor))
                            {
                                //checks to see if the stairs have been reached
                                if (x == xStairs && y == yStairs)
                                {
                                    completedCorridor = true;
                                }
                                //draws the path tile if it is not the stairs
                                if (lvlMap[80 * y + x] != 4)
                                    lvlMap[80 * y + x] = 2;
                                //draws walls next to the path if there is nothing there already
                                if (lvlMap[80 * (y + 1) + x] == 0)
                                    lvlMap[80 * (y + 1) + x] = 1;
                                if (lvlMap[80 * (y - 1) + x] == 0)
                                    lvlMap[80 * (y - 1) + x] = 1;
                            }
                            break;
                    }
                }
            }
            //fills in any corners of corridors
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (lvlMap[80 * (yCorridor + y) + (xCorridor + x)] == 0)
                        lvlMap[80 * (yCorridor + y) + (xCorridor + x)] = 1;
                }
            }
        }
    }
}
