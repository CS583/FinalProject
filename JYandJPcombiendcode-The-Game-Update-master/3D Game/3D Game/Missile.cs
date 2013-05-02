using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3D_Game
{
    class Missile : BasicModel
    {
        Matrix rotation = Matrix.Identity;
        float yawAngle = 0;
        float pitchAngle = 0;
        float rollAngle = 0;
        Vector3 direction;
        Vector3 enemyPosition;
        Vector3 currentPosition;

        public Missile(Model m, Vector3 Position,
            Vector3 Direction, float yaw,
            float pitch, float roll, Vector3 EnemyPosition)
            : base(m)
        {
            currentPosition = Position;
            world = Matrix.CreateTranslation(Position);
            yawAngle = yaw;
            pitchAngle = pitch;
            rollAngle = roll;
            direction = Direction;
            enemyPosition = EnemyPosition;
        }

        public override void Update()
        {
            rotation *= Matrix.CreateFromYawPitchRoll(yawAngle,
                pitchAngle, rollAngle);

            //Move model
            computeDirection();
            world *= Matrix.CreateTranslation(direction);
        }

        public override Matrix GetWorld()
        {
            return rotation * world;
        }

        public void computeDirection()
        {
            direction.X = enemyPosition.X - currentPosition.X;
            direction.Y = enemyPosition.Y - currentPosition.Y;
            direction.Z = currentPosition.Z -200;
            direction.Normalize();
            direction *= 5;

            currentPosition += direction;
            //enemyPosition.Z += 1;
            
            
        }
    }
}
