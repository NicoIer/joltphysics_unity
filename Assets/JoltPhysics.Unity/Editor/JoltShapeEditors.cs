// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using UnityEditor;
using UnityEngine;

namespace JoltPhysics.Unity.Editor
{
    [CustomEditor(typeof(JoltBoxShape))]
    public sealed class JoltBoxShapeEditor : UnityEditor.Editor
    {
        SerializedProperty _halfExtent;
        SerializedProperty _convexRadius;

        void OnEnable()
        {
            _halfExtent = serializedObject.FindProperty("_halfExtent");
            _convexRadius = serializedObject.FindProperty("_convexRadius");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_halfExtent);
            EditorGUILayout.PropertyField(_convexRadius);
            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            var shape = (JoltBoxShape)target;
            var t = shape.transform;

            Handles.color = Color.green;
            Handles.matrix = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);

            var halfExtent = shape.HalfExtent;

            EditorGUI.BeginChangeCheck();

            // Draw box handles on each axis
            var newHalfExtent = halfExtent;

            newHalfExtent.x = Handles.ScaleSlider(halfExtent.x, Vector3.zero, Vector3.right, Quaternion.identity, halfExtent.x + 0.5f, 0.01f);
            newHalfExtent.y = Handles.ScaleSlider(halfExtent.y, Vector3.zero, Vector3.up, Quaternion.identity, halfExtent.y + 0.5f, 0.01f);
            newHalfExtent.z = Handles.ScaleSlider(halfExtent.z, Vector3.zero, Vector3.forward, Quaternion.identity, halfExtent.z + 0.5f, 0.01f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(shape, "Change Box Half Extent");
                shape.HalfExtent = new Vector3(
                    Mathf.Max(0.01f, newHalfExtent.x),
                    Mathf.Max(0.01f, newHalfExtent.y),
                    Mathf.Max(0.01f, newHalfExtent.z));
            }

            Handles.matrix = Matrix4x4.identity;
        }
    }

    [CustomEditor(typeof(JoltSphereShape))]
    public sealed class JoltSphereShapeEditor : UnityEditor.Editor
    {
        SerializedProperty _radius;

        void OnEnable()
        {
            _radius = serializedObject.FindProperty("_radius");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_radius);
            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            var shape = (JoltSphereShape)target;
            var t = shape.transform;

            Handles.color = Color.green;

            EditorGUI.BeginChangeCheck();
            float newRadius = Handles.RadiusHandle(t.rotation, t.position, shape.Radius);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(shape, "Change Sphere Radius");
                shape.Radius = Mathf.Max(0.01f, newRadius);
            }
        }
    }

    [CustomEditor(typeof(JoltEllipsoidShape))]
    public sealed class JoltEllipsoidShapeEditor : UnityEditor.Editor
    {
        SerializedProperty _radii;

        void OnEnable()
        {
            _radii = serializedObject.FindProperty("_radii");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_radii);
            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI()
        {
            var shape = (JoltEllipsoidShape)target;
            var t = shape.transform;

            Handles.color = Color.green;
            Handles.matrix = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);

            var radii = shape.Radii;

            EditorGUI.BeginChangeCheck();
            var newRadii = radii;

            newRadii.x = Handles.ScaleSlider(radii.x, Vector3.zero, Vector3.right, Quaternion.identity, radii.x + 0.5f, 0.01f);
            newRadii.y = Handles.ScaleSlider(radii.y, Vector3.zero, Vector3.up, Quaternion.identity, radii.y + 0.5f, 0.01f);
            newRadii.z = Handles.ScaleSlider(radii.z, Vector3.zero, Vector3.forward, Quaternion.identity, radii.z + 0.5f, 0.01f);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(shape, "Change Ellipsoid Radii");
                shape.Radii = new Vector3(
                    Mathf.Max(0.01f, newRadii.x),
                    Mathf.Max(0.01f, newRadii.y),
                    Mathf.Max(0.01f, newRadii.z));
            }

            Handles.matrix = Matrix4x4.identity;
        }
    }

    [CustomEditor(typeof(JoltMeshShape))]
    public sealed class JoltMeshShapeEditor : UnityEditor.Editor
    {
        SerializedProperty _mesh;

        void OnEnable()
        {
            _mesh = serializedObject.FindProperty("_mesh");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_mesh);

            var shape = (JoltMeshShape)target;
            if (shape.Mesh != null)
            {
                EditorGUILayout.HelpBox(
                    $"Vertices: {shape.Mesh.vertexCount}\nTriangles: {shape.Mesh.triangles.Length / 3}",
                    MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
