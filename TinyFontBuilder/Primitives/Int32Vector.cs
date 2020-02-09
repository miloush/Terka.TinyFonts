namespace Terka
{
    using System;
    using System.Windows;

    internal partial struct Int32Vector : IEquatable<Int32Vector>
    {
        private int _x;
        private int _y;

        public Int32Vector(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        public bool Equals(Int32Vector vector)
        {
            return this == vector;
        }
        public override bool Equals(object obj)
        {
            if (obj is Int32Vector)
            {
                Int32Vector that = (Int32Vector)obj;
                return this == that;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return _x.GetHashCode() ^ _y.GetHashCode();
        }

        public static bool operator ==(Int32Vector v1, Int32Vector v2)
        {
            return v1.X == v2.X &&
                   v1.Y == v2.Y;
        }
        public static bool operator !=(Int32Vector v1, Int32Vector v2)
        {
            return !(v1 == v2);
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", _x, _y);
        }

        public static implicit operator Vector(Int32Vector @this)
        {
            return new Vector(@this.X, @this.Y);
        }

        public static explicit operator Int32Vector(Vector v)
        {
            return new Int32Vector((int)v.X, (int)v.Y);
        }
    }
}
