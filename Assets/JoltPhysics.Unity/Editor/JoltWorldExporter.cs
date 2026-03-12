// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.IO;
using JoltPhysics;
using UnityEditor;
using UnityEngine;

namespace JoltPhysics.Unity.Editor
{
    static class JoltWorldExporter
    {
        [MenuItem("Jolt Physics/Export World Data")]
        static void ExportMenuItem()
        {
            var data = ExportWorldData();
            if (data == null) return;

            string path = EditorUtility.SaveFilePanel(
                "Export Jolt World Data", "", "world.json", "json");
            if (string.IsNullOrEmpty(path)) return;

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"Exported Jolt world data to {path} ({data.bodies?.Length ?? 0} bodies)");
        }

        [MenuItem("Jolt Physics/Import World Data")]
        static void ImportMenuItem()
        {
            string path = EditorUtility.OpenFilePanel(
                "Import Jolt World Data", "", "json");
            if (string.IsNullOrEmpty(path)) return;

            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<WorldData>(json);
            if (data == null)
            {
                Debug.LogError("Failed to parse world data.");
                return;
            }

            CreateUnityWorld(data);
            Debug.Log($"Imported Jolt world data from {path} ({data.bodies?.Length ?? 0} bodies)");
        }

        /// <summary>
        /// Exports the current scene's Jolt physics configuration to a <see cref="WorldData"/>.
        /// </summary>
        public static WorldData ExportWorldData()
        {
            var settings = JoltPhysicsSettings.Instance;
            if (settings == null)
            {
                Debug.LogError("JoltPhysicsSettings not found.");
                return null;
            }

            // Global settings
            var data = new WorldData
            {
                gravity = settings.Gravity.ToJolt(),
                maxBodies = settings.MaxBodies,
                maxContactConstraints = settings.MaxContactConstraints,
                collisionSteps = settings.CollisionSteps,
            };

            // Layers
            var unityLayers = settings.Layers;
            data.layers = new LayerData[unityLayers.Length];
            for (int i = 0; i < unityLayers.Length; i++)
            {
                data.layers[i] = new LayerData
                {
                    name = unityLayers[i].name,
                    broadPhaseLayer = unityLayers[i].broadPhaseLayer,
                    collisionMask = GetCollisionMaskForLayer(settings, i),
                };
            }

            // Bodies
            var joltBodies = Object.FindObjectsByType<JoltBody>(FindObjectsSortMode.None);
            data.bodies = new BodyData[joltBodies.Length];

            for (int i = 0; i < joltBodies.Length; i++)
            {
                data.bodies[i] = ExportBody(joltBodies[i]);
            }

            return data;
        }

        static uint GetCollisionMaskForLayer(JoltPhysicsSettings settings, int layer)
        {
            uint mask = 0;
            for (int j = 0; j < settings.LayerCount; j++)
            {
                if (settings.GetCollisionEnabled(layer, j))
                    mask |= 1u << j;
            }
            return mask;
        }

        static BodyData ExportBody(JoltBody body)
        {
            var so = new SerializedObject(body);
            var t = body.transform;

            var bodyData = new BodyData
            {
                name = body.gameObject.name,
                position = t.position.ToJolt(),
                rotation = t.rotation.ToJolt(),
                motionType = (MotionType)so.FindProperty("_motionType").enumValueIndex,
                objectLayer = (uint)so.FindProperty("_objectLayer").longValue,
                friction = so.FindProperty("_friction").floatValue,
                restitution = so.FindProperty("_restitution").floatValue,
                linearDamping = so.FindProperty("_linearDamping").floatValue,
                angularDamping = so.FindProperty("_angularDamping").floatValue,
                gravityFactor = so.FindProperty("_gravityFactor").floatValue,
            };

            var shape = body.GetComponent<JoltShape>();
            if (shape != null)
                bodyData.shape = ExportShape(shape);

            return bodyData;
        }

        static ShapeData ExportShape(JoltShape shape)
        {
            switch (shape)
            {
                case JoltBoxShape box:
                    return ShapeData.CreateBox(
                        box.Center.ToJolt(),
                        (box.Size * 0.5f).ToJolt(),
                        box.ConvexRadius);

                case JoltSphereShape sphere:
                    return ShapeData.CreateSphere(sphere.Radius);

                case JoltEllipsoidShape ellipsoid:
                    return ShapeData.CreateEllipsoid(ellipsoid.Radii.ToJolt());

                case JoltMeshShape meshShape:
                    if (meshShape.Mesh == null)
                    {
                        Debug.LogWarning($"MeshShape on '{meshShape.gameObject.name}' has no mesh assigned.", meshShape);
                        return null;
                    }
                    var unityVerts = meshShape.Mesh.vertices;
                    var unityTris = meshShape.Mesh.triangles;
                    var verts = new Float3[unityVerts.Length];
                    for (int i = 0; i < unityVerts.Length; i++)
                        verts[i] = unityVerts[i].ToJolt();
                    return ShapeData.CreateMesh(verts, unityTris);

                default:
                    Debug.LogWarning($"Unknown shape type on '{shape.gameObject.name}'.", shape);
                    return null;
            }
        }

