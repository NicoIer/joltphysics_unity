// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using JoltPhysics;
using UnityEngine;

namespace JoltPhysics.Unity
{
    [AddComponentMenu("Jolt Physics/Box Shape")]
    public sealed class JoltBoxShape : JoltShape
    {
        [SerializeField] Vector3 _halfExtent = new(0.5f, 0.5f, 0.5f);
        [SerializeField] float _convexRadius = 0.05f;

        public Vector3 HalfExtent
        {
            get => _halfExtent;
            set => _halfExtent = value;
        }

        public float ConvexRadius
        {
            get => _convexRadius;
            set => _convexRadius = value;
        }

        public override Shape CreateShape()
        {
            return new BoxShape(_halfExtent.ToJolt(), _convexRadius);
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, _halfExtent * 2f);
            Gizmos.matrix = Matrix4x4.identity;
        }
#endif
    }
}
