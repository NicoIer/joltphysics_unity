// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Collections.Generic;
using JoltPhysics;
using UnityEngine;

namespace JoltPhysics.Unity
{
    public sealed class JoltPhysicsWorld : MonoBehaviour
    {
        public const uint LayerStatic = 0;
        public const uint LayerDynamic = 1;
        const uint NumObjectLayers = 2;
        const uint NumBroadPhaseLayers = 2;
        const byte BroadPhaseLayerStatic = 0;
        const byte BroadPhaseLayerDynamic = 1;

        public static JoltPhysicsWorld Instance { get; private set; }

        [Header("Physics Settings")]
        [SerializeField] Vector3 _gravity = new(0, -9.81f, 0);
        [SerializeField] int _collisionSteps = 1;
        [SerializeField] uint _maxBodies = 10240;
        [SerializeField] uint _maxContactConstraints = 10240;

        PhysicsSystem _physicsSystem;
        JobSystemThreadPool _jobSystem;
        BroadPhaseLayerInterfaceTable _broadPhaseLayerInterface;
        ObjectLayerPairFilterTable _objectLayerPairFilter;
        ObjectVsBroadPhaseLayerFilterTable _objectVsBroadPhaseLayerFilter;

        readonly List<JoltBody> _bodies = new();
        bool _initialized;

        public BodyInterface BodyInterface { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple JoltPhysicsWorld instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Initialize();
        }

        void Initialize()
        {
            if (_initialized) return;

            if (!JoltApi.Init())
            {
                Debug.LogError("Failed to initialize Jolt Physics.");
                return;
            }

            _jobSystem = new JobSystemThreadPool();

            // Set up layer configuration
            _broadPhaseLayerInterface = new BroadPhaseLayerInterfaceTable(NumObjectLayers, NumBroadPhaseLayers);
            _broadPhaseLayerInterface.MapObjectToBroadPhaseLayer(LayerStatic, BroadPhaseLayerStatic);
            _broadPhaseLayerInterface.MapObjectToBroadPhaseLayer(LayerDynamic, BroadPhaseLayerDynamic);

            _objectLayerPairFilter = new ObjectLayerPairFilterTable(NumObjectLayers);
            _objectLayerPairFilter.EnableCollision(LayerStatic, LayerDynamic);
            _objectLayerPairFilter.EnableCollision(LayerDynamic, LayerDynamic);

            _objectVsBroadPhaseLayerFilter = new ObjectVsBroadPhaseLayerFilterTable(
                _broadPhaseLayerInterface, NumBroadPhaseLayers,
                _objectLayerPairFilter, NumObjectLayers);

            var settings = new PhysicsSystemSettings
            {
                MaxBodies = _maxBodies,
                NumBodyMutexes = 0,
                MaxBodyPairs = _maxBodies * 4,
                MaxContactConstraints = _maxContactConstraints,
            };

            _physicsSystem = new PhysicsSystem(
                settings,
                _broadPhaseLayerInterface,
                _objectLayerPairFilter,
                _objectVsBroadPhaseLayerFilter);

            _physicsSystem.Gravity = _gravity.ToJolt();
            BodyInterface = _physicsSystem.GetBodyInterface();

            _initialized = true;
        }

        void FixedUpdate()
        {
            if (!_initialized) return;

            _physicsSystem.Update(Time.fixedDeltaTime, _collisionSteps, _jobSystem);

            // Sync transforms from Jolt to Unity
            for (int i = _bodies.Count - 1; i >= 0; i--)
            {
                var body = _bodies[i];
                if (body == null)
                {
                    _bodies.RemoveAt(i);
                    continue;
                }

                body.SyncTransformFromPhysics();
            }
        }

        public void RegisterBody(JoltBody body)
        {
            if (!_bodies.Contains(body))
                _bodies.Add(body);
        }

        public void UnregisterBody(JoltBody body)
        {
            _bodies.Remove(body);
        }

        void OnDestroy()
        {
            if (Instance != this) return;

            // Remove all bodies first
            for (int i = _bodies.Count - 1; i >= 0; i--)
            {
                if (_bodies[i] != null)
                    _bodies[i].DestroyBody();
            }

            _bodies.Clear();

            _physicsSystem?.Dispose();
            _jobSystem?.Dispose();
            _objectVsBroadPhaseLayerFilter?.Dispose();
            _objectLayerPairFilter?.Dispose();
            _broadPhaseLayerInterface?.Dispose();

            _physicsSystem = null;
            _jobSystem = null;

            if (_initialized)
            {
                JoltApi.Shutdown();
                _initialized = false;
            }

            Instance = null;
        }

        void OnValidate()
        {
            if (_initialized && _physicsSystem != null)
            {
                _physicsSystem.Gravity = _gravity.ToJolt();
            }
        }
    }
}
