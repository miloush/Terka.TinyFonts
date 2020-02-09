namespace Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static partial class DWrite
    {
        /// <summary>
        /// Creates a DirectWrite factory object that is used for subsequent creation of individual DirectWrite objects.
        /// </summary>
        /// <param name="factoryType">A value that specifies whether the factory object will be shared or isolated.</param>
        /// <param name="iid">A GUID value that identifies the DirectWrite factory interface, such as __uuidof(IDWriteFactory).</param>
        /// <returns></returns>
        /// <remarks>
        /// This function creates a DirectWrite factory object that is used for subsequent creation of individual DirectWrite objects.
        /// DirectWrite factory contains internal state data such as font loader registration and cached font data.
        /// In most cases it is recommended you use the shared factory object, because it allows multiple components that use DirectWrite to share internal DirectWrite state data,
        /// and thereby reduce memory usage. However, there are cases when it is desirable to reduce the impact of a component, such as a plug-in from an untrusted source,
        /// on the rest of the process, by sandboxing and isolating it from the rest of the process components. In such cases, it is recommended you use an isolated factory for the sandboxed component.
        /// </remarks>
        [DllImport("dwrite.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        private static extern IDWriteFactory DWriteCreateFactory(FactoryType factoryType, [MarshalAs(UnmanagedType.LPStruct)] Guid iid);

        public static IDWriteFactory CreateFactory(FactoryType factoryType = FactoryType.Shared)
        {
            return DWriteCreateFactory(factoryType, new Guid(UuidOf.IDWriteFactory));
        }
    }
}
