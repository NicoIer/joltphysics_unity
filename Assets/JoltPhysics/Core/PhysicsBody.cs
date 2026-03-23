using System;
using System.Numerics;

namespace JoltPhysics
{
    /// <summary>
    /// 物理刚体基类，封装 Jolt Body 的创建与管理。
    /// </summary>
    public class PhysicsBody
    {
        public string name { get; set; }
        public int layer { get; set; }

        [Flags]
        private enum DirtyFlags : byte
        {
            None           = 0,
            Sensor         = 1 << 0,
            MotionType     = 1 << 1,
            Friction       = 1 << 2,
            Restitution    = 1 << 3,
            GravityFactor  = 1 << 4,
            Active         = 1 << 5,
            LinearVelocity = 1 << 6,
        }

        private DirtyFlags _dirtyFlags;
        private bool _pendingActive;
        private Vector3 _pendingLinearVelocity;

        private bool _isSensor = true;
        public bool isSensor
        {
            get => _isSensor;
            set
            {
                _isSensor = value;
                if (bodyCreated)
                    _dirtyFlags |= DirtyFlags.Sensor;
            }
        }

        public Vector3 position { get; set; } = Vector3.Zero;
        public Quaternion rotation { get; set; } = Quaternion.Identity;

        private MotionType _motionType = MotionType.Kinematic;
        public MotionType motionType
        {
            get => _motionType;
            set
            {
                _motionType = value;
                if (bodyCreated)
                    _dirtyFlags |= DirtyFlags.MotionType;
            }
        }

        // AllowedDOFs 只能在创建时设置，运行时修改无效（Jolt 不支持运行时更改）
        public AllowedDOFs allowedDOFs { get; set; } = AllowedDOFs.All;

        private float _friction;
        public float friction
        {
            get => _friction;
            set
            {
                _friction = value;
                if (bodyCreated)
                    _dirtyFlags |= DirtyFlags.Friction;
            }
        }

        private float _restitution;
        public float restitution
        {
            get => _restitution;
            set
            {
                _restitution = value;
                if (bodyCreated)
                    _dirtyFlags |= DirtyFlags.Restitution;
            }
        }

        public float linearDamping { get; set; }
        public float angularDamping { get; set; }

        private float _gravityFactor;
        public float gravityFactor
        {
            get => _gravityFactor;
            set
            {
                _gravityFactor = value;
                if (bodyCreated)
                    _dirtyFlags |= DirtyFlags.GravityFactor;
            }
        }

        public bool active
        {
            get
            {
                if (bodyCreated)
                {
                    // 有 pending Active 变更时，保证返回 true 以确保 SyncToJoltPhysics 能被调用来刷新
                    if ((_dirtyFlags & DirtyFlags.Active) != 0)
                        return true;
                    return bodyInterface.IsActive(bodyId);
                }
                return _initialActive;
            }
            set
            {
                if (bodyCreated)
                {
                    _pendingActive = value;
                    _dirtyFlags |= DirtyFlags.Active;
                }
                else
                {
                    _initialActive = value;
                }
            }
        }

        public Vector3 linearVelocity
        {
            get
            {
                if (!bodyCreated) return Vector3.Zero;
                var v = bodyInterface.GetLinearVelocity(bodyId);
                return new Vector3(v.x, v.y, v.z);
            }
            set
            {
                if (!bodyCreated) return;
                _pendingLinearVelocity = value;
                _dirtyFlags |= DirtyFlags.LinearVelocity;
            }
        }

        // Jolt 运行时状态
        internal BodyID bodyId;
        internal bool bodyCreated;
        internal BodyInterface bodyInterface;
        internal bool _initialActive = true;

        public PhysicsBody()
        {
        }

        public PhysicsBody(string name, int layer)
        {
            this.name = name;
            this.layer = layer;
        }

