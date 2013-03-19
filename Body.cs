using System;
using System.Drawing;

namespace OrbitSim
{
    /// <summary>
    /// Body affected by gravity.
    /// </summary>
    class Body : Particle
    {
        public const double G = 6.6738e-11; //Gravitational constant (m^3/kg*s^2)

        //Scales used for collision detection
        private static double starRadiusScale = 1;
        private static double bodyRadiusScale = 1;
        private static double positionScale = 1;

        private static int nbCreatedBodies = 0;

        public String name { get; set; }
        public bool star { get; set; } //True if the body is a star

        /// <summary>
        /// Create a body using it's physical properties.
        /// </summary>
        /// <param name="mass">Mass (kg)</param>
        /// <param name="radius">Radius (m)</param>
        /// <param name="color">Color</param>
        /// <param name="star">True if the body is a star</param>
        /// <param name="x">Position x</param>
        /// <param name="y">Position y</param>
        /// <param name="speedX">Speed X</param>
        /// <param name="speedY">Speed Y</param>
        public Body(String name, double mass, int radius, Color color, bool star, double x, double y, double speedX, double speedY) : 
            base(x, y, speedX, speedY, mass, radius, color)
        {
            this.name = name;
            this.star = star;
        }

        /// <summary>
        /// Create a body not affected by gravity.
        /// </summary>
        /// <param name="radius">Radius (m)</param>
        /// <param name="color">Color</param>
        /// <param name="star">True if the body is a star</param>
        /// <param name="x">Position x (m)</param>
        /// <param name="y">Position y (m)</param>
        public Body(int radius, Color color, bool star, double x, double y):
            base(x, y, 0, 0, -1, radius, color)
        {
            this.name = "Body";//  +nbCreatedBodies++;
            this.star = star;
        }

        public Body(double mass, int radius, Color color, bool star, double x, double y, double speedX, double speedY) :
            this("Body " + nbCreatedBodies++, mass, radius, color, star, x, y, speedX, speedY)
        {
        }

        /*
        * How it works
        * ------------
        * Fg = GMm/r^2 (Fg> = GMm/r^2 * r>unit)
        * F = ma
        * GMm/r^2 = ma
        * a = -GM/r^2 (a> = -GM/r^2 * r>unit)
        * 
        * v = v + a*dt
        * x = x + v*dt
        */
        /// <summary>
        /// Calculate the acceleration produced by the gravity of a body from another.
        /// </summary>
        /// <param name="body">The other body</param>
        /// <param name="dt">Time step (s)</param>
        public void calcAcceleration(Body body, double dt)
        {
            if (this != body) //A body can't interact with itself
            {
                //r> = A.pos> - B.pos>
                Vector2D radius = position - body.position;

                //a> = -Gm/r^2 * r>
                Vector2D acceleration = (-G * body.mass / radius.lengthSquared) * radius.getUnitVector();
                speed += acceleration * dt; //v> = v> + a>*dt
            }
        }

        /// <summary>
        /// Update body's position.
        /// </summary>
        /// <param name="dt">Time step (s)</param>
        public void move(double dt)
        {
            position += speed * dt;
        }

        /// <summary>
        /// Check if the body is in collision with another body.
        /// The collisions detection depends on the collision scales:
        /// Different scales for radius and distance doesn't result in "real" collision detection,
        /// it will collide if it appear that they are colliding, not if it is really colliding in the simulation.
        /// </summary>
        /// <param name="body">The other body.</param>
        /// <returns>True if there is a collision with the other body</returns>
        public bool isColliding(Body body)
        {
            if (body != this)
            {
                //Distance between the 2 bodies possibly in collisions (Squared)
                int distanceSquare = (int)((position - body.position).lengthSquared * positionScale * positionScale);

                //Radius sum of the 2 bodies (Squared)
                int radiusA = (int)(star ? radius * starRadiusScale : radius * bodyRadiusScale);
                int radiusB = (int)(body.star ? body.radius * starRadiusScale : body.radius * bodyRadiusScale);
                int sumRadiusSquare = (radiusA + radiusB) * (radiusA + radiusB);

                if (distanceSquare < sumRadiusSquare)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Do a perfecly inelastic collision another body.
        /// </summary>
        /// <param name="body">The other body.</param>
        public void collide(Body body)
        {
            mass += body.mass;

            //v = (m1*u1 + m2*u2)/(m1 + m2)
            Vector2D newSpeed = (speed * mass + body.speed * body.mass) / (mass + body.mass);
            speed = newSpeed;
            colorAddition(body.color);
        }

        /// <summary>
        /// Blend the body's color with another color.
        /// </summary>
        /// <param name="color">The other color.</param>
        private void colorAddition(Color color)
        {
            int r = (this.color.R + color.R) / 2;
            int g = (this.color.G + color.G) / 2;
            int b = (this.color.B + color.B) / 2;

            this.color = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Change the collision scales.
        /// </summary>
        /// <param name="bodyScale">Body radius scale.</param>
        /// <param name="starScale">Star radius scale.</param>
        /// <param name="posScale">Position scale.</param>
        public static void setCollisionScales(double bodyScale, double starScale, double posScale)
        {
            starRadiusScale = starScale;
            bodyRadiusScale = bodyScale;
            positionScale = posScale;
        }

        public override string ToString()
        {
            return base.ToString() +" Star: " + star + "\n" +
                   "Name: " + name;
        }
    }
}
