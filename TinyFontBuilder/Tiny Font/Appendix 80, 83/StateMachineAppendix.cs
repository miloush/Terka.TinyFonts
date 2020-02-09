namespace Terka.TinyFonts
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    /// Base class for state machine based appendicies.
    /// </summary>
    public abstract partial class StateMachineAppendix : FontAppendix
    {
        private ushort _featureCount;
        private ushort _heapSize;
        private SentinelCollection<FeatureOffset> _featureOffsets;
        private SentinelCollection<Feature> _features;
        private byte[] _parametersHeap;

        /// <summary>
        /// Gets total size in bytes of this appendix.
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public override int GetSize(TinyFont font)
        {
                return sizeof(ushort) + sizeof(ushort) +
                       _featureOffsets.Count * FeatureOffset.SizeOf +
                       _features.Sum(f => f.GetSize()) +
                       (_parametersHeap == null ?  0 : _parametersHeap.Length);
        }

        /// <summary>
        /// Gets if this appendix has any valid content.
        /// </summary>
        public override bool HasContent
        {
            get { return _features.Sum(f => f.Rules.Count) > 0; }
        }

        /// <summary>
        /// Gets or sets how many features are stored in this appendix.
        /// </summary>
        public ushort FeatureCount
        {
            get { return _featureCount; }
            set { _featureCount = value; }
        }
        /// <summary>
        /// Gets or sets size of parameters heap in bytes.
        /// </summary>
        public ushort ParametersHeapSize
        {
            get { return _heapSize; }
            set { _heapSize = value; }
        }
        /// <summary>
        /// Gets feature offsets collection stored in this appendix.
        /// </summary>
        public SentinelCollection<FeatureOffset> FeatureOffsets
        {
            get { return _featureOffsets; }
        }
        /// <summary>
        /// Gets features collection stored in this appendix.
        /// </summary>
        public SentinelCollection<Feature> Features
        {
            get { return _features; }
        }
        /// <summary>
        /// Gets or sets heap for feature's parameters.
        /// </summary>
        public byte[] ParametersHeap
        {
            get { return _parametersHeap; }
            set { _parametersHeap = value; }
        }

        /// <summary>
        /// Creates a new instance of state machine appendix by its ID.
        /// </summary>
        /// <param name="id">Font appendix ID,</param>
        protected StateMachineAppendix(byte id)
            : base(id)
        {
            _featureOffsets = new SentinelCollection<FeatureOffset>();
            _featureOffsets.Sentinel = new FeatureOffset();
            _featureOffsets.Sentinel.Tag = 0xFFFFFFFF;

            _features = new SentinelCollection<Feature>();
            _features.Sentinel = new Feature();
        }

        /// <summary>
        /// Deserializes this structure from binary data.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
        /// <param name="font">Tiny Font containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is null.</exception>
        public override void ReadFrom(BinaryReader reader, TinyFont font)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            _featureCount = reader.ReadUInt16();
            _heapSize = reader.ReadUInt16();

            _featureOffsets.Clear();
            _featureOffsets.Capacity = _featureCount;

            for (int i = 0; i < _featureCount; i++)
            {
                FeatureOffset offset = new FeatureOffset();
                offset.ReadFrom(reader);

                _featureOffsets.Add(offset);
            }

            _featureOffsets.Sentinel.ReadFrom(reader);

            _features.Clear();
            _features.Capacity = _featureCount;

            for (int i = 0; i < _featureCount; i++)
            {
                Feature feature = new Feature();
                feature.ReadFrom(reader);

                _features.Add(feature);
            }

            _features.Sentinel.ReadFrom(reader);

            if (_heapSize > 0)
                _parametersHeap = reader.ReadBytes(_heapSize);
        }

        /// <summary>
        /// Serializes this structure into binary data.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
        /// <param name="font">Tiny Font containing this appendix.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public override void WriteTo(BinaryWriter writer, TinyFont font)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.Write(_featureCount);
            writer.Write(_heapSize);

            foreach (FeatureOffset featureOffset in _featureOffsets)
                featureOffset.WriteTo(writer);

            foreach (Feature feature in _features)
                feature.WriteTo(writer);

            if (_parametersHeap != null)
                writer.Write(_parametersHeap);
        }

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        /// <param name="font">Tiny Font containing this appendix.</param>
        public override void Update(TinyFont font)
        {
            UpdateAssert(_features.ItemsCount <= ushort.MaxValue, "Too many features.");
            UpdateAssert(_features.ItemsCount == _featureOffsets.ItemsCount, "Feature offset count mismatch.");
            UpdateAssert(_parametersHeap == null || _parametersHeap.Length <= ushort.MaxValue, "The parameters heap is too large.");

            _featureCount = (ushort)_features.ItemsCount;
            _heapSize = (ushort)(_parametersHeap == null ? 0 : _parametersHeap.Length);

            for (int i = 0; i < _features.ItemsCount; i++)
                _features[i].Update();

            _features.Sentinel.StateCount = 0;
            _features.Sentinel.Rules.Clear();
            _features.Sentinel.StateOffsets.Clear();
            _features.Sentinel.StateOffsets.Sentinel = 0;

            SortedDictionary<FeatureOffset, Feature> sortedFeatures = new SortedDictionary<FeatureOffset, Feature>();
            for (int i = 0; i < _featureCount; i++)
                sortedFeatures.Add(_featureOffsets[i], _features[i]);
           
            _featureOffsets.Clear();
            _featureOffsets.AddRange(sortedFeatures.Keys);

            _features.Clear();
            _features.AddRange(sortedFeatures.Values);

            int offset = 0;
            for (int i = 0; i < _features.Count; i++)
            {
                UpdateAssert(offset <= ushort.MaxValue, "Too large feature.");

                _featureOffsets[i].Offset = (ushort)offset;
                offset += _features[i].GetSize();
            }

            _featureOffsets.Sentinel.Tag = 0xFFFFFFFF;
        }

        private static void UpdateAssert(bool condition, string error)
        {
            if (condition == false)
                throw new InvalidOperationException(error);
        }

        /// <summary>
        /// Appends parameters to heap. If heap does not exists, new is created.
        /// </summary>
        /// <param name="parameters">Parameters to store on heap.</param>
        /// <returns>Zero-based offset to heap, where appended data begins.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameters"/> are null.</exception>
        public ushort AppendParameters(HeapParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            parameters.Update();

            int size = parameters.GetSize();
            ushort offset = 0;

            if (_parametersHeap == null)
                _parametersHeap = new byte[size];

            else
            {
                if (_parametersHeap.Length > ushort.MaxValue)
                    throw new InvalidOperationException("The parameters heap is too large.");

                offset = (ushort)_parametersHeap.Length;

                byte[] newHeap = new byte[_parametersHeap.Length + size];
                _parametersHeap.CopyTo(newHeap, 0);
                _parametersHeap = newHeap;
            }

            parameters.WriteTo(_parametersHeap, offset);

            return offset;
        }
    }
}
