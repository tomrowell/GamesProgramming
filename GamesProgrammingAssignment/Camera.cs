using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GamesProgrammingAssignment
{
    class Camera
    {
        private float xPos;
        private float yPos;
        private Matrix Transform;
        private float Rotation;
        private float Zoom;

        public Camera(float x, float y)
        {
            xPos = x * -20f;
            yPos = y * -20f;
            Rotation = 0f;
            Zoom = 2f;
        }        
        
        public void cameraMove(float xDistance, float yDistance)
        {
            xPos = xDistance * -20;
            yPos = yDistance * -20;
        }

        public Matrix cameraTransform(GraphicsDeviceManager graphics)
        {
            Transform = Matrix.CreateTranslation(new Vector3(xPos, yPos, 0)) * Matrix.CreateRotationZ(Rotation) * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) * Matrix.CreateTranslation(graphics.PreferredBackBufferWidth * 0.5f, graphics.PreferredBackBufferHeight * 0.5f, 0);
            return Transform;
        }
    }
}
