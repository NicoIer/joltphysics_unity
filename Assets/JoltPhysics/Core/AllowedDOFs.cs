// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    [Flags]
    public enum AllowedDOFs : uint
    {
        All = 0b111111,
        TranslationX = 0b000001,
        TranslationY = 0b000010,
        TranslationZ = 0b000100,
        RotationX = 0b001000,
        RotationY = 0b010000,
        RotationZ = 0b100000,
        Plane2D = TranslationX | TranslationY | RotationZ,
    }
}
