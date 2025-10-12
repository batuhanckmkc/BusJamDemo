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
        private int requiredLength => TargetLevel.Rows * TargetLevel.Columns;
        private int _selectedCellIndex = -1;
        // CRITICAL: Defer resizing and saving the list using a flag.
        private bool _needsResizeAndSave = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Check if there are changes in drawing Rows, Columns, or other fields
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script", "GridContents");
            
            // If Rows or Columns have changed, mark the resize flag.
            if (EditorGUI.EndChangeCheck())
            {
                // If the size has changed, STOP drawing the list immediately and prepare for deferral
                if (TargetLevel.GridContents == null || TargetLevel.GridContents.Count != requiredLength)
                {
                    _needsResizeAndSave = true;
                }
                // Save other property changes
                serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUILayout.Space(10);
            
            // Delayed Save Check
            if (_needsResizeAndSave)
            {
                // CRITICAL Defer saving and resizing
                EditorGUILayout.HelpBox("Grid size has changed. Please wait for resizing...", MessageType.Info);
                EditorApplication.delayCall += ExecuteDelayedResizeAndSave;
                _needsResizeAndSave = false; // Reset immediately to prevent repeated calls
            }
            else if (TargetLevel.GridContents != null && TargetLevel.GridContents.Count == requiredLength)
            {
                EditorGUILayout.LabelField("--- GRID CONTENTS ---", EditorStyles.boldLabel);
                DrawGridEditor();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void ExecuteDelayedResizeAndSave()
        {
            if (TargetLevel.GridContents == null || TargetLevel.GridContents.Count != requiredLength)
            {
                ResizeGridListInternal();
                
                EditorUtility.SetDirty(TargetLevel);
                AssetDatabase.SaveAssets(); 
                
                EditorUtility.RequestScriptReload();
            }
        }
        
        private void ResizeGridListInternal()
        {
            var newList = new List<CellContent>(requiredLength);
            for (int i = 0; i < requiredLength; i++)
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

        private void DrawGridEditor()
        {
            int columns = TargetLevel.Columns;
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical("Box");
            for (int i = 0; i < TargetLevel.Rows; i++)
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

        private void DrawCellDetails(int index)
        {
            CellContent currentContent = TargetLevel.GridContents[index];

            EditorGUILayout.BeginVertical("HelpBox");
            
            EditorGUILayout.LabelField($"--- Cell Details: ({index / TargetLevel.Columns}, {index % TargetLevel.Columns}) ---", EditorStyles.boldLabel);
            
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
        
        private Color GetColorForCellType(CellContentType type)
        {
            switch (type)
            {
                case CellContentType.Passenger: return new Color(1f, 0.6f, 0.6f);
                case CellContentType.Tunnel: return new Color(0.6f, 0.6f, 1f); 
                case CellContentType.Empty: default: return Color.grey * 0.8f; 
            }
        }
        
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