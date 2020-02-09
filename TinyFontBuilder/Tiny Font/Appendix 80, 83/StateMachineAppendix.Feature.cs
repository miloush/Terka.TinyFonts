namespace Terka.TinyFonts
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    partial class StateMachineAppendix
    {
        /// <summary>
        /// Represents one feature.
        /// </summary>
        public class Feature
        {
            private ushort _stateCount;
            private SentinelCollection<ushort> _stateOffsets;
            private List<Rule> _rules;

            /// <summary>
            /// Gets or sets the number of states, includes the initial one.
            /// </summary>
            public ushort StateCount
            {
                get { return _stateCount; }
                set { _stateCount = value; }
            }

            /// <summary>
            /// Gets offets of states.
            /// </summary>
            public SentinelCollection<ushort> StateOffsets
            {
                get { return _stateOffsets; }
            }

            /// <summary>
            /// Gets rules for state machine.
            /// </summary>
            public IList<Rule> Rules
            {
                get { return _rules; }
            }

            /// <summary>
            /// Creates new instance of feature.
            /// </summary>
            public Feature()
            {
                _stateOffsets = new SentinelCollection<ushort>();
                _rules = new List<Rule>();
            }

            /// <summary>
            /// Deserializes this structure from binary data.
            /// </summary>
            /// <param name="reader">The <see cref="BinaryReader"/> to read the data from.</param>
            public void ReadFrom(BinaryReader reader)
            {
                _stateCount = reader.ReadUInt16();

                for (int i = 0; i < _stateCount; i++)
                    _stateOffsets.Add(reader.ReadUInt16());
                
                _stateOffsets.Sentinel = reader.ReadUInt16();

                if (_stateCount % 2 == 1)
                    reader.ReadUInt16();

                int ruleCount = _stateOffsets.Sentinel;
                for (int i = 0; i < ruleCount; i++)
                {
                    Rule rule = new Rule();
                    rule.ReadFrom(reader);

                    _rules.Add(rule);
                }
            }

            /// <summary>
            /// Serializes this structure into binary data.
            /// </summary>
            /// <param name="writer">The <see cref="BinaryWriter"/> to write the data to.</param>
            public void WriteTo(BinaryWriter writer)
            {
                writer.Write(_stateCount);

                for (int i = 0; i < _stateOffsets.Count; i++)
                    writer.Write(_stateOffsets[i]);

                if (_stateOffsets.Count % 2 == 0)
                    writer.Write((ushort)0);

                for (int i = 0; i < _rules.Count; i++)
                    _rules[i].WriteTo(writer);
            }

            /// <summary>
            /// Ensures this structure contains valid data.
            /// </summary>
            public void Update()
            {
                _stateCount = 0;
                _stateOffsets.Clear();
                _stateOffsets.Sentinel = 0;

                if (_rules.Count > 0)
                {
                    // It is critical for the sort algorithm to be stable, List.Sort resp. Array.Sort is not.
                    Rule[] sortedRules = Enumerable.OrderBy(_rules, r => r.RequiredState).ToArray();
                    _rules.Clear();
                    _rules.AddRange(sortedRules);

                    Dictionary<ushort, ushort> stateMapping = new Dictionary<ushort, ushort>(_stateOffsets.Count);
                    ushort lastUsedState = ushort.MaxValue;
                    ushort lastRequiredState = (ushort)~_rules[0].RequiredState;

                    UpdateAssert(_rules.Count <= ushort.MaxValue, "Too many rules.");
                    _stateOffsets.Sentinel = (ushort)_rules.Count;

                    for (ushort i = 0; i < _rules.Count; i++)
                        if (_rules[i].RequiredState != lastRequiredState)
                        {
                            lastRequiredState = _rules[i].RequiredState;
                            _rules[i].RequiredState = ++lastUsedState;

                            _stateOffsets.Add(i);

                            stateMapping[lastRequiredState] = _rules[i].RequiredState;
                        }
                        else
                            _rules[i].RequiredState = lastUsedState;

                    _stateCount = ++lastUsedState;
                    foreach (Rule rule in _rules)
                        if (stateMapping.ContainsKey(rule.NewState))
                            rule.NewState = stateMapping[rule.NewState];
                        else
                            rule.NewState = 0;
                }
            }

            internal int GetSize()
            {
                int stateSize = (sizeof(ushort) + sizeof(ushort) * (_stateCount + 1 + _stateCount % 2));
                int ruleSize = _rules.Count * Rule.SizeOf;

                return stateSize + ruleSize;
            }
        }
    }
}
