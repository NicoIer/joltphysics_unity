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
    }
}
