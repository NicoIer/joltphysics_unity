// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public readonly struct BodyInterface
    {
        internal readonly IntPtr Handle;

        internal BodyInterface(IntPtr handle)
        {
            Handle = handle;
        }

        public BodyID CreateAndAddBody(BodyCreationSettings settings, Activation activation)
        {
            unsafe
            {
                uint id = Methods.JPH_BodyInterface_CreateAndAddBody(
                    (JPH_BodyInterface*)Handle,
                    (JPH_BodyCreationSettings*)settings.Handle,
                    (JPH_Activation)activation);
                return new BodyID(id);
            }
        }

        public void RemoveAndDestroyBody(BodyID bodyID)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_RemoveAndDestroyBody(
                    (JPH_BodyInterface*)Handle, bodyID.Value);
            }
        }

        public Float3 GetPosition(BodyID bodyID)
        {
            unsafe
            {
                JPH_Vec3 result;
                Methods.JPH_BodyInterface_GetPosition(
                    (JPH_BodyInterface*)Handle, bodyID.Value, &result);
                return new Float3(result.x, result.y, result.z);
            }
        }

        public void SetPosition(BodyID bodyID, Float3 position, Activation activation)
        {
            unsafe
            {
                var pos = new JPH_Vec3 { x = position.x, y = position.y, z = position.z };
                Methods.JPH_BodyInterface_SetPosition(
                    (JPH_BodyInterface*)Handle, bodyID.Value,
                    &pos, (JPH_Activation)activation);
            }
        }
        
        public Quat GetRotation(BodyID bodyID)
        {
            unsafe
            {
                JPH_Quat result;
                Methods.JPH_BodyInterface_GetRotation(
                    (JPH_BodyInterface*)Handle, bodyID.Value, &result);
                return new Quat(result.x, result.y, result.z, result.w);
            }
        }

        public void SetRotation(BodyID bodyID, Quat rotation, Activation activation)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetRotation(
                    (JPH_BodyInterface*)Handle, bodyID.Value,
                    (JPH_Quat*)&rotation, (JPH_Activation)activation);
            }
        }
        
        

        public Float3 GetLinearVelocity(BodyID bodyID)
        {
            unsafe
            {
                JPH_Vec3 result;
                Methods.JPH_BodyInterface_GetLinearVelocity(
                    (JPH_BodyInterface*)Handle, bodyID.Value, &result);
                return new Float3(result.x, result.y, result.z);
            }
        }

        public void SetLinearVelocity(BodyID bodyID, Float3 velocity)
        {
            unsafe
            {
                var vel = new JPH_Vec3 { x = velocity.x, y = velocity.y, z = velocity.z };
                Methods.JPH_BodyInterface_SetLinearVelocity(
                    (JPH_BodyInterface*)Handle, bodyID.Value, &vel);
            }
        }

        public void GetPositionAndRotation(BodyID bodyID, out Float3 position, out Quat rotation)
        {
            unsafe
            {
                JPH_Vec3 pos;
                JPH_Quat rot;
                Methods.JPH_BodyInterface_GetPositionAndRotation(
                    (JPH_BodyInterface*)Handle, bodyID.Value, &pos, &rot);
                position = new Float3(pos.x, pos.y, pos.z);
                rotation = new Quat(rot.x, rot.y, rot.z, rot.w);
            }
        }

        public void SetPositionAndRotation(BodyID bodyID, Float3 position, Quat rotation,
            Activation activation)
        {
            unsafe
            {
                var pos = new JPH_Vec3 { x = position.x, y = position.y, z = position.z };
                var rot = new JPH_Quat { x = rotation.x, y = rotation.y, z = rotation.z, w = rotation.w };
                Methods.JPH_BodyInterface_SetPositionAndRotation(
                    (JPH_BodyInterface*)Handle, bodyID.Value,
                    &pos, &rot, (JPH_Activation)activation);
            }
        }

        public void MoveKinematic(BodyID bodyID, Float3 targetPosition, Quat targetRotation, float deltaTime)
        {
            unsafe
            {
                var pos = new JPH_Vec3 { x = targetPosition.x, y = targetPosition.y, z = targetPosition.z };
                var rot = new JPH_Quat { x = targetRotation.x, y = targetRotation.y, z = targetRotation.z, w = targetRotation.w };
                Methods.JPH_BodyInterface_MoveKinematic(
                    (JPH_BodyInterface*)Handle, bodyID.Value,
                    &pos, &rot, deltaTime);
            }
        }

        public float GetRestitution(BodyID bodyID)
        {
            unsafe
            {
                return Methods.JPH_BodyInterface_GetRestitution(
                    (JPH_BodyInterface*)Handle, bodyID.Value);
            }
        }

        public void SetRestitution(BodyID bodyID, float restitution)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetRestitution(
                    (JPH_BodyInterface*)Handle, bodyID.Value, restitution);
            }
        }

        public float GetFriction(BodyID bodyID)
        {
            unsafe
            {
                return Methods.JPH_BodyInterface_GetFriction(
                    (JPH_BodyInterface*)Handle, bodyID.Value);
            }
        }

        public void SetFriction(BodyID bodyID, float friction)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetFriction(
                    (JPH_BodyInterface*)Handle, bodyID.Value, friction);
            }
        }

        public void SetShape(BodyID bodyID, Shape shape, bool updateMassProperties, Activation activation)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetShape(
                    (JPH_BodyInterface*)Handle, bodyID.Value,
                    (JPH_Shape*)shape.Handle,
                    (byte)(updateMassProperties ? 1 : 0),
                    (JPH_Activation)activation);
            }
        }

        public void ActivateBody(BodyID bodyID)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_ActivateBody(
                    (JPH_BodyInterface*)Handle, bodyID.Value);
            }
        }

        public void DeactivateBody(BodyID bodyID)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_DeactivateBody(
                    (JPH_BodyInterface*)Handle, bodyID.Value);
            }
        }

        public void SetUserData(BodyID bodyID, ulong userData)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetUserData(
                    (JPH_BodyInterface*)Handle, bodyID.Value, userData);
            }
        }

        public ulong GetUserData(BodyID bodyID)
        {
            unsafe
            {
                return Methods.JPH_BodyInterface_GetUserData(
                    (JPH_BodyInterface*)Handle, bodyID.Value);
            }
        }

        public bool IsActive(BodyID bodyID)
        {
            unsafe
            {
                return Methods.JPH_BodyInterface_IsActive(
                    (JPH_BodyInterface*)Handle, bodyID.Value) != 0;
            }
        }

        public void SetIsSensor(BodyID bodyID, bool isSensor)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetIsSensor(
                    (JPH_BodyInterface*)Handle, bodyID.Value,
                    (byte)(isSensor ? 1 : 0));
            }
        }

        public void SetMotionType(BodyID bodyID, MotionType motionType, Activation activation)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetMotionType(
                    (JPH_BodyInterface*)Handle, bodyID.Value,
                    (JPH_MotionType)motionType, (JPH_Activation)activation);
            }
        }

        public void SetGravityFactor(BodyID bodyID, float gravityFactor)
        {
            unsafe
            {
                Methods.JPH_BodyInterface_SetGravityFactor(
                    (JPH_BodyInterface*)Handle, bodyID.Value, gravityFactor);
            }
        }

        public bool IsSensor(BodyID bodyID)
        {
            unsafe
            {
                return Methods.JPH_BodyInterface_IsSensor(
                    (JPH_BodyInterface*)Handle, bodyID.Value) != 0;
            }
        }

        /// <summary>
        /// 根据 Body 当前 Shape 的实际类型生成对应的 ShapeData 快照。
        /// </summary>
        public ShapeData GetShapeData(BodyID bodyID)
        {
            unsafe
            {
                var shape = Methods.JPH_BodyInterface_GetShape(
                    (JPH_BodyInterface*)Handle, bodyID.Value);
                if (shape == null)
                    return ShapeData.CreateSphere(0.1f);

                var subType = Methods.JPH_Shape_GetSubType(shape);
                switch (subType)
                {
                    case JPH_ShapeSubType.JPH_ShapeSubType_Box:
                    {
                        JPH_Vec3 halfExt;
                        Methods.JPH_BoxShape_GetHalfExtent((JPH_BoxShape*)shape, &halfExt);
                        float convexRadius = Methods.JPH_BoxShape_GetConvexRadius((JPH_BoxShape*)shape);
                        return ShapeData.CreateBox(
                            new Float3(0, 0, 0),
                            new Float3(halfExt.x, halfExt.y, halfExt.z),
                            convexRadius);
                    }
                    case JPH_ShapeSubType.JPH_ShapeSubType_Sphere:
                    {
                        float radius = Methods.JPH_SphereShape_GetRadius((JPH_SphereShape*)shape);
                        return ShapeData.CreateSphere(radius);
                    }
                    case JPH_ShapeSubType.JPH_ShapeSubType_Capsule:
                    {
                        float capsuleRadius = Methods.JPH_CapsuleShape_GetRadius((JPH_CapsuleShape*)shape);
                        float halfHeight = Methods.JPH_CapsuleShape_GetHalfHeightOfCylinder((JPH_CapsuleShape*)shape);
                        return ShapeData.CreateCapsule(halfHeight, capsuleRadius);
                    }
                    case JPH_ShapeSubType.JPH_ShapeSubType_ConvexHull:
                    {
                        uint numPoints = Methods.JPH_ConvexHullShape_GetNumPoints((JPH_ConvexHullShape*)shape);
                        var points = new Float3[numPoints];
                        for (uint i = 0; i < numPoints; i++)
                        {
                            JPH_Vec3 pt;
                            Methods.JPH_ConvexHullShape_GetPoint((JPH_ConvexHullShape*)shape, i, &pt);
                            points[i] = new Float3(pt.x, pt.y, pt.z);
                        }
                        return ShapeData.CreateConvexHull(points);
                    }
                    default:
                    {
                        // 其他: 用 LocalBounds 近似为 Box
                        JPH_AABox bounds;
                        Methods.JPH_Shape_GetLocalBounds(shape, &bounds);
                        float hx = (bounds.max.x - bounds.min.x) * 0.5f;
                        float hy = (bounds.max.y - bounds.min.y) * 0.5f;
                        float hz = (bounds.max.z - bounds.min.z) * 0.5f;
                        float cx = (bounds.max.x + bounds.min.x) * 0.5f;
                        float cy = (bounds.max.y + bounds.min.y) * 0.5f;
                        float cz = (bounds.max.z + bounds.min.z) * 0.5f;
                        return ShapeData.CreateBox(
                            new Float3(cx, cy, cz),
                            new Float3(hx, hy, hz));
                    }
                }
            }
        }

        /// <summary>
        /// 对指定 Body 做射线检测（世界空间）。
        /// worldDirection 的长度即为射线最大距离。
        /// 返回 true 时 fraction ∈ [0,1]，实际命中距离 = fraction * |worldDirection|。
        /// </summary>
        public bool RaycastBody(BodyID bodyID, Float3 worldOrigin, Float3 worldDirection, out float fraction)
        {
            fraction = float.MaxValue;
            unsafe
            {
                // 获取 body 世界变换
                JPH_Vec3 bodyPos;
                JPH_Quat bodyRot;
                Methods.JPH_BodyInterface_GetPositionAndRotation(
                    (JPH_BodyInterface*)Handle, bodyID.Value, &bodyPos, &bodyRot);

                // 获取 body 的 shape（非拥有指针）
                var shape = Methods.JPH_BodyInterface_GetShape(
                    (JPH_BodyInterface*)Handle, bodyID.Value);
                if (shape == null) return false;

                // 计算 body 旋转的逆（共轭四元数）
                float qx = -bodyRot.x, qy = -bodyRot.y, qz = -bodyRot.z, qw = bodyRot.w;

                // 将射线原点转换到 body 局部空间
                float ox = worldOrigin.x - bodyPos.x;
                float oy = worldOrigin.y - bodyPos.y;
                float oz = worldOrigin.z - bodyPos.z;
                RotateByQuat(qx, qy, qz, qw, ox, oy, oz, out var localOrigin);

                // 将射线方向转换到 body 局部空间
                RotateByQuat(qx, qy, qz, qw, worldDirection.x, worldDirection.y, worldDirection.z, out var localDir);

                JPH_RayCastResult hit;
                if (Methods.JPH_Shape_CastRay(shape, &localOrigin, &localDir, &hit) != 0)
                {
                    fraction = hit.fraction;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 用四元数旋转向量: result = q * v * q^-1
        /// </summary>
        private static void RotateByQuat(float qx, float qy, float qz, float qw,
            float vx, float vy, float vz, out JPH_Vec3 result)
        {
            // t = 2 * cross(q.xyz, v)
            float tx = 2f * (qy * vz - qz * vy);
            float ty = 2f * (qz * vx - qx * vz);
            float tz = 2f * (qx * vy - qy * vx);
            result.x = vx + qw * tx + (qy * tz - qz * ty);
            result.y = vy + qw * ty + (qz * tx - qx * tz);
            result.z = vz + qw * tz + (qx * ty - qy * tx);
        }
    }
}
