namespace Terka.TinyResources
{
    using System;
    using System.IO;

    public partial class TinyResourcesFile
    {
        /// <summary>
        /// Represents a single Tiny Resource.
        /// </summary>
        public class Resource
        {
            private ResourceHeader _header;
            private byte[] _data;

            /// <summary>
            /// Gets or sets the resource header.
            /// </summary>
            public ResourceHeader Header
            {
                get { return _header; }
                set { _header = value; }
            }
            /// <summary>
            /// Gets or sets the resource data.
            /// </summary>
            public byte[] Data
            {
                get { return _data; }
                set { _data = value; }
            }

            /// <summary>
            /// Creates a blank instance of <see cref="Resource"/>.
            /// /// </summary>
            public Resource()
            {

            }
            /// <summary>
            /// Creates a new instance of <see cref="Resource"/> with specified <see cref="TinyResourceKind"/> and existing data.
            /// </summary>
            /// <param name="kind"></param>
            /// <param name="data"></param>
            public Resource(TinyResourceKind kind, byte[] data)
            {
                if (data == null)
                    throw new ArgumentNullException("data");

                _header = new ResourceHeader(kind);
                _header.Size = (uint)data.Length;

                _data = data;
            }
            /// <summary>
            /// Creates a new instance of <see cref="Resource"/> with existing <see cref="ResourceHeader"/> and data.
            /// </summary>
            /// <param name="header"></param>
            /// <param name="data"></param>
            public Resource(ResourceHeader header, byte[] data)
            {
                if (header == null)
                    throw new ArgumentNullException("header");
                if (data == null)
                    throw new ArgumentNullException("data");
                
                _header = header;
                _data = data;
            }

            /// <summary>
            /// Deserializes this structure from binary data.
            /// </summary>
            /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
            /// <param name="header">Optional <see cref="FileHeader"/> that specifies padding to skip after reading.</param>
            /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
            public virtual void ReadFrom(BinaryReader reader, FileHeader header = null)
            {
                if (reader == null)
                    throw new ArgumentNullException("reader");

                _header = new ResourceHeader();
                _header.ReadFrom(reader, header);

                _data = reader.ReadBytes(checked((int)_header.Size));
            }

            /// <summary>
            /// Serializes this structure into binary data.
            /// </summary>
            /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
            /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
            /// <exception cref="InvalidOperationException"><see cref="Header"/> is null.</exception>
            public virtual void WriteTo(BinaryWriter writer)
            {
                if (writer == null)
                    throw new ArgumentNullException("writer");
                if (_header == null)
                    throw new InvalidOperationException();

                _header.WriteTo(writer);

                if (_data != null)
                    writer.Write(_data);
            }

            /// <summary>
            /// Ensures this structure contains valid data.
            /// </summary>
            /// <exception cref="InvalidOperationException"><see cref="Header"/> is null.</exception>
            public virtual void Update()
            {
                if (_header == null)
                    throw new InvalidOperationException();

                _header.Size = _data == null ? 0 : (uint)_data.Length;
                _header.Update();
            }
        }
    }
}
