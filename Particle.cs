using System.Drawing;

namespace OrbitSim
{
    /// <summary>
    /// A physical particle.
    /// </summary>
    abstract class Particle
    {
        public double mass { get; set; } //Mass
        public int radius { get; set; } //Radius
        public Color color { get; set; } //Color
        public Vector2D position { get; set; } //Position vector
        public Vector2D speed { get; set; } //Speed vector

        /// <summary>
        /// Create a particle using it's physical properties.
        /// </summary>
        /// <param name="x">Position x</param>
        /// <param name="y">Position y</param>
        /// <param name="speedX">Speed x</param>
        /// <param name="speedY">Speed y</param>
        /// <param name="mass">Mass</param>
        /// <param name="radius">Radius</param>
        /// <param name="color">Color</param>
        protected Particle(double x, double y, double speedX, double speedY, double mass, int radius, Color color) : 
            this(new Vector2D(x, y), new Vector2D(speedX, speedY), mass, radius, color)
        {
        }

        /// <summary>
        /// Create a particle using it's physical properties (Vector)
        /// </summary>
        /// <param name="position">Position vector</param>
        /// <param name="speed">Speed vector</param>
        /// <param name="mass">Mass</param>
        /// <param name="radius">Radius</param>
        /// <param name="color">Color</param>
        protected Particle(Vector2D position, Vector2D speed, double mass, int radius, Color color)
        {
            this.mass = mass;
            this.radius = radius;
            this.color = color;
            this.position = position;
            this.speed = speed;
        }

        public override string ToString()
        {
            return "Position: " + position + "\n" +
                   "Speed: " + speed + "\n" +
                   "Mass: " + mass + " Radius: " + radius + " Color: " + color;
        }
    }
}
