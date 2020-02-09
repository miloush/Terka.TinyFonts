namespace Terka.TinyResources
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Represents a Tiny Resource file.
    /// </summary>
    public partial class TinyResourcesFile : List<TinyResourcesFile.Resource>
    {
        private FileHeader _header;

        /// <summary>
        /// Gets or sets the Tiny resource file header.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
        public FileHeader Header
        {
            get { return _header; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _header = value;
            }
        }
        /// <summary>
        /// Gets a collection of resources in the file.
        /// </summary>
        public IList<TinyResourcesFile.Resource> Resources
        {
            get { return this; }
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="TinyResourcesFile"/>.
        /// </summary>
        public TinyResourcesFile()
        {
            _header = new FileHeader(0);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TinyResourcesFile"/> from an existing file.
        /// </summary>
        /// <param name="path">The file to be loaded.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TinyResourcesFile(string path)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
                ReadFrom(reader);
        }
        /// <summary>
        /// Creates a new instance of the <see cref="TinyResourcesFile"/> from a collection of resources.
        /// </summary>
        /// <param name="resources">The collection of resources to create the file from.</param>
        public TinyResourcesFile(IEnumerable<TinyResourcesFile.Resource> resources) : base(resources)
        {
            _header = new FileHeader((uint)Count);
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

            _header = new FileHeader();
            _header.ReadFrom(reader);

            Clear();
            Capacity = (int)_header.NumberOfResoruces;
            for (int i = 0; i < _header.NumberOfResoruces; i++)
            {
                Resource resource = new Resource();
                resource.ReadFrom(reader, _header);

                Add(resource);
            }
        }
        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Header"/> is null.</exception>
        /// <remarks>
        /// This method does not ensure the serialized data are valid Tiny Resource.
        /// Use <see cref="Update"/> method to ensure valid output.
        /// </remarks>
        public virtual void WriteTo(BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException();
            if (_header == null)
                throw new InvalidOperationException();

            _header.WriteTo(writer);
            for (int i = 0; i < Count; i++)
                this[i].WriteTo(writer);
        }

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Header"/> is null.</exception>
        public virtual void Update()
        {
            if (_header == null)
                throw new InvalidOperationException();

            _header.NumberOfResoruces = (uint)Count;
            _header.Update();

            for (int i = 0; i < Count; i++)
                this[i].Update();
        }

        /// <summary>
        /// Saves the Tiny Resource to a file.
        /// </summary>
        /// <param name="path">The file to save to.</param>
        /// <remarks>
        /// This method ensures the written data are valid using the <see cref="Update"/> method.
        /// </remarks>
        public void Save(string path)
        {
            using (Stream stream = File.Create(path))
                Save(stream);
        }
        /// <summary>
        /// Saves the Tiny Resource to a stream and closes it.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        /// <remarks>
        /// This method ensures the written data are valid using the <see cref="Update"/> method.
        /// </remarks>
        public void Save(Stream stream)
        {
            Update();

            using (BinaryWriter writer = new BinaryWriter(stream))
                WriteTo(writer);
        }
    }
}
