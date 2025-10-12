using BusJamDemo.LevelLoad;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace BusJamDemo.Editor
{
    [CustomEditor(typeof(LevelData_SO))]
    public class LevelDataEditor : UnityEditor.Editor
    {
        private LevelData_SO TargetLevel => (LevelData_SO)target;
        
        // Ensure minimum 1x1 size for calculation
        private int CurrentRows => Mathf.Max(1, TargetLevel.Rows);
        private int CurrentColumns => Mathf.Max(1, TargetLevel.Columns);
        
        private int requiredLength => CurrentRows * CurrentColumns;
        private int _selectedCellIndex = -1;
        
        // Flag to defer list resizing and saving until the next editor frame
        private bool _needsResizeAndSave = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Find properties for Rows and Columns
            SerializedProperty rowsProp = serializedObject.FindProperty("Rows");
            SerializedProperty columnsProp = serializedObject.FindProperty("Columns");

            // 1. DRAW NON-GRID SIZE PROPERTIES
            DrawPropertiesExcluding(serializedObject, "m_Script", "GridContents", "Rows", "Columns");
            
            // 2. CHECK FOR ROWS/COLUMNS CHANGES ONLY
            EditorGUI.BeginChangeCheck();
            
            // Draw Rows and Columns fields
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(rowsProp);
            EditorGUILayout.PropertyField(columnsProp);
            EditorGUILayout.EndHorizontal();

            // Did Rows or Columns change?
            if (EditorGUI.EndChangeCheck())
            {
                // CRITICAL FIX: Clamp values to ensure non-zero size before applying
                TargetLevel.Rows = Mathf.Max(1, rowsProp.intValue);
                TargetLevel.Columns = Mathf.Max(1, columnsProp.intValue);
                
                // Only set the resize flag if Rows or Columns changed and the current length is incorrect
                if (TargetLevel.GridContents == null || TargetLevel.GridContents.Count != requiredLength)
                {
                    _needsResizeAndSave = true;
                }
            }
            
            // Apply all property modifications (for all fields drawn above)
            serializedObject.ApplyModifiedProperties();
            
            EditorGUILayout.Space(10);
            
            // 3. Delayed Save Check
            if (_needsResizeAndSave)
            {
                EditorGUILayout.HelpBox("Grid size has changed. Deferring resize and save to next frame.", MessageType.Info);
                EditorApplication.delayCall += ExecuteDelayedResizeAndSave;
                _needsResizeAndSave = false; 
            }
            // 4. Normal Grid Drawing - Use the clamped requiredLength
            else if (TargetLevel.GridContents != null && TargetLevel.GridContents.Count == requiredLength)
            {
                EditorGUILayout.LabelField("--- GRID CONTENTS ---", EditorStyles.boldLabel);
                DrawGridEditor();
            }
        }

        /// <summary>
        /// Executes the grid resizing and saving on the next editor frame.
        /// This is the fix for the InvalidOperationException during layout calculation.
        /// </summary>
        private void ExecuteDelayedResizeAndSave()
        {
            if (target == null) return;
            
            // Recalculate based on clamped values
            int currentRequiredLength = CurrentRows * CurrentColumns; 

            if (TargetLevel.GridContents == null || TargetLevel.GridContents.Count != currentRequiredLength)
            {
                ResizeGridListInternal(currentRequiredLength);
                
                EditorUtility.SetDirty(TargetLevel);
                AssetDatabase.SaveAssets(); 
                
                Repaint();
            }
        }
        
        /// <summary>
        /// Resizes the GridContents list, preserving existing data where possible.
        /// </summary>
        private void ResizeGridListInternal(int newLength)
        {
            var newList = new List<CellContent>(newLength);
            for (int i = 0; i < newLength; i++)
            {
                if (i < TargetLevel.GridContents.Count && TargetLevel.GridContents[i] != null)
                {
                    newList.Add(TargetLevel.GridContents[i]);
                }
                else
                {
                    newList.Add(new EmptyContent { Type = CellContentType.Empty });
                }
            }
            TargetLevel.GridContents = newList;
        }

        /// <summary>
        /// Draws the grid of clickable buttons in the Inspector.
        /// </summary>
        private void DrawGridEditor()
        {
            // Use CurrentColumns here to avoid division by zero
            int columns = CurrentColumns;
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.BeginVertical("Box");
            for (int i = 0; i < CurrentRows; i++) // Use CurrentRows
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < columns; j++)
                {
                    int index = i * columns + j;
                    DrawCellButton(index, columns);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            if (_selectedCellIndex != -1 && _selectedCellIndex < requiredLength)
            {
                DrawCellDetails(_selectedCellIndex);
            }

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(TargetLevel);
            }
        }
        
        /// <summary>
        /// Draws a single cell as a clickable button showing its type and coordinates.
        /// </summary>
        private void DrawCellButton(int index, int columns)
        {
            CellContent currentContent = TargetLevel.GridContents[index];
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            Color baseColor = GetColorForCellType(currentContent.Type);
            
            if (index == _selectedCellIndex)
            {
                buttonStyle.normal.textColor = Color.yellow;
                buttonStyle.fontStyle = FontStyle.Bold;
                baseColor *= 1.2f; 
            }
            
            GUI.backgroundColor = baseColor;
            
            string buttonText = $"{currentContent.GetTypeName()}\n({index / columns}, {index % columns})";

            float buttonWidth = (EditorGUIUtility.currentViewWidth / columns) - 18; 
            if (GUILayout.Button(buttonText, buttonStyle, GUILayout.Width(buttonWidth), GUILayout.Height(40)))
            {
                _selectedCellIndex = (_selectedCellIndex == index) ? -1 : index;
            }
            
            GUI.backgroundColor = Color.white;
        }

        /// <summary>
        /// Draws the detail panel for the currently selected cell, allowing type and data modification.
        /// </summary>
        private void DrawCellDetails(int index)
        {
            CellContent currentContent = TargetLevel.GridContents[index];

            EditorGUILayout.BeginVertical("HelpBox");
            
            // Use CurrentColumns in the display string
            EditorGUILayout.LabelField($"--- Cell Details: ({index / CurrentColumns}, {index % CurrentColumns}) ---", EditorStyles.boldLabel);
            
            CellContentType selectedType = (CellContentType)EditorGUILayout.EnumPopup("Cell Type", currentContent.Type);
            
            if (selectedType != currentContent.Type)
            {
                TargetLevel.GridContents[index] = CreateNewContentObject(selectedType);
                currentContent = TargetLevel.GridContents[index];
            }

            EditorGUILayout.Space(5);
            
            if (currentContent.Type == CellContentType.Passenger)
            {
                PassengerContent passenger = (PassengerContent)currentContent;
                passenger.Color = (ColorType)EditorGUILayout.EnumPopup("Passenger Color", passenger.Color);
            }
            else if (currentContent.Type == CellContentType.Tunnel)
            {
                TunnelContent tunnel = (TunnelContent)currentContent;
                EditorGUILayout.LabelField("Passenger Sequence:", EditorStyles.miniLabel);
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Passenger (Red)", EditorStyles.miniButton))
                {
                    tunnel.PassengerSequence.Add(ColorType.Red); 
                }
                if (GUILayout.Button("Clear Sequence", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to clear the tunnel sequence?", "Clear", "Cancel"))
                    {
                        tunnel.PassengerSequence.Clear();
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                for(int i = 0; i < tunnel.PassengerSequence.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    tunnel.PassengerSequence[i] = (ColorType)EditorGUILayout.EnumPopup($"Slot {i}", tunnel.PassengerSequence[i]);
                    if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
                    {
                        tunnel.PassengerSequence.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }

            }
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Provides a visual color hint for the cell button based on its content type.
        /// </summary>
        private Color GetColorForCellType(CellContentType type)
        {
            switch (type)
            {
                case CellContentType.Passenger: return new Color(1f, 0.6f, 0.6f);
                case CellContentType.Tunnel: return new Color(0.6f, 0.6f, 1f); 
                case CellContentType.Empty: default: return Color.grey * 0.8f; 
            }
        }
        
        /// <summary>
        /// Creates a new POCO (CellContent) object based on the selected type.
        /// </summary>
        private CellContent CreateNewContentObject(CellContentType type)
        {
            switch (type)
            {
                case CellContentType.Passenger:
                    return new PassengerContent { Type = type };
                case CellContentType.Tunnel:
                    return new TunnelContent { Type = type };
                case CellContentType.Empty:
                default:
                    return new EmptyContent { Type = CellContentType.Empty };
            }
        }
    }
}