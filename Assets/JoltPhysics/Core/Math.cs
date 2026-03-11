// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.Runtime.InteropServices;

namespace JoltPhysics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Float3 : IEquatable<Float3>
    {
        public float x;
        public float y;
        public float z;

        public static readonly Float3 Zero = new(0f, 0f, 0f);
        public static readonly Float3 One = new(1f, 1f, 1f);
        public static readonly Float3 Up = new(0f, 1f, 0f);
        public static readonly Float3 Down = new(0f, -1f, 0f);

        public Float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(Float3 other) => x == other.x && y == other.y && z == other.z;
        public override bool Equals(object obj) => obj is Float3 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(x, y, z);
        public override string ToString() => $"Float3({x}, {y}, {z})";

        public static bool operator ==(Float3 left, Float3 right) => left.Equals(right);
        public static bool operator !=(Float3 left, Float3 right) => !left.Equals(right);

        public static Float3 operator +(Float3 a, Float3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Float3 operator -(Float3 a, Float3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Float3 operator *(Float3 a, float s) => new(a.x * s, a.y * s, a.z * s);
        public static Float3 operator *(float s, Float3 a) => new(a.x * s, a.y * s, a.z * s);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quat : IEquatable<Quat>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public static readonly Quat Identity = new(0f, 0f, 0f, 1f);

        public Quat(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool Equals(Quat other) => x == other.x && y == other.y && z == other.z && w == other.w;
        public override bool Equals(object obj) => obj is Quat other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(x, y, z, w);
        public override string ToString() => $"Quat({x}, {y}, {z}, {w})";

        public static bool operator ==(Quat left, Quat right) => left.Equals(right);
        public static bool operator !=(Quat left, Quat right) => !left.Equals(right);
    }
}