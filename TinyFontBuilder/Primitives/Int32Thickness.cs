namespace Terka
{
    using System;
    using System.Windows;

    internal struct Int32Thickness : IEquatable<Int32Thickness>
    {
        private int _left;
        private int _top;
        private int _right;
        private int _bottom;

        public Int32Thickness(int uniformLength)
        {
            _left = _top = _right = _bottom = uniformLength;
        }
        public Int32Thickness(int left, int top, int right, int bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        public int Left { get { return _left; } set { _left = value; } }
        public int Top { get { return _top; } set { _top = value; } }
        public int Right { get { return _right; } set { _right = value; } }
        public int Bottom { get { return _bottom; } set { _bottom = value; } }

        public bool Equals(Int32Thickness thickness)
        {
            return this == thickness;
        }
        public override bool Equals(object obj)
        {
            if (obj is Int32Thickness)
            {
                Int32Thickness that = (Int32Thickness)obj;
                return this == that;
            }

            return false;
        }
        public override int GetHashCode()
        {
            return _left.GetHashCode() ^ _top.GetHashCode() ^ _right.GetHashCode() ^ _bottom.GetHashCode();
        }

        public static bool operator ==(Int32Thickness t1, Int32Thickness t2)
        {
            return
                t1._left == t2._left &&
                t1._top == t2._top &&
                t1._right == t2._right &&
                t1._bottom == t2._bottom;
        }
        public static bool operator !=(Int32Thickness t1, Int32Thickness t2)
        {
            return !(t1 == t2);
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", _left, _top, _right, _bottom);
        }

        public static implicit operator Thickness(Int32Thickness @this)
        {
            return new Thickness(@this.Left, @this.Top, @this.Right, @this.Bottom);
        }
        public static explicit operator Int32Thickness(Thickness t)
        {
            return new Int32Thickness((int)t.Left, (int)t.Top, (int)t.Right, (int)t.Bottom);
        }
    }
}
