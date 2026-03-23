using System;

namespace JoltPhysics
{
    public struct RayCastResult
    {
        public BodyID BodyID;
        public float Fraction;

        public RayCastResult(BodyID bodyId, float fraction)
        {
            BodyID = bodyId;
            Fraction = fraction;
        }
    }

    public readonly struct NarrowPhaseQuery
    {
        internal readonly IntPtr Handle;

        internal NarrowPhaseQuery(IntPtr handle)
        {
            Handle = handle;
        }

        /// <summary>
        /// 投射射线，返回最近的命中。
        /// direction 的长度即为射线最大距离（非归一化）。
        /// fraction ∈ [0,1]，实际距离 = fraction * |direction|。
        /// </summary>
        public bool CastRay(Float3 origin, Float3 direction, out RayCastResult result)
        {
            result = default;
            unsafe
            {
                var o = new JPH_Vec3 { x = origin.x, y = origin.y, z = origin.z };
                var d = new JPH_Vec3 { x = direction.x, y = direction.y, z = direction.z };
                JPH_RayCastResult hit;
                if (Methods.JPH_NarrowPhaseQuery_CastRay(
                        (JPH_NarrowPhaseQuery*)Handle,
                        &o, &d, &hit,
                        null, null, null) != 0)
                {
                    result = new RayCastResult(new BodyID(hit.bodyID), hit.fraction);
                    return true;
                }
                return false;
            }
        }
    }
}
