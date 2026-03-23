// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public abstract class Shape : NativeHandle
    {
        internal Shape(IntPtr handle) : base(handle, true)
        {
        }

        public Float3 GetCenterOfMass()
        {
            ThrowIfDisposed();
            unsafe
            {
                JPH_Vec3 result;
                Methods.JPH_Shape_GetCenterOfMass((JPH_Shape*)Handle, &result);
                return new Float3(result.x, result.y, result.z);
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_Shape_Destroy((JPH_Shape*)Handle);
            }
        }
    }

    public sealed class SphereShape : Shape
    {
        public SphereShape(float radius)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_SphereShape_Create(radius);
            }
        }

        public float Radius
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_SphereShape_GetRadius((JPH_SphereShape*)Handle);
                }
            }
        }
    }

    public sealed class CapsuleShape : Shape
    {
        public CapsuleShape(float halfHeightOfCylinder, float radius)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_CapsuleShape_Create(halfHeightOfCylinder, radius);
            }
        }

        public float Radius
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_CapsuleShape_GetRadius((JPH_CapsuleShape*)Handle);
                }
            }
        }

        public float HalfHeightOfCylinder
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_CapsuleShape_GetHalfHeightOfCylinder((JPH_CapsuleShape*)Handle);
                }
            }
        }
    }

    public sealed class BoxShape : Shape
    {
        public BoxShape(Float3 halfExtent, float convexRadius = 0.05f)
            : base(IntPtr.Zero)
        {
            unsafe
            {
                var native = new JPH_Vec3 { x = halfExtent.x, y = halfExtent.y, z = halfExtent.z };
                Handle = (IntPtr)Methods.JPH_BoxShape_Create(&native, convexRadius);
            }
        }

        public Float3 HalfExtent
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_Vec3 result;
                    Methods.JPH_BoxShape_GetHalfExtent((JPH_BoxShape*)Handle, &result);
                    return new Float3(result.x, result.y, result.z);
                }
            }
        }
    }
}
