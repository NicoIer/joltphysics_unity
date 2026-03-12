// Copyright (c) 2026 NicoIer and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using UnityEditor;
using UnityEngine;

namespace JoltPhysics.Unity.Editor
{
    static class JoltPhysicsSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new SettingsProvider("Project/Jolt Physics", SettingsScope.Project)
            {
                label = "Jolt Physics",
                guiHandler = OnGUI,
                keywords = new[] { "Jolt", "Physics", "Layer", "Collision" }
            };
        }

        static bool _collisionMatrixFoldout = true;
        static Vector2 _scrollPos;

        static void OnGUI(string searchContext)
        {
            var settings = JoltPhysicsSettings.Instance;
            if (settings == null)
            {
                EditorGUILayout.HelpBox(
                    "JoltPhysicsSettings asset not found. Click below to create one.",
                    MessageType.Warning);
                if (GUILayout.Button("Create Settings Asset"))
                    _ = JoltPhysicsSettings.Instance;
                return;
            }

            var so = new SerializedObject(settings);
            so.Update();

            EditorGUILayout.LabelField("Global Physics", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty("_gravity"));
            EditorGUILayout.PropertyField(so.FindProperty("_collisionSteps"));
            EditorGUILayout.PropertyField(so.FindProperty("_maxBodies"));
            EditorGUILayout.PropertyField(so.FindProperty("_maxContactConstraints"));

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Object Layers", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(so.FindProperty("_layers"), true);

            so.ApplyModifiedProperties();

            // Collision Matrix
            EditorGUILayout.Space(10);
            _collisionMatrixFoldout = EditorGUILayout.Foldout(_collisionMatrixFoldout,
                "Layer Collision Matrix", true, EditorStyles.foldoutHeader);

            if (_collisionMatrixFoldout)
            {
                settings.EnsureCollisionMaskSize();
                DrawCollisionMatrix(settings);
            }

            if (GUI.changed)
                EditorUtility.SetDirty(settings);
        }

        const float CheckboxSize = 16f;
        const float LabelWidth = 100f;
        const float RotatedLabelHeight = 80f;

        static void DrawCollisionMatrix(JoltPhysicsSettings settings)
        {
            int n = settings.LayerCount;
            if (n == 0)
            {
                EditorGUILayout.HelpBox("No layers defined.", MessageType.Info);
                return;
            }

            var layers = settings.Layers;
            float matrixWidth = LabelWidth + n * CheckboxSize + 20f;

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos,
                GUILayout.MinHeight(RotatedLabelHeight + n * (CheckboxSize + 2f) + 20f));

            // --- Rotated column headers at the top (right-aligned, like Unity Physics) ---
            var headerRect = GUILayoutUtility.GetRect(matrixWidth, RotatedLabelHeight);
            var savedMatrix = GUI.matrix;

            for (int col = 0; col < n; col++)
            {
                // Each column header positioned above its checkbox column
                // Columns go right-to-left: column 0 is rightmost (matching Unity style)
                int visualCol = n - 1 - col;
                float x = headerRect.x + LabelWidth + visualCol * CheckboxSize + CheckboxSize * 0.5f;
                float y = headerRect.y + RotatedLabelHeight;

                var pivot = new Vector2(x, y);
                GUIUtility.RotateAroundPivot(-45f, pivot);

                var labelRect = new Rect(x - 4f, y - RotatedLabelHeight, RotatedLabelHeight, CheckboxSize);
                GUI.Label(labelRect, layers[col].name, EditorStyles.miniLabel);

                GUI.matrix = savedMatrix;
            }

            // --- Rows with checkboxes ---
            for (int row = 0; row < n; row++)
            {
                var rowRect = GUILayoutUtility.GetRect(matrixWidth, CheckboxSize + 2f);

                // Row label on the left
                var labelRect = new Rect(rowRect.x, rowRect.y, LabelWidth, CheckboxSize);
                GUI.Label(labelRect, layers[row].name, EditorStyles.label);

                // Checkboxes: only draw the triangle (row collides with columns >= row)
                // Columns are drawn right-to-left so column 0 is on the far right
                for (int col = n - 1; col >= row; col--)
                {
                    int visualCol = n - 1 - col;
                    var toggleRect = new Rect(
                        rowRect.x + LabelWidth + visualCol * CheckboxSize,
                        rowRect.y,
                        CheckboxSize,
                        CheckboxSize);

                    bool current = settings.GetCollisionEnabled(row, col);
                    bool newVal = GUI.Toggle(toggleRect, current, GUIContent.none);
                    if (newVal != current)
                    {
                        Undo.RecordObject(settings, "Change Jolt Collision Matrix");
                        settings.SetCollisionEnabled(row, col, newVal);
                        EditorUtility.SetDirty(settings);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
