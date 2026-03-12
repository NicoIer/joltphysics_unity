// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using UnityEngine;

namespace JoltPhysics.Unity
{
    [Serializable]
    public class JoltLayerDefinition
    {
        public string name = "Unnamed";
        /// <summary>
        /// BroadPhase layer index. Typically 0=Static, 1=Dynamic.
        /// Layers sharing the same broadPhase group are tested together.
        /// </summary>
        public byte broadPhaseLayer;
    }

    /// <summary>
    /// Project-wide Jolt Physics configuration.
    /// Stored as a ScriptableObject asset at Resources/JoltPhysicsSettings.
    /// </summary>
    public sealed class JoltPhysicsSettings : ScriptableObject
    {
        const string ResourcePath = "JoltPhysicsSettings";
        const int MaxLayers = 16;

        static JoltPhysicsSettings _instance;

        [Header("Global Physics")]
        [SerializeField] Vector3 _gravity = new(0, -9.81f, 0);
        [SerializeField] int _collisionSteps = 1;
        [SerializeField] uint _maxBodies = 10240;
        [SerializeField] uint _maxContactConstraints = 10240;

        [Header("Layers")]
        [SerializeField] JoltLayerDefinition[] _layers = new[]
        {
            new JoltLayerDefinition { name = "Static", broadPhaseLayer = 0 },
            new JoltLayerDefinition { name = "Dynamic", broadPhaseLayer = 1 },
        };

        /// <summary>
        /// Per-layer collision bitmask. _collisionMask[i] bit j == 1 means layer i collides with layer j.
        /// Symmetric: if layer A collides with B, then B also collides with A.
        /// </summary>
        [SerializeField] uint[] _collisionMask = new uint[]
        {
            // Static:  collides with Dynamic (bit 1)
            0b10,
            // Dynamic: collides with Static (bit 0) + Dynamic (bit 1)
            0b11,
        };

        public static JoltPhysicsSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<JoltPhysicsSettings>(ResourcePath);
#if UNITY_EDITOR
                    if (_instance == null)
                        _instance = CreateDefaultAsset();
#endif
                }
                return _instance;
            }
        }

        public Vector3 Gravity => _gravity;
        public int CollisionSteps => _collisionSteps;
        public uint MaxBodies => _maxBodies;
        public uint MaxContactConstraints => _maxContactConstraints;
        public JoltLayerDefinition[] Layers => _layers;
        public int LayerCount => _layers?.Length ?? 0;

        /// <summary>
        /// Returns the layer index by name, or -1 if not found.
        /// </summary>
        public int GetLayerIndex(string layerName)
        {
            if (_layers == null) return -1;
            for (int i = 0; i < _layers.Length; i++)
            {
                if (_layers[i].name == layerName)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Returns the layer names for display in dropdowns.
        /// </summary>
        public string[] GetLayerNames()
        {
            if (_layers == null) return Array.Empty<string>();
            var names = new string[_layers.Length];
            for (int i = 0; i < _layers.Length; i++)
                names[i] = $"{_layers[i].name} ({i})";
            return names;
        }

        /// <summary>
        /// Whether two object layers should collide with each other.
        /// </summary>
        public bool GetCollisionEnabled(int layerA, int layerB)
        {
            if (_collisionMask == null) return false;
            int n = _layers?.Length ?? 0;
            if (layerA < 0 || layerA >= n || layerB < 0 || layerB >= n) return false;
            if (layerA >= _collisionMask.Length) return false;
            return (_collisionMask[layerA] & (1u << layerB)) != 0;
        }

        /// <summary>
        /// Set whether two object layers should collide. Keeps the matrix symmetric.
        /// </summary>
        public void SetCollisionEnabled(int layerA, int layerB, bool enabled)
        {
            if (_layers == null) return;
            int n = _layers.Length;
            if (layerA < 0 || layerA >= n || layerB < 0 || layerB >= n) return;

            EnsureCollisionMaskSize();

            if (enabled)
            {
                _collisionMask[layerA] |= 1u << layerB;
                _collisionMask[layerB] |= 1u << layerA;
            }
            else
            {
                _collisionMask[layerA] &= ~(1u << layerB);
                _collisionMask[layerB] &= ~(1u << layerA);
            }
        }

        /// <summary>
        /// Ensures the collision mask array matches the current layer count.
        /// </summary>
        public void EnsureCollisionMaskSize()
        {
            int n = _layers?.Length ?? 0;
            if (_collisionMask == null || _collisionMask.Length != n)
            {
                var newMask = new uint[n];
                if (_collisionMask != null)
                {
                    Array.Copy(_collisionMask, newMask,
                        Mathf.Min(_collisionMask.Length, n));
                }
                _collisionMask = newMask;
            }
        }

        /// <summary>
        /// Returns the number of unique broad phase layers used.
        /// </summary>
        public int GetBroadPhaseLayerCount()
        {
            if (_layers == null) return 0;
            int max = 0;
            foreach (var layer in _layers)
            {
                if (layer.broadPhaseLayer > max)
                    max = layer.broadPhaseLayer;
            }
            return max + 1;
        }

#if UNITY_EDITOR
        static JoltPhysicsSettings CreateDefaultAsset()
        {
            var settings = CreateInstance<JoltPhysicsSettings>();

            const string folderPath = "Assets/Resources";
            if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

            string assetPath = $"{folderPath}/{ResourcePath}.asset";
            UnityEditor.AssetDatabase.CreateAsset(settings, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log($"Created JoltPhysicsSettings at {assetPath}");
            return settings;
        }
#endif
    }
}
