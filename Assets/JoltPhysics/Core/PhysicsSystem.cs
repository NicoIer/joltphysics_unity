// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public struct PhysicsSystemSettings
    {
        public uint MaxBodies;
        public uint NumBodyMutexes;
        public uint MaxBodyPairs;
        public uint MaxContactConstraints;

        public static PhysicsSystemSettings Default => new()
        {
            MaxBodies = 10240,
            NumBodyMutexes = 0,
            MaxBodyPairs = 65536,
            MaxContactConstraints = 10240,
        };
    }

    public sealed class PhysicsSystem : NativeHandle
    {
        // Keep GC references to filters to prevent premature collection
        private readonly BroadPhaseLayerInterfaceTable _broadPhaseLayerInterface;
        private readonly ObjectLayerPairFilterTable _objectLayerPairFilter;
        private readonly ObjectVsBroadPhaseLayerFilterTable _objectVsBroadPhaseLayerFilter;

        public PhysicsSystem(
            PhysicsSystemSettings settings,
            BroadPhaseLayerInterfaceTable broadPhaseLayerInterface,
            ObjectLayerPairFilterTable objectLayerPairFilter,
            ObjectVsBroadPhaseLayerFilterTable objectVsBroadPhaseLayerFilter)
            : base(IntPtr.Zero, true)
        {
            _broadPhaseLayerInterface = broadPhaseLayerInterface;
            _objectLayerPairFilter = objectLayerPairFilter;
            _objectVsBroadPhaseLayerFilter = objectVsBroadPhaseLayerFilter;

            unsafe
            {
                var nativeSettings = new JPH_PhysicsSystemSettings
                {
                    maxBodies = settings.MaxBodies,
                    numBodyMutexes = settings.NumBodyMutexes,
                    maxBodyPairs = settings.MaxBodyPairs,
                    maxContactConstraints = settings.MaxContactConstraints,
                    broadPhaseLayerInterface = (JPH_BroadPhaseLayerInterface*)broadPhaseLayerInterface.Handle,
                    objectLayerPairFilter = (JPH_ObjectLayerPairFilter*)objectLayerPairFilter.Handle,
                    objectVsBroadPhaseLayerFilter =
                        (JPH_ObjectVsBroadPhaseLayerFilter*)objectVsBroadPhaseLayerFilter.Handle,
                };
                Handle = (IntPtr)Methods.JPH_PhysicsSystem_Create(&nativeSettings);
            }
        }

        public BodyInterface GetBodyInterface()
        {
            ThrowIfDisposed();
            unsafe
            {
                var ptr = (IntPtr)Methods.JPH_PhysicsSystem_GetBodyInterface((JPH_PhysicsSystem*)Handle);
                return new BodyInterface(ptr);
            }
        }

        public Float3 Gravity
        {
            get
            {
                ThrowIfDisposed();
                unsafe
                {
                    JPH_Vec3 result;
                    Methods.JPH_PhysicsSystem_GetGravity((JPH_PhysicsSystem*)Handle, &result);
                    return new Float3(result.x, result.y, result.z);
                }
            }
            set
            {
                ThrowIfDisposed();
                unsafe
                {
                    var native = new JPH_Vec3 { x = value.x, y = value.y, z = value.z };
                    Methods.JPH_PhysicsSystem_SetGravity((JPH_PhysicsSystem*)Handle, &native);
                }
            }
        }

        public void Update(float deltaTime, int collisionSteps, JobSystemThreadPool jobSystem)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_PhysicsSystem_Update(
                    (JPH_PhysicsSystem*)Handle,
                    deltaTime,
                    collisionSteps,
                    (JPH_JobSystem*)jobSystem.Handle);
            }
        }

        protected override void DestroyNative()
        {
            unsafe
            {
                Methods.JPH_PhysicsSystem_Destroy((JPH_PhysicsSystem*)Handle);
            }
        }
    }
}
