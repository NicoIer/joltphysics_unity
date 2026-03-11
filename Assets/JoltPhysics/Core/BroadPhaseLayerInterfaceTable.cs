// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public sealed class BroadPhaseLayerInterfaceTable : NativeHandle
    {
        public BroadPhaseLayerInterfaceTable(uint numObjectLayers, uint numBroadPhaseLayers)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_BroadPhaseLayerInterfaceTable_Create(
                    numObjectLayers, numBroadPhaseLayers);
            }
        }

        public void MapObjectToBroadPhaseLayer(uint objectLayer, byte broadPhaseLayer)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_BroadPhaseLayerInterfaceTable_MapObjectToBroadPhaseLayer(
                    (JPH_BroadPhaseLayerInterface*)Handle, objectLayer, broadPhaseLayer);
            }
        }

        protected override void DestroyNative()
        {
            // Filter tables do not have explicit destroy functions in the C API.
            // They are managed by the PhysicsSystem lifetime.
        }
    }
}
