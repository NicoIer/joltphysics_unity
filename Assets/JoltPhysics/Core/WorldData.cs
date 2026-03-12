// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;

namespace JoltPhysics
{
    public enum JoltShapeType
    {
        Box = 0,
        Sphere = 1,
        Ellipsoid = 2,
        Mesh = 3,
    }

    [Serializable]
    public class LayerData
    {
        public string name;
        public byte broadPhaseLayer;
        public uint collisionMask;
    }

    /// <summary>
    /// Flat shape data with type discriminator.
    /// Only fields relevant to <see cref="type"/> are populated.
    /// </summary>
    [Serializable]
    public class ShapeData
    {
        public JoltShapeType type;

        // Box
        public Float3 center;
        public Float3 halfExtent;
        public float convexRadius;

        // Sphere
        public float radius;

        // Ellipsoid
        public Float3 radii;

        // Mesh
        public Float3[] vertices;
        public int[] triangles;

        public static ShapeData CreateBox(Float3 center, Float3 halfExtent, float convexRadius = 0.05f)
        {
            return new ShapeData
            {
                type = JoltShapeType.Box,
                center = center,
                halfExtent = halfExtent,
                convexRadius = convexRadius,
            };
        }

        public static ShapeData CreateSphere(float radius)
        {
            return new ShapeData
            {
                type = JoltShapeType.Sphere,
                radius = radius,
            };
        }

        public static ShapeData CreateEllipsoid(Float3 radii)
        {
            return new ShapeData
            {
                type = JoltShapeType.Ellipsoid,
                radii = radii,
            };
        }

        public static ShapeData CreateMesh(Float3[] vertices, int[] triangles)
        {
            return new ShapeData
            {
                type = JoltShapeType.Mesh,
                vertices = vertices,
                triangles = triangles,
            };
        }
    }

    [Serializable]
    public class BodyData
    {
        public string name;
        public Float3 position;
        public Quat rotation;
        public MotionType motionType;
        public uint objectLayer;
        public float friction;
        public float restitution;
        public float linearDamping;
        public float angularDamping;
        public float gravityFactor;
        public ShapeData shape;
    }

    [Serializable]
    public class WorldData
    {
        public Float3 gravity;
        public uint maxBodies;
        public uint maxContactConstraints;
        public int collisionSteps;
        public LayerData[] layers;
        public BodyData[] bodies;

        public static WorldData CreateDefault()
        {
            return new WorldData
            {
                gravity = new Float3(0, -9.81f, 0),
                maxBodies = 10240,
                maxContactConstraints = 10240,
                collisionSteps = 1,
                layers = new[]
                {
                    new LayerData { name = "Static", broadPhaseLayer = 0, collisionMask = 0b10 },
                    new LayerData { name = "Dynamic", broadPhaseLayer = 1, collisionMask = 0b11 },
                },
                bodies = Array.Empty<BodyData>(),
            };
        }
    }
}
