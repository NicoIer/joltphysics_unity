// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public sealed class ObjectLayerPairFilterTable : NativeHandle
    {
        public ObjectLayerPairFilterTable(uint numObjectLayers)
            : base(IntPtr.Zero, true)
        {
            unsafe
            {
                Handle = (IntPtr)Methods.JPH_ObjectLayerPairFilterTable_Create(numObjectLayers);
            }
        }

        public void EnableCollision(uint layer1, uint layer2)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_ObjectLayerPairFilterTable_EnableCollision(
                    (JPH_ObjectLayerPairFilter*)Handle, layer1, layer2);
            }
        }

        public void DisableCollision(uint layer1, uint layer2)
        {
            ThrowIfDisposed();
            unsafe
            {
                Methods.JPH_ObjectLayerPairFilterTable_DisableCollision(
                    (JPH_ObjectLayerPairFilter*)Handle, layer1, layer2);
            }
        }

        protected override void DestroyNative()
        {
            // Filter tables do not have explicit destroy functions in the C API.
            // They are managed by the PhysicsSystem lifetime.
        }
    }
}
