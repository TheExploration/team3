using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.CodeGenerating;
using System.Runtime.CompilerServices;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Serializing.Helping;
using GameKit.Dependencies.Utilities;
using UnityEngine;

namespace FishNet.Serializing
{
    public partial class Writer
    {
        /// <summary>
        /// Used to insert length for delta flags.
        /// </summary>
        private ReservedLengthWriter _reservedLengthWriter = new();

        private const double LARGEST_DELTA_PRECISION_INT8 = (sbyte.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_INT16 = (short.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_INT32 = (int.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_INT64 = (long.MaxValue / DOUBLE_ACCURACY);

        private const double LARGEST_DELTA_PRECISION_UINT8 = (byte.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT16 = (ushort.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT32 = (uint.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT64 = (ulong.MaxValue / DOUBLE_ACCURACY);
        internal const double DOUBLE_ACCURACY = 1000d;
        internal const double DOUBLE_ACCURACY_PRECISION = (1f / DOUBLE_ACCURACY);
        internal const decimal DECIMAL_ACCURACY = 1000m;

        internal const byte VECTOR2_FLAG_BYTES = 1;
        internal const byte VECTOR3_FLAG_BYTES = 1;

        #region Other.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaBoolean(bool valueA, bool valueB)
        {
            return (valueA != valueB);
        }
        #endregion

        #region Whole values.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaInt8(sbyte valueA, sbyte valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        public bool WriteDeltaUInt8(byte valueA, byte valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaInt16(short valueA, short valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaUInt16(ushort valueA, ushort valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaInt32(int valueA, int valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaUInt32(uint valueA, uint valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaInt64(long valueA, long valueB) => WriteDeltaUInt64((ulong)valueA, (ulong)valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaUInt64(ulong valueA, ulong valueB)
        {
            if (valueA == valueB) return false;

            bool bLargerThanA = (valueB > valueA);
            ulong next = (bLargerThanA) ? (valueB - valueA) : (valueA - valueB);

            WriteBoolean(bLargerThanA);
            WriteUnsignedPackedWhole(next);

            return true;
        }

        /// <summary>
        /// Writes the difference between two values for signed and unsigned shorts and ints.
        /// </summary>
        private bool WriteDifference8_16_32(long valueA, long valueB)
        {
            if (valueA == valueB) return false;

            long next = (valueB - valueA);
            WriteSignedPackedWhole(next);

            return true;
        }
        #endregion

        #region Single.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaSingle(float valueA, float valueB)
        {
            UDeltaPrecisionType dpt = GetUDeltaPrecisionType(valueA, valueB, out float unsignedDifference);

            if (dpt == UDeltaPrecisionType.Unset) return false;

            WriteUInt8Unpacked((byte)dpt);
            WriteDeltaSingle(dpt, unsignedDifference, unsigned: true);

            return true;
        }

        /// <summary>
        /// Writes a delta value using a compression type.
        /// </summary>
        private void WriteDeltaSingle(UDeltaPrecisionType dpt, float value, bool unsigned)
        {
            if (dpt.FastContains(UDeltaPrecisionType.UInt8))
            {
                if (unsigned)
                    WriteUInt8Unpacked((byte)Math.Floor(value * DOUBLE_ACCURACY));
                else
                    WriteInt8Unpacked((sbyte)Math.Floor(value * DOUBLE_ACCURACY));
            }
            else if (dpt.FastContains(UDeltaPrecisionType.UInt16))
            {
                if (unsigned)
                    WriteUInt16Unpacked((ushort)Math.Floor(value * DOUBLE_ACCURACY));
                else
                    WriteInt16Unpacked((short)Math.Floor(value * DOUBLE_ACCURACY));
            }
            //Anything else is unpacked.
            else
            {
                WriteSingleUnpacked(value);
            }
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// Value returned should be written as signed.
        /// </summary>
        private UDeltaPrecisionType GetSDeltaPrecisionType(float valueA, float valueB, out float signedDifference)
        {
            signedDifference = (valueB - valueA);
            float posValue = (signedDifference < 0f) ? (signedDifference * -1f) : signedDifference;

            return GetDeltaPrecisionType(posValue, unsigned: false);
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        private UDeltaPrecisionType GetUDeltaPrecisionType(float valueA, float valueB, out float unsignedDifference)
        {
            bool bIsLarger = (valueB > valueA);
            if (bIsLarger)
                unsignedDifference = (valueB - valueA);
            else
                unsignedDifference = (valueA - valueB);

            UDeltaPrecisionType result = GetDeltaPrecisionType(unsignedDifference, unsigned: true);
            //If result is set then set if bIsLarger.
            if (bIsLarger && result != UDeltaPrecisionType.Unset)
                result |= UDeltaPrecisionType.NextValueIsLarger;

            return result;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private UDeltaPrecisionType GetDeltaPrecisionType(float positiveValue, bool unsigned)
        {
            if (unsigned)
            {
                return positiveValue switch
                {
                    < (float)DOUBLE_ACCURACY_PRECISION => UDeltaPrecisionType.Unset,
                    < (float)LARGEST_DELTA_PRECISION_UINT8 => UDeltaPrecisionType.UInt8,
                    < (float)LARGEST_DELTA_PRECISION_UINT16 => UDeltaPrecisionType.UInt16,
                    _ => UDeltaPrecisionType.Unset,
                };
            }
            else
            {
                return positiveValue switch
                {
                    < (float)DOUBLE_ACCURACY_PRECISION => UDeltaPrecisionType.Unset,
                    < (float)LARGEST_DELTA_PRECISION_INT8 => UDeltaPrecisionType.UInt8,
                    < (float)LARGEST_DELTA_PRECISION_INT16 => UDeltaPrecisionType.UInt16,
                    _ => UDeltaPrecisionType.Unset,
                };
            }
        }
        #endregion

        #region Double.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaDouble(double valueA, double valueB)
        {
            UDeltaPrecisionType dpt = GetUDeltaPrecisionType(valueA, valueB, out double positiveDifference);

            if (dpt == UDeltaPrecisionType.Unset) return false;

            WriteUInt8Unpacked((byte)dpt);
            WriteDeltaDouble(dpt, positiveDifference, unsigned: true);

            return true;
        }

        /// <summary>
        /// Writes a double using DeltaPrecisionType.
        /// </summary>
        private void WriteDeltaDouble(UDeltaPrecisionType dpt, double value, bool unsigned)
        {
            if (dpt.FastContains(UDeltaPrecisionType.UInt8))
            {
                if (unsigned)
                    WriteUInt8Unpacked((byte)Math.Floor(value * DOUBLE_ACCURACY));
                else
                    WriteInt8Unpacked((sbyte)Math.Floor(value * DOUBLE_ACCURACY));
            }
            else if (dpt.FastContains(UDeltaPrecisionType.UInt16))
            {
                if (unsigned)
                    WriteUInt16Unpacked((ushort)Math.Floor(value * DOUBLE_ACCURACY));
                else
                    WriteInt16Unpacked((short)Math.Floor(value * DOUBLE_ACCURACY));
            }
            else if (dpt.FastContains(UDeltaPrecisionType.UInt32))
            {
                if (unsigned)
                    WriteUInt32Unpacked((uint)Math.Floor(value * DOUBLE_ACCURACY));
                else
                    WriteInt32Unpacked((int)Math.Floor(value * DOUBLE_ACCURACY));
            }
            else if (dpt.FastContains(UDeltaPrecisionType.Unset))
            {
                WriteDoubleUnpacked(value);
            }
            else
            {
                NetworkManagerExtensions.LogError($"Unhandled precision type of {dpt}.");
            }
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        private UDeltaPrecisionType GetSDeltaPrecisionType(double valueA, double valueB, out double signedDifference)
        {
            signedDifference = (valueB - valueA);
            double posValue = (signedDifference < 0d) ? (signedDifference * -1d) : signedDifference;

            return GetDeltaPrecisionType(posValue, unsigned: false);
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        private UDeltaPrecisionType GetUDeltaPrecisionType(double valueA, double valueB, out double unsignedDifference)
        {
            bool bIsLarger = (valueB > valueA);
            if (bIsLarger)
                unsignedDifference = (valueB - valueA);
            else
                unsignedDifference = (valueA - valueB);

            UDeltaPrecisionType result = GetDeltaPrecisionType(unsignedDifference, unsigned: true);
            if (bIsLarger && result != UDeltaPrecisionType.Unset)
                result |= UDeltaPrecisionType.NextValueIsLarger;

            return result;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private UDeltaPrecisionType GetDeltaPrecisionType(double positiveValue, bool unsigned)
        {
            if (unsigned)
            {
                return positiveValue switch
                {
                    < LARGEST_DELTA_PRECISION_UINT8 => UDeltaPrecisionType.UInt8,
                    < LARGEST_DELTA_PRECISION_UINT16 => UDeltaPrecisionType.UInt16,
                    < LARGEST_DELTA_PRECISION_UINT32 => UDeltaPrecisionType.UInt32,
                    _ => UDeltaPrecisionType.Unset,
                };
            }
            else
            {
                return positiveValue switch
                {
                    < LARGEST_DELTA_PRECISION_INT8 => UDeltaPrecisionType.UInt8,
                    < LARGEST_DELTA_PRECISION_INT16 => UDeltaPrecisionType.UInt16,
                    < LARGEST_DELTA_PRECISION_INT32 => UDeltaPrecisionType.UInt32,
                    _ => UDeltaPrecisionType.Unset,
                };
            }
        }
        #endregion

        #region Decimal
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaDecimal(decimal valueA, decimal valueB)
        {
            UDeltaPrecisionType dpt = GetUDeltaPrecisionType(valueA, valueB, out decimal positiveDifference);

            if (dpt == UDeltaPrecisionType.Unset) return false;

            WriteUInt8Unpacked((byte)dpt);
            WriteDeltaDecimal(dpt, positiveDifference, unsigned: true);

            return true;
        }

        /// <summary>
        /// Writes a double using DeltaPrecisionType.
        /// </summary>
        private void WriteDeltaDecimal(UDeltaPrecisionType dpt, decimal value, bool unsigned)
        {
            if (dpt.FastContains(UDeltaPrecisionType.UInt8))
            {
                if (unsigned)
                    WriteUInt8Unpacked((byte)Math.Floor(value * DECIMAL_ACCURACY));
                else
                    WriteInt8Unpacked((sbyte)Math.Floor(value * DECIMAL_ACCURACY));
            }
            else if (dpt.FastContains(UDeltaPrecisionType.UInt16))
            {
                if (unsigned)
                    WriteUInt16Unpacked((ushort)Math.Floor(value * DECIMAL_ACCURACY));
                else
                    WriteInt16Unpacked((short)Math.Floor(value * DECIMAL_ACCURACY));
            }
            else if (dpt.FastContains(UDeltaPrecisionType.UInt32))
            {
                if (unsigned)
                    WriteUInt32Unpacked((uint)Math.Floor(value * DECIMAL_ACCURACY));
                else
                    WriteInt32Unpacked((int)Math.Floor(value * DECIMAL_ACCURACY));
            }
            else if (dpt.FastContains(UDeltaPrecisionType.UInt64))
            {
                if (unsigned)
                    WriteUInt64Unpacked((ulong)Math.Floor(value * DECIMAL_ACCURACY));
                else
                    WriteInt64Unpacked((long)Math.Floor(value * DECIMAL_ACCURACY));
            }
            else if (dpt.FastContains(UDeltaPrecisionType.Unset))
            {
                WriteDecimalUnpacked(value);
            }
            else
            {
                NetworkManagerExtensions.LogError($"Unhandled precision type of {dpt}.");
            }
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        private UDeltaPrecisionType GetSDeltaPrecisionType(decimal valueA, decimal valueB, out decimal signedDifference)
        {
            signedDifference = (valueB - valueA);
            decimal posValue = (signedDifference < 0m) ? (signedDifference * -1m) : signedDifference;

            return GetDeltaPrecisionType(posValue, unsigned: false);
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        private UDeltaPrecisionType GetUDeltaPrecisionType(decimal valueA, decimal valueB, out decimal unsignedDifference)
        {
            bool bIsLarger = (valueB > valueA);
            if (bIsLarger)
                unsignedDifference = (valueB - valueA);
            else
                unsignedDifference = (valueA - valueB);

            UDeltaPrecisionType result = GetDeltaPrecisionType(unsignedDifference, unsigned: true);
            if (bIsLarger && result != UDeltaPrecisionType.Unset)
                result |= UDeltaPrecisionType.NextValueIsLarger;

            return result;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private UDeltaPrecisionType GetDeltaPrecisionType(decimal positiveValue, bool unsigned)
        {
            if (unsigned)
            {
                return positiveValue switch
                {
                    < (decimal)LARGEST_DELTA_PRECISION_UINT8 => UDeltaPrecisionType.UInt8,
                    < (decimal)LARGEST_DELTA_PRECISION_UINT16 => UDeltaPrecisionType.UInt16,
                    < (decimal)LARGEST_DELTA_PRECISION_UINT32 => UDeltaPrecisionType.UInt32,
                    < (decimal)LARGEST_DELTA_PRECISION_UINT64 => UDeltaPrecisionType.UInt64,
                    _ => UDeltaPrecisionType.Unset,
                };
            }
            else
            {
                return positiveValue switch
                {
                    < (decimal)LARGEST_DELTA_PRECISION_INT8 => UDeltaPrecisionType.UInt8,
                    < (decimal)LARGEST_DELTA_PRECISION_INT16 => UDeltaPrecisionType.UInt16,
                    < (decimal)LARGEST_DELTA_PRECISION_INT32 => UDeltaPrecisionType.UInt32,
                    < (decimal)LARGEST_DELTA_PRECISION_INT64 => UDeltaPrecisionType.UInt64,
                    _ => UDeltaPrecisionType.Unset,
                };
            }
        }
        #endregion

        #region Types.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaNetworkBehaviour(NetworkBehaviour valueA, NetworkBehaviour valueB)
        {
            if (valueA == valueB) return false;

            WriteNetworkBehaviour(valueB);

            return true;
        }
        #endregion

        #region Unity.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        [DefaultDeltaWriter]
        public bool WriteDeltaQuaternion(Quaternion valueA, Quaternion valueB)
        {
            const float minimumChange = 0.0025f;
            bool result = false;

            if (Mathf.Abs(valueA.x - valueB.x) > minimumChange)
                result = true;
            else if (Mathf.Abs(valueA.y - valueB.y) > minimumChange)
                result = true;
            else if (Mathf.Abs(valueA.z - valueB.z) > minimumChange)
                result = true;
            else if (Mathf.Abs(valueA.w - valueB.w) > minimumChange)
                result = true;

            if (result)
                WriteQuaternion32(valueB);

            return result;
        }

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        [DefaultDeltaWriter]
        public bool WriteDeltaVector2(Vector2 valueA, Vector2 valueB)
        {
            //TODO Fit as many flags into a byte as possible for pack levels of each axis rather than 1 per axis.
            byte allFlags = 0;

            int startPosition = Position;
            Skip(1);

            if (WriteUDeltaSingle(valueA.x, valueB.x))
                allFlags += 1;
            if (WriteUDeltaSingle(valueA.y, valueB.y))
                allFlags += 2;

            if (allFlags != 0)
            {
                InsertUInt8Unpacked(allFlags, startPosition);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        [DefaultDeltaWriter]
        public bool WriteDeltaVector3(Vector3 valueA, Vector3 valueB)
        {
            //TODO Fit as many flags into a byte as possible for pack levels of each axis rather than 1 per axis.
            byte allFlags = 0;

            int startPosition = Position;
            Skip(1);

            if (WriteUDeltaSingle(valueA.x, valueB.x))
                allFlags += 1;
            if (WriteUDeltaSingle(valueA.y, valueB.y))
                allFlags += 2;
            if (WriteUDeltaSingle(valueA.z, valueB.z))
                allFlags += 4;

            if (allFlags != 0)
            {
                InsertUInt8Unpacked(allFlags, startPosition);
                return true;
            }

            return false;
        }
        #endregion

        #region Prediction.
        /// <summary>
        /// Writes a delta reconcile.
        /// </summary>
        internal void WriteDeltaReconcile<T>(T lastReconcile, T value, DeltaSerializerOption deltaOption) => WriteDelta(lastReconcile, value, deltaOption);

        /// <summary>
        /// Writes a delta replicate using a list.
        /// </summary>
        internal void WriteDeltaReplicate<T>(List<T> values, int offset, DeltaSerializerOption deltaOption) where T : IReplicateData
        {
            int collectionCount = values.Count;
            //Replicate list will never be null, no need to write null check.
            //Number of entries being written.
            byte count = (byte)(collectionCount - offset);
            WriteUInt8Unpacked(count);

            T prev;
            //Set previous if not full and if enough room in the collection to go back.
            if (deltaOption != DeltaSerializerOption.FullSerialize && collectionCount > count)
                prev = values[offset - 1];
            else
                prev = default;

            for (int i = offset; i < collectionCount; i++)
            {
                T v = values[i];
                WriteDelta(prev, v, deltaOption);

                prev = v;
                //After the first loop the deltaOption can be set to root, if not already.
                deltaOption = DeltaSerializerOption.RootSerialize;
            }
        }

        /// <summary>
        /// Writes a delta replicate using a BasicQueue.
        /// </summary>
        internal void WriteDeltaReplicate<T>(BasicQueue<T> values, int redundancyCount, DeltaSerializerOption deltaOption) where T : IReplicateData
        {
            int collectionCount = values.Count;
            //Replicate list will never be null, no need to write null check.
            //Number of entries being written.
            byte count = (byte)redundancyCount;
            WriteUInt8Unpacked(count);

            int offset = (collectionCount - redundancyCount);
            T prev;
            //Set previous if not full and if enough room in the collection to go back.
            if (deltaOption != DeltaSerializerOption.FullSerialize && collectionCount > count)
                prev = values[offset - 1];
            else
                prev = default;

            for (int i = offset; i < collectionCount; i++)
            {
                T v = values[i];
                WriteDelta(prev, v, deltaOption);

                prev = v;
                //After the first loop the deltaOption can be set to root, if not already.
                deltaOption = DeltaSerializerOption.RootSerialize;
            }
        }
        #endregion

        #region Generic.
        public bool WriteDelta<T>(T prev, T next, DeltaSerializerOption option)
        {
            Func<Writer, T, T, DeltaSerializerOption, bool> del = GenericDeltaWriter<T>.Write;

            if (del == null)
            {
                NetworkManager.LogError($"Write delta method not found for {typeof(T).FullName}. Use a supported type or create a custom serializer.");

                return false;
            }
            else
            {
                return del.Invoke(this, prev, next, option);
            }
        }
        #endregion
    }
}