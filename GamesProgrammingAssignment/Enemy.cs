using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesProgrammingAssignment
{
    class Enemy
    {
        private float xPos;
        private float yPos;
        private int xGrid;
        private int yGrid;
        private float xCentre;
        private float yCentre;

        private int Health;

        private bool Seen = false;
        private bool Attacked = false;
        private Timer attackTimer;

        public Enemy(int x, int y, int hp)
        {
            xPos = x;
            yPos = y;
            xGrid = x;
            yGrid = y;

            //offsets the centre of the enemy
            xCentre = x + 0.5f;
            yCentre = y + 0.9f;

            Health = hp;
        }

        //allows other classes to retrieve and change the private variables xPos and yPos
        public float xPosGet()
        {
            return xPos;
        }
        public float yPosGet()
        {
            return yPos;
        }
        public void xPosSet(float x)
        {
            xPos = x;
        }
        public void yPosSet(float y)
        {
            yPos = y;
        }

        //allows other classes to retrieve and change the private variables xGrid and yGrid
        public int xGridGet()
        {
            return xGrid;
        }
        public int yGridGet()
        {
            return yGrid;
        }
        public void xGridSet(int x)
        {
            xGrid = x;
        }            
        public void yGridSet(int y)
        {
            yGrid = y;
        }

        //allows other classes to retrieve the private variables xCentre and yCentre
        public float xCentreGet()
        {
            return xCentre;
        }
        public float yCentreGet()
        {
            return yCentre;
        }

        //allows other classes to update xCentre, yCentre, xGrid and yGrid according to xPos and yPos
        public void xyUpdate(int xPlayer, int yPlayer)
        {
            xCentre = xPos + 0.5f;
            yCentre = yPos + 0.9f;

            xGrid = (int)xCentre;
            yGrid = (int)yCentre;

            //checks to see if the enemy has seen the player
            if (xGrid - xPlayer < 5 && xPlayer - xGrid < 5 && yGrid - yPlayer < 5 && yPlayer - yGrid < 5)
                Seen = true;


        }

        //allows other classes to retrieve the Seen boolean
        public bool seenGet()
        {
            return Seen;
        }

        //allows other classes to see if the enemy has attacked the player recently
        public bool attackedGet()
        {
            return Attacked;
        }
        //allows other classes to activate a timer when the enemy has attacked the player
        public void playerAttacked()
        {
            Attacked = true;
            attackTimer = new System.Timers.Timer(1000);
            attackTimer.Elapsed += attackedReset;
            attackTimer.Enabled = true;
        }
        private void attackedReset(Object a, ElapsedEventArgs b)
        {
            Attacked = false;
            attackTimer.Enabled = false;
        }
    }
}
