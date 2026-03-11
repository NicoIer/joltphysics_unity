// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public sealed class ObjectVsBroadPhaseLayerFilterTable : NativeHandle
    {
        public ObjectVsBroadPhaseLayerFilterTable(
            BroadPhaseLayerInterfaceTable broadPhaseLayerInterface,
            uint numBroadPhaseLayers,
            ObjectLayerPairFilterTable objectLayerPairFilter,
            uint numObjectLayers)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_ObjectVsBroadPhaseLayerFilterTable_Create(
                    (JPH_BroadPhaseLayerInterface*)broadPhaseLayerInterface.Handle,
                    numBroadPhaseLayers,
                    (JPH_ObjectLayerPairFilter*)objectLayerPairFilter.Handle,
                    numObjectLayers);
            }
        }

        protected override void DestroyNative()
        {
            // Filter tables do not have explicit destroy functions in the C API.
            // They are managed by the PhysicsSystem lifetime.
        }
    }
}
