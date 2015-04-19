using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GamesProgrammingAssignment
{
    public class Player
    {
        private float xPos;
        private float yPos;
        private int xGrid;
        private int yGrid;
        private float xCentre;
        private float yCentre;

        private bool Sword = false;
        private bool Shield = false;
        private int SwordCharges;
        private int ShieldCharges;

        private int Health;

        public Player(int x, int y, int hp)
        {
            xPos = x;
            yPos = y;
            xGrid = x;
            yGrid = y;

            //offsets the centre of the player
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
        public void xyUpdate()
        {
            xCentre = xPos + 0.5f;
            yCentre = yPos + 0.9f;

            xGrid = (int)xCentre;
            yGrid = (int)yCentre;
        }

        public int healthGet()
        {
            return Health;
        }
        public void healthSet(int HP)
        {
            Health = HP;
        }

        public bool swordGet()
        {
            return Sword;
        }
        public bool shieldGet()
        {
            return Shield;
        }
        public void swordSet(bool swordState)
        {
            Sword = swordState;
        }
        public void shieldSet(bool shieldState)
        {
            Shield = shieldState;
        }

        public int swordChargesGet()
        {
            return SwordCharges;
        }
        public int shieldChargesGet()
        {
            return ShieldCharges;
        }
        public void swordChargesSet(int charges)
        {
            SwordCharges += charges;
        }
        public void shieldChargesSet(int charges)
        {
            ShieldCharges += charges;
        }
    }
}
