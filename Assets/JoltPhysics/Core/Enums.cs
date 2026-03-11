// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace JoltPhysics
{
    public enum MotionType : uint
    {
        Static = 0,
        Kinematic = 1,
        Dynamic = 2,
    }

    public enum Activation : uint
    {
        Activate = 0,
        DontActivate = 1,
    }

    public enum BodyType : uint
    {
        Rigid = 0,
        Soft = 1,
    }
}