        /// <summary>
        /// Creates Unity GameObjects from <see cref="WorldData"/>.
        /// </summary>
        public static void CreateUnityWorld(WorldData data)
        {
            if (data == null) return;

            Undo.IncrementCurrentGroup();
            int undoGroup = Undo.GetCurrentGroup();

            // Apply layer settings
            ApplySettingsFromData(data);

            // Ensure JoltPhysicsWorld exists
            var world = Object.FindFirstObjectByType<JoltPhysicsWorld>();
            if (world == null)
            {
                var worldGo = new GameObject("JoltPhysicsWorld");
                Undo.RegisterCreatedObjectUndo(worldGo, "Create JoltPhysicsWorld");
                world = worldGo.AddComponent<JoltPhysicsWorld>();
            }

            // Create bodies
            if (data.bodies != null)
            {
                foreach (var bodyData in data.bodies)
                {
                    if (bodyData?.shape == null) continue;
                    CreateUnityBody(bodyData);
                }
            }

            Undo.SetCurrentGroupName("Import Jolt World");
            Undo.CollapseUndoOperations(undoGroup);
        }

        static void ApplySettingsFromData(WorldData data)
        {
            var settings = JoltPhysicsSettings.Instance;
            if (settings == null) return;

            Undo.RecordObject(settings, "Apply Jolt World Settings");

            var so = new SerializedObject(settings);
            so.Update();

            so.FindProperty("_gravity").vector3Value = new Vector3(data.gravity.x, data.gravity.y, data.gravity.z);
            so.FindProperty("_collisionSteps").intValue = data.collisionSteps;
            so.FindProperty("_maxBodies").longValue = data.maxBodies;
            so.FindProperty("_maxContactConstraints").longValue = data.maxContactConstraints;

            // Layers
            if (data.layers != null && data.layers.Length > 0)
            {
                var layersProp = so.FindProperty("_layers");
                layersProp.arraySize = data.layers.Length;
                for (int i = 0; i < data.layers.Length; i++)
                {
                    var elem = layersProp.GetArrayElementAtIndex(i);
                    elem.FindPropertyRelative("name").stringValue = data.layers[i].name;
                    elem.FindPropertyRelative("broadPhaseLayer").intValue = data.layers[i].broadPhaseLayer;
                }

                // Collision masks
                var maskProp = so.FindProperty("_collisionMask");
                maskProp.arraySize = data.layers.Length;
                for (int i = 0; i < data.layers.Length; i++)
                {
                    maskProp.GetArrayElementAtIndex(i).longValue = data.layers[i].collisionMask;
                }
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(settings);
        }

        static void CreateUnityBody(BodyData bodyData)
        {
            var go = new GameObject(string.IsNullOrEmpty(bodyData.name) ? "JoltBody" : bodyData.name);
            Undo.RegisterCreatedObjectUndo(go, "Create Jolt Body");

            go.transform.position = new Vector3(bodyData.position.x, bodyData.position.y, bodyData.position.z);
            go.transform.rotation = new Quaternion(bodyData.rotation.x, bodyData.rotation.y,
                bodyData.rotation.z, bodyData.rotation.w);

            // Add shape component
            var shapeData = bodyData.shape;
            switch (shapeData.type)
            {
                case JoltShapeType.Box:
                {
                    var box = go.AddComponent<JoltBoxShape>();
                    box.Center = new Vector3(shapeData.center.x, shapeData.center.y, shapeData.center.z);
                    box.Size = new Vector3(shapeData.halfExtent.x * 2f, shapeData.halfExtent.y * 2f,
                        shapeData.halfExtent.z * 2f);
                    box.ConvexRadius = shapeData.convexRadius;
                    break;
                }
                case JoltShapeType.Sphere:
                {
                    var sphere = go.AddComponent<JoltSphereShape>();
                    sphere.Radius = shapeData.radius;
                    break;
                }
                case JoltShapeType.Ellipsoid:
                {
                    var ellipsoid = go.AddComponent<JoltEllipsoidShape>();
                    ellipsoid.Radii = new Vector3(shapeData.radii.x, shapeData.radii.y, shapeData.radii.z);
                    break;
                }
                case JoltShapeType.Mesh:
                {
                    var meshShape = go.AddComponent<JoltMeshShape>();
                    if (shapeData.vertices != null && shapeData.triangles != null)
                    {
                        meshShape.Mesh = JoltMeshShape.ToUnityMesh(
                            shapeData.vertices, shapeData.triangles, bodyData.name + "_mesh");
                    }
                    break;
                }
            }

            // Add body component and set properties via SerializedObject
            var body = go.AddComponent<JoltBody>();
            var so = new SerializedObject(body);
            so.Update();
            so.FindProperty("_motionType").enumValueIndex = (int)bodyData.motionType;
            so.FindProperty("_objectLayer").longValue = bodyData.objectLayer;
            so.FindProperty("_friction").floatValue = bodyData.friction;
            so.FindProperty("_restitution").floatValue = bodyData.restitution;
            so.FindProperty("_linearDamping").floatValue = bodyData.linearDamping;
            so.FindProperty("_angularDamping").floatValue = bodyData.angularDamping;
            so.FindProperty("_gravityFactor").floatValue = bodyData.gravityFactor;
            so.ApplyModifiedProperties();
        }
    }
}
