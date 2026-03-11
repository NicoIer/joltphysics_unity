// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public abstract class NativeHandle : IDisposable
    {
        internal IntPtr Handle;
        internal bool OwnsHandle;
        private bool _disposed;

        internal NativeHandle(IntPtr handle, bool ownsHandle)
        {
            Handle = handle;
            OwnsHandle = ownsHandle;
        }

        protected abstract void DestroyNative();

        public bool IsDisposed => _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (OwnsHandle && Handle != IntPtr.Zero)
            {
                DestroyNative();
            }

            Handle = IntPtr.Zero;
            GC.SuppressFinalize(this);
        }

        ~NativeHandle()
        {
            if (!_disposed && OwnsHandle && Handle != IntPtr.Zero)
            {
                DestroyNative();
                Handle = IntPtr.Zero;
            }

            _disposed = true;
        }

        internal void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }
}