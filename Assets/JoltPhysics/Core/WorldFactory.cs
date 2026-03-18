// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.Collections.Generic;

namespace JoltPhysics
{
    /// <summary>
    /// Holds all native resources for a Jolt physics world created from <see cref="WorldData"/>.
    /// Dispose to clean up all native handles.
    /// </summary>
    public sealed class PhysicsWorldInstance : IDisposable
    {
        public PhysicsSystem PhysicsSystem { get; internal set; }
        public JobSystemThreadPool JobSystem { get; internal set; }
        public BodyInterface BodyInterface { get; internal set; }
        public BodyID[] BodyIds { get; internal set; }

        // Keep GC references to prevent premature collection
        internal BroadPhaseLayerInterfaceTable BroadPhaseLayerInterface;
        internal ObjectLayerPairFilterTable ObjectLayerPairFilter;
        internal ObjectVsBroadPhaseLayerFilterTable ObjectVsBroadPhaseLayerFilter;
        internal List<Shape> Shapes;

        bool _disposed;

        public void Update(float deltaTime, int collisionSteps)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(PhysicsWorldInstance));
            PhysicsSystem.Update(deltaTime, collisionSteps, JobSystem);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // Remove and destroy all bodies
            if (BodyIds != null)
            {
                foreach (var id in BodyIds)
                {
                    if (id.IsValid)
                        BodyInterface.RemoveAndDestroyBody(id);
                }
            }

            // Dispose shapes
            if (Shapes != null)
            {
                foreach (var shape in Shapes)
                    shape?.Dispose();
            }

            PhysicsSystem?.Dispose();
            JobSystem?.Dispose();
            ObjectVsBroadPhaseLayerFilter?.Dispose();
            ObjectLayerPairFilter?.Dispose();
            BroadPhaseLayerInterface?.Dispose();
        }
    }

    /// <summary>
    /// Creates Jolt physics worlds and shapes from serializable data.
    /// No Unity dependency — usable on server or any .NET host.
    /// </summary>
    public static class WorldFactory
    {
        /// <summary>
        /// Creates a complete physics world from data.
        /// The caller must have called <see cref="JoltAPI.Init"/> before and
        /// must call <see cref="JoltAPI.Shutdown"/> after disposing the returned instance.
        /// </summary>
        public static PhysicsWorldInstance CreateWorld(WorldData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.layers == null || data.layers.Length == 0)
                throw new ArgumentException("WorldData must have at least one layer.");

            int numObjectLayers = data.layers.Length;
            int numBroadPhaseLayers = GetBroadPhaseLayerCount(data.layers);

            // Layer configuration
            var broadPhase = new BroadPhaseLayerInterfaceTable(
                (uint)numObjectLayers, (uint)numBroadPhaseLayers);
            for (int i = 0; i < numObjectLayers; i++)
                broadPhase.MapObjectToBroadPhaseLayer((uint)i, data.layers[i].broadPhaseLayer);

            var objectPairFilter = new ObjectLayerPairFilterTable((uint)numObjectLayers);
            for (int i = 0; i < numObjectLayers; i++)
            {
                uint mask = data.layers[i].collisionMask;
                for (int j = 0; j < numObjectLayers; j++)
                {
                    if ((mask & (1u << j)) != 0)
                        objectPairFilter.EnableCollision((uint)i, (uint)j);
                    else
                        objectPairFilter.DisableCollision((uint)i, (uint)j);
                }
            }

            var vsBroadPhase = new ObjectVsBroadPhaseLayerFilterTable(
                broadPhase, (uint)numBroadPhaseLayers,
                objectPairFilter, (uint)numObjectLayers);

            // Physics system
            var settings = new PhysicsSystemSettings
            {
                MaxBodies = data.maxBodies > 0 ? data.maxBodies : 10240,
                NumBodyMutexes = 0,
                MaxBodyPairs = (data.maxBodies > 0 ? data.maxBodies : 10240) * 4,
                MaxContactConstraints = data.maxContactConstraints > 0 ? data.maxContactConstraints : 10240,
            };

            var physicsSystem = new PhysicsSystem(settings, broadPhase, objectPairFilter, vsBroadPhase);
            physicsSystem.Gravity = data.gravity;

            var jobSystem = new JobSystemThreadPool();
            var bodyInterface = physicsSystem.GetBodyInterface();

            // Create bodies
            var bodyIds = new List<BodyID>();
            var shapes = new List<Shape>();

            if (data.bodies != null)
            {
                foreach (var bodyData in data.bodies)
                {
                    var shape = CreateShape(bodyData.shape);
                    if (shape == null) continue;
                    shapes.Add(shape);

                    using var creationSettings = new BodyCreationSettings(
                        shape,
                        bodyData.position,
                        bodyData.rotation,
                        bodyData.motionType,
                        bodyData.objectLayer);

                    creationSettings.Friction = bodyData.friction;
                    creationSettings.Restitution = bodyData.restitution;
                    creationSettings.LinearDamping = bodyData.linearDamping;
                    creationSettings.AngularDamping = bodyData.angularDamping;
                    creationSettings.GravityFactor = bodyData.gravityFactor;

                    var activation = bodyData.motionType == MotionType.Static
                        ? Activation.DontActivate
                        : Activation.Activate;

                    var id = bodyInterface.CreateAndAddBody(creationSettings, activation);
                    bodyIds.Add(id);
                }
            }

            return new PhysicsWorldInstance
            {
                PhysicsSystem = physicsSystem,
                JobSystem = jobSystem,
                BodyInterface = bodyInterface,
                BodyIds = bodyIds.ToArray(),
                BroadPhaseLayerInterface = broadPhase,
                ObjectLayerPairFilter = objectPairFilter,
                ObjectVsBroadPhaseLayerFilter = vsBroadPhase,
                Shapes = shapes,
            };
        }

        /// <summary>
        /// Creates a Jolt Shape from shape data. The caller owns the returned shape.
        /// </summary>
        public static Shape CreateShape(ShapeData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            switch (data.type)
            {
                case JoltShapeType.Box:
                    return new BoxShape(data.halfExtent, data.convexRadius);

                case JoltShapeType.Sphere:
                    return new SphereShape(data.radius);

                case JoltShapeType.Ellipsoid:
                {
                    var sphere = new SphereShape(1.0f);
                    var scaled = new ScaledShape(sphere, data.radii);
                    sphere.Dispose();
                    return scaled;
                }

                case JoltShapeType.Mesh:
                {
                    if (data.vertices == null || data.triangles == null)
                        throw new ArgumentException("Mesh shape requires vertices and triangles.");
                    using var settings = new MeshShapeSettings(data.vertices, data.triangles);
                    return settings.CreateShape();
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(data.type), data.type, "Unknown shape type.");
            }
        }

        static int GetBroadPhaseLayerCount(LayerData[] layers)
        {
            int max = 0;
            foreach (var layer in layers)
            {
                if (layer.broadPhaseLayer > max)
                    max = layer.broadPhaseLayer;
            }
            return max + 1;
        }
    }
}
