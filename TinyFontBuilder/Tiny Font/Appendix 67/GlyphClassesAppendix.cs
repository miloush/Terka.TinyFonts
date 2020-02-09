namespace Terka.TinyFonts
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    /// Font appendix for glyph classes.
    /// </summary>
    public partial class GlyphClassesAppendix : FontAppendix
    {
        private ushort _coverageCount;
        private ushort _coverageGlyphCount;
        private List<Coverage> _coverages;
        private List<int> _coverageGlyphs;

        /// <summary>
        /// Gets size of this appendix.
        /// </summary>
        /// <param name="font">Tiny Font containing this appendix.</param>
        /// <returns>Size in bytes.</returns>
        public override int GetSize(TinyFont font)
        {
                return sizeof(ushort) + sizeof(ushort) +
                       _coverages.Count * Coverage.SizeOf +
                       _coverageGlyphCount * sizeof(int);
                       //sizeof(ushort) * (_coverageGlyphCount % 2);
        }

        /// <summary>
        /// Gets if appendix has any valid content.
        /// </summary>
        public override bool HasContent
        {
            get
            {
                return _coverages.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets count of coverages in this appendix.
        /// </summary>
        public ushort CoverageCount
        {
            get { return _coverageCount; }
            set { _coverageCount = value; }
        }
        /// <summary>
        /// Gets or sets total count of glyphs accros all coverages in this appendix.
        /// </summary>
        public ushort CoverageGlyphCount
        {
            get { return _coverageGlyphCount; }
            set { _coverageGlyphCount = value; }
        }

        /// <summary>
        /// Gets coverages collection.
        /// </summary>
        public IList<Coverage> Coverages
        {
            get { return _coverages; }
        }
        /// <summary>
        /// Gets coverage glyphs collection.
        /// </summary>
        public IList<int> CoverageGlyphs
        {
            get { return _coverageGlyphs; }
        }


        /// <summary>
        /// Creates new instance of appendix.
        /// </summary>
        public GlyphClassesAppendix() : base(GlyphClasses)
        {
            _coverages = new List<Coverage>();
            _coverageGlyphs = new List<int>();
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

            _coverageCount = reader.ReadUInt16();
            _coverageGlyphCount = reader.ReadUInt16();

            _coverages.Clear();
            _coverages.Capacity = _coverageCount;

            for (int i = 0; i < _coverageCount; i++)
            {
                Coverage coverage = new Coverage();
                coverage.ReadFrom(reader);

                _coverages.Add(coverage);
            }

            _coverageGlyphs.Clear();
            _coverageGlyphs.Capacity = _coverageGlyphCount;

            for (int i = 0; i < _coverageGlyphCount; i++)
                CoverageGlyphs.Add(reader.ReadInt32());

            //if (_coverageGlyphCount % 2 == 1)
            //    reader.ReadUInt16();
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

            writer.Write(_coverageCount);
            writer.Write(_coverageGlyphCount);

            for (int i = 0; i < _coverages.Count; i++)
                _coverages[i].WriteTo(writer);

            for (int i = 0; i < _coverageGlyphs.Count; i++)
                writer.Write(_coverageGlyphs[i]);

            //if (_coverageGlyphCount % 2 == 1)
            //    writer.Write((ushort)0);
        }

        /// <summary>
        /// Ensures this structure contains valid data.
        /// </summary>
        /// <param name="font">Tiny Font containing this appendix.</param>
        public override void Update(TinyFont font)
        {
            UpdateAssert(_coverages.Count <= ushort.MaxValue, "Too many coverages.");
            UpdateAssert(_coverageGlyphs.Count <= ushort.MaxValue, "Too many coverage glyphs.");

            _coverageCount = (ushort)_coverages.Count;
            _coverageGlyphCount = (ushort)_coverageGlyphs.Count;

            foreach (Coverage coverage in _coverages)
            {
                UpdateAssert(coverage.Offset + coverage.Count <= _coverageGlyphs.Count, "Out of range coverage.");
                _coverageGlyphs.Sort(coverage.Offset, coverage.Count, Comparer<int>.Default);
            }

            foreach (Coverage coverage in _coverages)
                UpdateAssert(Helper.IsSorted(_coverageGlyphs, coverage.Offset, coverage.Count, Comparer<int>.Default), "Incompatible overlapping coverages.");
        }
        private static void UpdateAssert(bool condition, string error)
        {
            if (condition == false)
                throw new InvalidOperationException(error);
        }

        /// <summary>
        /// Finds an existing coverage for given sequence of glyphs.
        /// </summary>
        /// <param name="glyphs">The sequnce of glyphs to look for.</param>
        /// <returns>An existing <see cref="Coverage" /> containing <paramref name="glyphs"/> or null if no such coverage exists.</returns>
        public Coverage FindCoverage(params int[] glyphs)
        {
            if (glyphs == null || glyphs.Length < 1)
                return new Coverage(0, 0);

            Array.Sort(glyphs);

            int firstIndex = _coverageGlyphs.IndexOf(glyphs[0]);
            while (firstIndex >= 0)
            {
                if (firstIndex + glyphs.Length > _coverageGlyphs.Count)
                    return null;

                bool match = true;
                for (int i = 1; i < glyphs.Length; i++)
                    if (glyphs[i] != _coverageGlyphs[firstIndex + i])
                    {
                        match = false;
                        break;
                    }

                if (match)
                    return new Coverage((ushort)firstIndex, (ushort)glyphs.Length);

                firstIndex = _coverageGlyphs.IndexOf(glyphs[0], firstIndex + 1);
            }

            return null;
        }
        /// <summary>
        /// Append new glyph <see cref="Coverage"/> to this appendix.
        /// </summary>
        /// <param name="glyphs">The sequnce of glyphs for coverage.</param>
        /// <returns>Created <see cref="Coverage"/>.</returns>
        public Coverage AppendCoverage(params int[] glyphs)
        {
            Coverage coverage;

            if (glyphs == null || glyphs.Length < 1)
                coverage = new Coverage(0, 0);
            else
            {
                Array.Sort(glyphs);

                coverage = new Coverage((ushort)_coverageGlyphs.Count, (ushort)glyphs.Length);
                _coverageGlyphs.AddRange(glyphs);
            }

            _coverages.Add(coverage);

            return coverage;
        }
    }
}
