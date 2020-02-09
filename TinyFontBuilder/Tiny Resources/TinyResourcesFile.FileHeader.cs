namespace Terka.TinyResources
{
    using System;
    using System.IO;

    partial class TinyResourcesFile
    {
        /// <summary>
        /// Tiny Resource file header.
        /// </summary>
        public class FileHeader
        {
            internal const uint RequiredMagicNumber = 0xF995B0A8;
            internal const uint RequiredVersion = 2;
            internal const uint RequiredSize = 5 * sizeof(uint);

            private uint _magicNumber;
            private uint _version;
            private uint _sizeOfHeader;
            private uint _sizeOfResourceHeader;
            private uint _numberOfResources;

            /// <summary>
            /// Always A8 B0 95 F9.
            /// </summary>
            public uint MagicNumber
            {
                get { return _magicNumber; }
                set { _magicNumber = value; }
            }
            /// <summary>
            /// Version of the Tiny Resource format.
            /// </summary>
            public uint Version
            {
                get { return _version; }
                set { _version = value; }
            }
            /// <summary>
            /// Size of this header.
            /// </summary>
            public uint SizeOfHeader
            {
                get { return _sizeOfHeader; }
                set { _sizeOfHeader = value; }
            }
            /// <summary>
            /// Size of data resources header.
            /// </summary>
            public uint SizeOfResourceHeader
            {
                get { return _sizeOfResourceHeader; }
                set { _sizeOfResourceHeader = value; }
            }
            /// <summary>
            /// Number of resources in the file.
            /// </summary>
            public uint NumberOfResoruces
            {
                get { return _numberOfResources; }
                set { _numberOfResources = value; }
            }

            /// <summary>
            /// Creates a blank instance of the <see cref="FileHeader"/> class.
            /// </summary>
            public FileHeader()
            {

            }
            /// <summary>
            /// Creates a new instance of the <see cref="FileHeader"/> class with required values.
            /// </summary>
            /// <param name="numberOfResources"></param>
            public FileHeader(uint numberOfResources)
            {
                _magicNumber = RequiredMagicNumber;
                _version = RequiredVersion;
                _sizeOfHeader = RequiredSize;
                _sizeOfResourceHeader = ResourceHeader.RequiredSize;
                _numberOfResources = numberOfResources;
            }

            /// <summary>
            /// Deserializes this structure from binary data.
            /// </summary>
            /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
            /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
            public virtual void ReadFrom(BinaryReader reader)
            {
                if (reader == null)
                    throw new ArgumentNullException("reader");

                _magicNumber = reader.ReadUInt32();
                Helper.Check(RequiredMagicNumber, _magicNumber);

                _version = reader.ReadUInt32();
                Helper.CheckAtLeast(RequiredVersion, _version);
                    
                _sizeOfHeader = reader.ReadUInt32();
                Helper.CheckAtLeast(RequiredSize, _sizeOfHeader);

                _sizeOfResourceHeader = reader.ReadUInt32();
                _numberOfResources = reader.ReadUInt32();

                reader.ReadBytes((int)(_sizeOfHeader - RequiredSize));
            }
            /// <summary>
            /// Serializes this structure into binary data.
            /// </summary>
            /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
            /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
            public virtual void WriteTo(BinaryWriter writer)
            {
                if (writer == null)
                    throw new ArgumentNullException("writer");

                writer.Write(_magicNumber);
                writer.Write(_version);
                writer.Write(_sizeOfHeader);
                writer.Write(_sizeOfResourceHeader);
                writer.Write(_numberOfResources);
            }

            /// <summary>
            /// Ensures this structure contains valid data.
            /// </summary>
            public virtual void Update()
            {
                _magicNumber = RequiredMagicNumber;
                
                if (_version < RequiredVersion)
                    _version = RequiredVersion;

                if (_sizeOfHeader < RequiredSize)
                    _sizeOfHeader = RequiredSize;

                if (_sizeOfResourceHeader < ResourceHeader.RequiredSize)
                    _sizeOfResourceHeader = ResourceHeader.RequiredSize;
            }
        }
    }
}
