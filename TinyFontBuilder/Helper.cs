using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Terka
{
    internal static class Helper
    {
        internal static int Get(this BitArray bitArray, int index, int length)
        {
            int value = 0;

            for (int i = 0; i < length; i++, value <<= 1)
                if (bitArray[index + i])
                    value |= 1;

            return value;
        }

        internal static void Set(this BitArray bitArray, int index, int value, int length)
        {
            for (int i = length - 1; i >= 0; i--, value >>= 1)
                bitArray[index + i] = (value & 1) == 0; 
        }

        internal static void Append(this BitArray bitArray, bool value)
        {
            Contract.Requires(bitArray != null, "Bit array cannot be null.");

            int length = bitArray.Length;

            bitArray.Length = length + 1;
            if (value)
                bitArray[length] = true;
        }
        internal static void ApendMsb(this BitArray bitArray, int value, int validBits)
        {
            Contract.Requires(bitArray != null, "Bit array cannot be null.");
            Contract.Requires(value < (1 << validBits), "The value takes more than valid number of bits.");

            bitArray.Length += validBits;

            int length = bitArray.Length;
            for (int i = 0; i < validBits; i++, value >>= 1)
                if ((value & 1) != 0)
                    bitArray[length - i - 1] = true;
        }
        internal static void AppendLsb(this BitArray bitArray, int value, int validBits)
        {
            Contract.Requires(bitArray != null, "Bit array cannot be null.");
            Contract.Requires(value < (1 << validBits), "The value takes more than valid number of bits.");

            int length = bitArray.Length;
            bitArray.Length += validBits;

            for (int i = 0; i < validBits; i++, value >>= 1)
                if ((value & 1) != 0)
                    bitArray[length + i] = true;
        }
        internal static void Add(this BitArray bitArray, BitArray bits)
        {
            Contract.Requires(bitArray != null, "Bit array cannot be null.");

            if (bits == null)
                return;

            int startIndex = bitArray.Length;
            bitArray.Length += bits.Length;

            for (int i = 0; i < bits.Length; i++)
                bitArray[startIndex + i] = bits[i];
        }

        internal static int UnpackFlags(int mask, int shift, int flags)
        {
            return (flags & mask) >> shift;
        }
        internal static int PackFlags(int mask, int shift, int flags, int value)
        {
            value <<= shift;
            if ((value & ~mask) != 0)
                throw new ArgumentOutOfRangeException("value");

            return (flags & ~mask) | value;
        }

        internal static int SetFlag(int flags, int flag, bool value)
        {
            if (value)
                return flags | flag;
            else
                return flags & ~flag;
        }

        internal static T Check<T>(T required, T value) where T : IEquatable<T>
        {
            if (!required.Equals(value))
                throw new SerializationException();

            return value;
        }
        internal static T CheckAtLeast<T>(T required, T value) where T : IComparable<T>
        {
            if (required.CompareTo(value) > 0)
                throw new SerializationException();

            return value;
        }

        internal static sbyte FitIntoInt8(int value, TraceSource trace)
        {
            if (value <= sbyte.MinValue)
            {
                if (trace != null)
                    trace.TraceEvent(TraceEventType.Warning, 0, "Value will be truncated to Int8: {0}", value);

                return sbyte.MinValue;
            }

            if (value >= sbyte.MaxValue)
            {
                if (trace != null)
                    trace.TraceEvent(TraceEventType.Warning, 0, "Value will be truncated to Int8: {0}", value);

                return sbyte.MaxValue;
            }

            return (sbyte)value;
        }
        internal static short FitIntoInt16(int value, TraceSource trace)
        {
            if (value <= short.MinValue)
            {
                if (trace != null)
                    trace.TraceEvent(TraceEventType.Warning, 0, "Value will be truncated to Int16: {0}", value);

                return short.MinValue;
            }

            if (value >= short.MaxValue)
            {
                if (trace != null)
                    trace.TraceEvent(TraceEventType.Warning, 0, "Value will be truncated to Int16: {0}", value);

                return short.MaxValue;
            }

            return (short)value;
        }

        internal static byte ReverseBits(byte value)
        {
            return (byte)((value * 0x0202020202UL & 0x010884422010UL) % 1023);
        }

        internal static IEnumerable<int> ToCodePoints(this string s)
        {
            Contract.Requires(s != null, "String cannot be null.");

            for (int i = 0; i < s.Length; i++)
            {
                int codepoint = char.ConvertToUtf32(s, i);
                if (codepoint > char.MaxValue)
                    i++;

                yield return codepoint;
            }
        }

        internal static bool ReadBit(byte[] array, long bitNumber)
        {
            long byteIndex = bitNumber / 8;
            int bitMask = 1 << (int)(bitNumber % 8);

            return (array[byteIndex] & bitMask) != 0;
        }
        internal static void WriteBit(byte[] array, long bitNumber, bool value)
        {
            long byteIndex = bitNumber / 8;
            int bitMask = 1 << (int)(bitNumber % 8);

            int data = array[byteIndex];

            if (value)
                data |= bitMask;
            else
                data &= ~bitMask;

            array[byteIndex] = (byte)data;
        }

        internal static bool IsSorted<T>(IList<T> list, int offset, int count, IComparer<T> comparer)
        {
            for (int i = 0; i < count - 1; i++)
            {
                if (comparer.Compare(list[offset + i], list[offset + i + 1]) > 0)
                    return false;
            }

            return true;
        }
    }
}
