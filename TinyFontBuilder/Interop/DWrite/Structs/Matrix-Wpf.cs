namespace Win32
{
    using System.Runtime.InteropServices;

    partial class DWrite
    {
        partial struct Matrix
        {
            public static explicit operator Matrix(System.Windows.Media.Matrix m)
            {
                return new Matrix((float)m.M11, (float)m.M12, (float)m.M21, (float)m.M22, (float)m.OffsetX, (float)m.OffsetY);
            }

            public static explicit operator System.Windows.Media.Matrix(Matrix m)
            {
                return new System.Windows.Media.Matrix(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
            }
        };
    }
}
