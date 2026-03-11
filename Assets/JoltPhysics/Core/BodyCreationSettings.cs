// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public sealed class BodyCreationSettings : NativeHandle
    {
        public BodyCreationSettings(Shape shape, Float3 position, Quat rotation,
            MotionType motionType, uint objectLayer)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                var pos = new JPH_Vec3 { x = position.x, y = position.y, z = position.z };
                var rot = new JPH_Quat { x = rotation.x, y = rotation.y, z = rotation.z, w = rotation.w };
                Handle = (IntPtr)Methods.JPH_BodyCreationSettings_Create3(
                    (JPH_Shape*)shape.Handle, &pos, &rot,
                    (JPH_MotionType)motionType, objectLayer);
            }
        }

        public float Friction
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetFriction(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetFriction(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public float Restitution
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetRestitution(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetRestitution(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public float LinearDamping
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetLinearDamping(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetLinearDamping(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public float AngularDamping
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetAngularDamping(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetAngularDamping(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        public float GravityFactor
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    return Methods.JPH_BodyCreationSettings_GetGravityFactor(
                        (JPH_BodyCreationSettings*)Handle);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    Methods.JPH_BodyCreationSettings_SetGravityFactor(
                        (JPH_BodyCreationSettings*)Handle, value);
                }
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_BodyCreationSettings_Destroy((JPH_BodyCreationSettings*)Handle);
            }
        }
    }
}
