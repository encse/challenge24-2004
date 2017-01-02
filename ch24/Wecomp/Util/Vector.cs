using System;

namespace Wecomp.Util
{
    public class Vector : IComparable
    {
        public readonly double X;
        public readonly double Y;

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector operator *(double c, Vector v)
        {
            return new Vector(c * v.X, c * v.Y);
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public Vector Rotate(double rad)
        {
            var radTheta = rad;

            var xT = Math.Cos(radTheta) * X - Math.Sin(radTheta) * Y;
            var yT = Math.Sin(radTheta) * X + Math.Cos(radTheta) * Y;
            return new Vector(xT, yT);
        }
        

        public Vector RotateDeg(double degree)
        {
            var radTheta = degree / 180 * Math.PI;

            var xT = Math.Cos(radTheta) * X - Math.Sin(radTheta) * Y;
            var yT = Math.Sin(radTheta) * X + Math.Cos(radTheta) * Y;
            return new Vector(xT, yT);
        }

        public int CompareTo(object obj)
        {
            var that = (Vector)obj;
            if (Math.Abs(this.Y - that.Y) < 0.00001)
                return Math.Sign(this.X - that.X);
            return Math.Sign(this.Y - that.Y);
        }

        /// <summary>
        /// angle, formed by the x-axis and the vector
        /// </summary>
        public double Angle()
        {
            var radians = Math.Atan2(Y, X);
            return radians * (180 / Math.PI);
        }

        public static double CrossProduct(Vector v1, Vector v2)
        {
            return v1.X * v2.Y - v2.X * v1.Y;
        }

        public string Tsto()
        {
            return "[{0},{1}]".StFormat(X, Y);
        }
    }	
}
