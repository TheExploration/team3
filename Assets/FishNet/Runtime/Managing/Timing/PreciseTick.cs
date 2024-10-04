﻿using System;
using FishNet.Serializing;
using GameKit.Dependencies.Utilities;
using UnityEngine;

namespace FishNet.Managing.Timing
{
    public readonly struct PreciseTick : IEquatable<PreciseTick>
    {
        /// <summary>
        /// The current tick.
        /// </summary>
        public readonly uint Tick;
        /// <summary>
        /// Percentage of the tick returned between 0d and 1d.
        /// </summary>
        public readonly double PercentAsDouble;
        /// <summary>
        /// Percentage of the tick returned between 0 and 100.
        /// </summary>
        public readonly byte PercentAsByte;

        /// <summary>
        /// Maximum value a percent can be as a double.
        /// </summary>
        public const double MAXIMUM_DOUBLE_PERCENT = 1d;
        /// <summary>
        /// Maximum value a percent can be as a byte.
        /// </summary>
        public const byte MAXIMUM_BYTE_PERCENT = 100;

        /// <summary>
        /// Creates a precise tick where the percentage is a byte between 0 and 100.
        /// </summary>
        public PreciseTick(uint tick, byte percentAsByte)
        {
            Tick = tick;

            percentAsByte = Maths.ClampByte(percentAsByte, 0, MAXIMUM_BYTE_PERCENT);
            PercentAsByte = percentAsByte;
            PercentAsDouble = (percentAsByte / 100d);
        }

        /// <summary>
        /// Creates a precise tick where the percentage is a double between 0d and 1d.
        /// </summary>
        public PreciseTick(uint tick, double percent)
        {
            Tick = tick;
            percent = Maths.ClampDouble(percent, 0d, MAXIMUM_DOUBLE_PERCENT);
            PercentAsByte = (byte)Math.Round(percent * 100d);
            PercentAsDouble = percent;
        }

        public bool IsValid() => (Tick != TimeManager.UNSET_TICK);

        /// <summary>
        /// Prints PreciseTick information as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Tick {Tick}, Percent {PercentAsByte.ToString("000")}";

        public static bool operator ==(PreciseTick a, PreciseTick b)
        {
            return (a.Tick == b.Tick && a.PercentAsByte == b.PercentAsByte);
        }

        public static bool operator !=(PreciseTick a, PreciseTick b)
        {
            return !(a == b);
        }

        public static bool operator >=(PreciseTick a, PreciseTick b)
        {
            if (b.Tick > a.Tick) return false;
            if (a.Tick > b.Tick) return true;
            //If here ticks are the same.
            return a.PercentAsByte >= b.PercentAsByte;
        }

        public static bool operator <=(PreciseTick a, PreciseTick b) => (b >= a);

        public static bool operator >(PreciseTick a, PreciseTick b)
        {

            if (b.Tick > a.Tick) return false;
            if (a.Tick > b.Tick) return true;
            //if here ticks are the same.
            return a.PercentAsByte > b.PercentAsByte;
        }

        public static bool operator <(PreciseTick a, PreciseTick b) => (b > a);

        public bool Equals(PreciseTick other) => (Tick == other.Tick && PercentAsByte == other.PercentAsByte);

        public override bool Equals(object obj)=> obj is PreciseTick other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Tick, PercentAsDouble, PercentAsByte);
        
    }

    public static class PreciseTickExtensions
    {
        /// <summary>
        /// Adds value onto a PreciseTick.
        /// </summary>
        /// <param name="value">Value to add.</param>
        /// <param name="delta">Tick delta.</param>
        /// <returns></returns>
        public static PreciseTick Add(this PreciseTick pt, PreciseTick value, double delta)
        {
            double percent = (pt.PercentAsDouble + value.PercentAsDouble);
            ulong tick = ((ulong)pt.Tick + (ulong)value.Tick);

            if (percent > PreciseTick.MAXIMUM_DOUBLE_PERCENT)
            {
                tick++;
                percent -= PreciseTick.MAXIMUM_DOUBLE_PERCENT;
            }

            if (tick > uint.MaxValue)
            {
                tick = uint.MaxValue;
                percent = 0d;
            }

            return new PreciseTick((uint)tick, percent);
        }

        /// <summary>
        /// Subtracts value from a PreciseTick.
        /// </summary>
        /// <param name="value">Value to subtract.</param>
        /// <param name="delta">Tick delta.</param>
        /// <returns></returns>
        public static PreciseTick Subtract(this PreciseTick pt, PreciseTick value, double delta)
        {
            double percent = (pt.PercentAsDouble - value.PercentAsDouble);
            long tick = ((long)pt.Tick - (long)value.Tick);

            // Percentage is negative so subtract an additional tick and absolute percentage.
            if (percent < 0d)
            {
                tick--;
                percent *= -1d;
            }

            if (tick < 0)
            {
                tick = 0;
                percent = 0d;
            }
            
            return new PreciseTick((uint)tick, percent);
        }
        
    }

    public static class PreciseTickSerializer
    {
        public static void WritePreciseTick(this Writer writer, PreciseTick value)
        {
            writer.WriteTickUnpacked(value.Tick);
            writer.WriteUInt8Unpacked(value.PercentAsByte);
        }

        public static PreciseTick ReadPreciseTick(this Reader reader)
        {
            uint tick = reader.ReadTickUnpacked();
            byte percentByte = reader.ReadUInt8Unpacked();
            return new(tick, percentByte);
        }
    }
}