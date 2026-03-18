// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace JoltPhysics
{
    public static class JoltAPI
    {
        public static bool Init()
        {
            return Methods.JPH_Init() != 0;
        }

        public static void Shutdown()
        {
            Methods.JPH_Shutdown();
        }
    }
}
