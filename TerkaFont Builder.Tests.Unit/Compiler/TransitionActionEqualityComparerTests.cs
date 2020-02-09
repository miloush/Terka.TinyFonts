namespace Terka.FontBuilder.Compiler
{
    using System;

    using NUnit.Framework;

    using Rhino.Mocks;

    using Terka.FontBuilder.Compiler.Output;

    // ReSharper disable ReturnValueOfPureMethodIsNotUsed

    // ReSharper disable InconsistentNaming
    // ReSharper disable ObjectCreationAsStatement

    /// <summary>
    /// Tests for the <see cref="TransitionActionEqualityComparer"/> class.
    /// </summary>
    [TestFixture]
    public class TransitionActionEqualityComparerTests
    {
        /// <summary>
        /// Gets the comparer.
        /// </summary>
        /// <returns>The comparer.</returns>
        public TransitionActionEqualityComparer GetComparer()
        {
            return new TransitionActionEqualityComparer();
        }

        /// <summary>
        /// Tests that Equals returns false on two actions of different type.
        /// </summary>
        [Test]
        public void Equals_DifferentStateType_ReturnsFalse()
        {
            this.GetComparer().Equals(new SubstitutionAction(), new PositioningAdjustmentAction());
        }

        /// <summary>
        /// Tests that Equals returns false if only X is null.
        /// </summary>
        [Test]
        public void Equals_NullX_ReturnsFalse()
        {
            var stateB = new SubstitutionAction
            {
                ReplacedGlyphCount = 2,
                ReplacementGlyphIds = new ushort[] { 3, 4 }
            };

            Assert.IsFalse(this.GetComparer().Equals(null, stateB));
        }

        /// <summary>
        /// Tests that Equals returns false if only Y is null.
        /// </summary>
        [Test]
        public void Equals_NullY_ReturnsFalse()
        {
            var stateA = new SubstitutionAction
            {
                ReplacedGlyphCount = 2,
                ReplacementGlyphIds = new ushort[] { 3, 4 }
            };

            Assert.IsFalse(this.GetComparer().Equals(stateA, null));
        }

        /// <summary>
        /// Tests that Equals returns true when both values are null.
        /// </summary>
        [Test]
        public void Equals_NullXAndY_ReturnsTrue()
        {
            Assert.IsTrue(this.GetComparer().Equals(null, null));
        }

        /// <summary>
        /// Tests that Equals returns false on two actions with different replaced glyph counts but same other parameters.
        /// </summary>
        [Test]
        public void Equals_SubstitutionActionDifferentReplacedGlyphCounts_ReturnsFalse()
        {
            var comparer = this.GetComparer();

            var stateA = new SubstitutionAction
            {
                ReplacedGlyphCount = 2,
                SkippedGlyphCount = 1,
                ReplacementGlyphIds = new ushort[] { 1, 2 }
            };

            var stateB = new SubstitutionAction
            {
                ReplacedGlyphCount = 3,
                SkippedGlyphCount = 1,
                ReplacementGlyphIds = new ushort[] { 1, 2 }
            };

            var result = comparer.Equals(stateA, stateB);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that Equals returns false on two actions with different skipped glyph counts but same other parameters.
        /// </summary>
        [Test]
        public void Equals_SubstitutionActionDifferentSkippedGlyphCounts_ReturnsFalse()
        {
            var comparer = this.GetComparer();

            var stateA = new SubstitutionAction
            {
                ReplacedGlyphCount = 2,
                SkippedGlyphCount = 1,
                ReplacementGlyphIds = new ushort[] { 1, 2 }
            };

            var stateB = new SubstitutionAction
            {
                ReplacedGlyphCount = 3,
                SkippedGlyphCount = 2,
                ReplacementGlyphIds = new ushort[] { 1, 2 }
            };

            var result = comparer.Equals(stateA, stateB);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that Equals returns false on two actions with different replacement glyph IDs but same other parameters.
        /// </summary>
        [Test]
        public void Equals_SubstitutionActionDifferentReplacementGlyphIds_ReturnsFalse()
        {
            var comparer = this.GetComparer();

            var stateA = new SubstitutionAction
            {
                ReplacedGlyphCount = 2,
                SkippedGlyphCount = 1,
                ReplacementGlyphIds = new ushort[] { 1, 2 }
            };

            var stateB = new SubstitutionAction
            {
                ReplacedGlyphCount = 2,
                SkippedGlyphCount = 1,
                ReplacementGlyphIds = new ushort[] { 3, 4 }
            };

            var result = comparer.Equals(stateA, stateB);

            Assert.IsFalse(result);
        }

        /// <summary>
        /// Tests that Equals returns true on two equal actions.
        /// </summary>
        [Test]
        public void Equals_SubstitutionActionEqualActions_ReturnsTrue()
        {
            var comparer = this.GetComparer();

            var stateA = new SubstitutionAction
            {
                ReplacedGlyphCount = 2, 
                ReplacementGlyphIds = new ushort[] { 1, 2 }
            };

            var stateB = new SubstitutionAction
            {
                ReplacedGlyphCount = 2, 
                ReplacementGlyphIds = new ushort[] { 1, 2 }
            };

            var result = comparer.Equals(stateA, stateB);

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Tests that Equals returns true on two equal position adjustment actions.
        /// </summary>
        [Test]
        public void Equals_PositionAdjustmentActionEqualActions_ReturnsTrue()
        {
            var a = new PositioningAdjustmentAction
            {
                PositionChanges = new[]
                {
                    new GlyphPositionChange
                    {
                        AdvanceX = 1,
                        AdvanceY = 2
                    }
                }
            };

            var b = new PositioningAdjustmentAction
            {
                PositionChanges = new[]
                {
                    new GlyphPositionChange
                    {
                        AdvanceX = 1,
                        AdvanceY = 2
                    }
                }
            };

            Assert.IsTrue(this.GetComparer().Equals(a, b));
        }

        /// <summary>
        /// Tests that Equals returns false on two position adjustment actions with different position changes.
        /// </summary>
        [Test]
        public void Equals_PositionAdjustmentActionDifferentPositionChanges_ReturnsFalse()
        {
            var a = new PositioningAdjustmentAction
            {
                PositionChanges = new[]
                {
                    new GlyphPositionChange
                    {
                        AdvanceX = 1,
                        AdvanceY = 2
                    },
                    new GlyphPositionChange
                    {
                        OffsetX = 1,
                        OffsetY = 2
                    }
                }
            };

            var b = new PositioningAdjustmentAction
            {
                PositionChanges = new[]
                {
                    new GlyphPositionChange
                    {
                        AdvanceX = 1,
                        AdvanceY = 2
                    },
                    new GlyphPositionChange
                    {
                        OffsetX = 9,
                        OffsetY = 10
                    }
                }
            };

            Assert.IsFalse(this.GetComparer().Equals(a, b));
        }

        /// <summary>
        /// Tests that Equals returns true on two equal anchor point to anchor point actions.
        /// </summary>
        [Test]
        public void Equals_AnchorPointToAnchorPointActionEqualActions_ReturnsTrue()
        {
            var a = new AnchorPointToAnchorPointAction
            {
                CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 2 },
                PreviousGlyphAnchorPoint = new AnchorPoint { X = 3, Y = 4 },
            };

            var b = new AnchorPointToAnchorPointAction()
            {
                CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 2 },
                PreviousGlyphAnchorPoint = new AnchorPoint { X = 3, Y = 4 },
            };

            Assert.IsTrue(this.GetComparer().Equals(a, b));
        }

        /// <summary>
        /// Tests that Equals returns false on two anchor point to anchor point actions with different current glyph anchor points.
        /// </summary>
        [Test]
        public void Equals_AnchorPointToAnchorPointActionDifferentCurrentGlyphAnchorPoints_ReturnsFalse()
        {
            var a = new AnchorPointToAnchorPointAction
            {
                CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 2 },
                PreviousGlyphAnchorPoint = new AnchorPoint { X = 3, Y = 4 },
            };

            var b = new AnchorPointToAnchorPointAction
            {
                CurrentGlyphAnchorPoint = new AnchorPoint { X = 9, Y = 2 },
                PreviousGlyphAnchorPoint = new AnchorPoint { X = 3, Y = 4 },
            };

            Assert.IsFalse(this.GetComparer().Equals(a, b));
        }

        /// <summary>
        /// Tests that Equals returns false on two anchor point to anchor point actions with different previous glyph anchor points.
        /// </summary>
        [Test]
        public void Equals_AnchorPointToAnchorPointActionDifferentPreviousGlyphAnchorPoints_ReturnsFalse()
        {
            var a = new AnchorPointToAnchorPointAction
            {
                CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 2 },
                PreviousGlyphAnchorPoint = new AnchorPoint { X = 3, Y = 4 },
            };

            var b = new AnchorPointToAnchorPointAction
            {
                CurrentGlyphAnchorPoint = new AnchorPoint { X = 1, Y = 2 },
                PreviousGlyphAnchorPoint = new AnchorPoint { X = 9, Y = 4 },
            };

            Assert.IsFalse(this.GetComparer().Equals(a, b));
        }

        /// <summary>
        /// Tests that Equals throws exception when given unknown action type.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Equals_UnknownActionType_ThrowsException()
        {
            var a = MockRepository.GenerateMock<ITransitionAction>();
            var b = MockRepository.GenerateMock<ITransitionAction>();

            Assert.IsFalse(this.GetComparer().Equals(a, b));
        }
    }
}