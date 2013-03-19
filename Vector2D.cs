
namespace OrbitSim
{
    /// <summary>
    /// Vector2D (Simple and fast)
    /// Author: Keven Villeneuve
    /// </summary>
    public class Vector2D
    {
        public double x { get; set; } //X component
        public double y { get; set; } //Y component

        public double lengthSquared //Vector's length squared
        {
            get { return x*x + y*y; }
        }

        public double length //Vector's length
        {
            get { return System.Math.Sqrt(lengthSquared); }
        }

        /// <summary>
        /// Create a vector from it's components.
        /// </summary>
        /// <param name="x">X component.</param>
        /// <param name="y">Y component.</param>
        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Return a copy of the normalized vector.
        /// </summary>
        /// <returns>Normalized vector</returns>
        public Vector2D getUnitVector()
        {
            double length = this.length;
            return new Vector2D(x/length, y/length);
        }

        /// <summary>
        /// Overload + : Add 2 vectors.
        /// </summary>
        /// <param name="vector1">Vector 1</param>
        /// <param name="vector2">Vector 2</param>
        /// <returns>Vector added of vector1 and vector2</returns>
        public static Vector2D operator +(Vector2D vector1, Vector2D vector2)
        {
            return new Vector2D(vector1.x + vector2.x, vector1.y + vector2.y);
        }

        /// <summary>
        /// Overload + : Subtract 2 vectors.
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns>Vector subbed of vector1 and vector2</returns>
        public static Vector2D operator -(Vector2D vector1, Vector2D vector2)
        {
            return new Vector2D(vector1.x - vector2.x, vector1.y - vector2.y);
        }

        /// <summary>
        /// Overload + : Multiply a vector and a value.
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="value">Value</param>
        /// <returns>Vector multiplied by a value</returns>
        public static Vector2D operator *(Vector2D vector, double value)
        {
            return new Vector2D(vector.x*value, vector.y*value);
        }

        /// <summary>
        /// Overload + : Multiply a value and a vector.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="vector">Vector</param>
        /// <returns>Vector multiplied by a value</returns>
        public static Vector2D operator *(double value, Vector2D vector)
        {
            return new Vector2D(vector.x * value, vector.y * value);
        }

        /// <summary>
        /// Overload + : Divide a vector and a value.
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="value">Value</param>
        /// <returns>Vector divided by a value</returns>
        public static Vector2D operator /(Vector2D vector, double value)
        {
            return new Vector2D(vector.x * 1/value, vector.y * 1/value);
        }

        public override string ToString()
        {
            return "Vector2D: (" + x + "," + y + ") :" + length;
        }
    }
}