        /// <summary>
        /// 创建 Jolt Body。由 PhysicsMgr 调用。
        /// </summary>
        internal BodyID CreateBody(BodyInterface bi, Shape shape, Activation activation = Activation.Activate)
        {
            bodyInterface = bi;

            var pos = new Float3(position.X, position.Y, position.Z);
            var rot = new Quat(rotation.X, rotation.Y, rotation.Z, rotation.W);

            using var settings = new BodyCreationSettings(shape, pos, rot, _motionType, (uint)layer);
            settings.IsSensor = _isSensor;
            settings.AllowedDOFs = allowedDOFs;
            settings.Friction = _friction;
            settings.Restitution = _restitution;
            settings.LinearDamping = linearDamping;
            settings.AngularDamping = angularDamping;
            settings.GravityFactor = _gravityFactor;

            bodyId = bi.CreateAndAddBody(settings, activation);
            bodyCreated = true;
            return bodyId;
        }

        internal void DestroyBody()
        {
            if (!bodyCreated) return;
            bodyInterface.RemoveAndDestroyBody(bodyId);
            bodyCreated = false;
            _dirtyFlags = DirtyFlags.None;
        }

        /// <summary>
        /// 同步当前 position/rotation 及所有 dirty 属性到 Jolt Body。
        /// 子类可 override 实现偏移逻辑，但必须调用 base 以刷新 dirty 属性。
        /// </summary>
        public virtual void SyncToJoltPhysics()
        {
            if (!bodyCreated) return;

            // 先刷新 dirty 属性（Active 除外，放到最后处理）
            if (_dirtyFlags != DirtyFlags.None)
            {
                if ((_dirtyFlags & DirtyFlags.Sensor) != 0)
                    bodyInterface.SetIsSensor(bodyId, _isSensor);
                if ((_dirtyFlags & DirtyFlags.MotionType) != 0)
                    bodyInterface.SetMotionType(bodyId, _motionType, Activation.Activate);
                if ((_dirtyFlags & DirtyFlags.Friction) != 0)
                    bodyInterface.SetFriction(bodyId, _friction);
                if ((_dirtyFlags & DirtyFlags.Restitution) != 0)
                    bodyInterface.SetRestitution(bodyId, _restitution);
                if ((_dirtyFlags & DirtyFlags.GravityFactor) != 0)
                    bodyInterface.SetGravityFactor(bodyId, _gravityFactor);
                if ((_dirtyFlags & DirtyFlags.LinearVelocity) != 0)
                    bodyInterface.SetLinearVelocity(bodyId, new Float3(_pendingLinearVelocity.X, _pendingLinearVelocity.Y, _pendingLinearVelocity.Z));
            }

            // 同步 position/rotation
            var pos = new Float3(position.X, position.Y, position.Z);
            var rot = new Quat(rotation.X, rotation.Y, rotation.Z, rotation.W);
            bodyInterface.SetPositionAndRotation(bodyId, pos, rot, Activation.Activate);

            // Active 放最后：deactivate 不会被上面的 SetPositionAndRotation(Activate) 覆盖
            if ((_dirtyFlags & DirtyFlags.Active) != 0)
            {
                if (_pendingActive)
                    bodyInterface.ActivateBody(bodyId);
                else
                    bodyInterface.DeactivateBody(bodyId);
            }

            _dirtyFlags = DirtyFlags.None;
        }

        /// <summary>
        /// 从 Jolt Body 读取位置/旋转到本地字段。
        /// </summary>
        public virtual void SyncFromJoltPhysics()
        {
            if (!bodyCreated) return;
            bodyInterface.GetPositionAndRotation(bodyId, out var pos, out var rot);
            position = new Vector3(pos.x, pos.y, pos.z);
            rotation = new Quaternion(rot.x, rot.y, rot.z, rot.w);
        }

        /// <summary>
        /// 获取当前 Body 的 ShapeData 快照。根据 Jolt Shape 实际类型生成。
        /// 子类可 override 提供更精确的形状数据（如含 offset 信息）。
        /// </summary>
        public virtual ShapeData GetShapeData()
        {
            if (!bodyCreated) return ShapeData.CreateSphere(0.1f);
            return bodyInterface.GetShapeData(bodyId);
        }
    }
}
