using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
    class ShootingEnemy : BasicModel
    {
        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        Vector3 direction;
        int shotDelay = 180;
        int timeUntilShot = 0;
        bool shoot = false;
        Vector3 currentPosition;

        public ShootingEnemy(Model m, Vector3 Position,
            Vector3 Direction, float yaw,
            float pitch, float roll)
            : base(m)
        {
            world = Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            currentPosition = Position;
        }

        public override void Update()
        {
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle,
                pitchAngle, rollAngle);

            //Move model
            world *= Matrix.CreateTranslation(direction);
            currentPosition += direction;

            if (timeUntilShot < 1)
            {
                shoot = true;
                timeUntilShot = shotDelay;
            }
            timeUntilShot--;
        }

        public override Matrix GetWorld()
        {
            return rotation * world;
        }

        public bool getShoot(){
            return shoot;
        }

        public void setShoot(bool set)
        {
            shoot = set;
        }

        public Vector3 getDirection()
        {
            return direction;
        }

        public Vector3 getPosition()
        {
            return currentPosition;
        }

    }
}