using BusJamDemo.LevelLoad;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BusJamDemo.Utility;

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

            // Find all required properties
            SerializedProperty levelIndexProp = serializedObject.FindProperty("LevelIndex");
            SerializedProperty levelTimeProp = serializedObject.FindProperty("Time");
            SerializedProperty busSpawnDistance = serializedObject.FindProperty("BusSpawnDistance");
            SerializedProperty busSpacingX = serializedObject.FindProperty("BusSpacingX");

            SerializedProperty rowsProp = serializedObject.FindProperty("Rows");
            SerializedProperty columnsProp = serializedObject.FindProperty("Columns");
            SerializedProperty boardingCellProp = serializedObject.FindProperty(nameof(TargetLevel.BoardingCellContent));
            SerializedProperty busContentsProp = serializedObject.FindProperty(nameof(TargetLevel.BusContents));
            
            
            // 1. Level Details
            EditorGUILayout.LabelField("--- Level Details ---", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(levelIndexProp);
            EditorGUILayout.PropertyField(busSpawnDistance);
            EditorGUILayout.PropertyField(busSpacingX);

            if (levelTimeProp != null)
            {
                 EditorGUILayout.PropertyField(levelTimeProp);
            }
           
            EditorGUILayout.Space(10);

            // 2. GAME MECHANICS DATA
            EditorGUILayout.LabelField("--- GAME MECHANICS DATA ---", EditorStyles.boldLabel);
            
            EditorGUILayout.PropertyField(boardingCellProp, true); 
            EditorGUILayout.PropertyField(busContentsProp, true);
            
            DrawBusContentInitializer();
            if (busContentsProp.isExpanded && TargetLevel.BusContents != null && TargetLevel.BusContents.Any(x => x == null))
            {
                 EditorGUILayout.HelpBox("Bus Contents listesinde 'None' yazan bozuk elemanlar var. Lütfen 'Element X' yazan bozuk elemanları silin ve yukarıdaki butonlar ile yeniden ekleyin.", MessageType.Error);
            }
            
            EditorGUILayout.Space(10);
            
            // 3. GRID CONTENTS & SIZE
            
            if (_needsResizeAndSave)
            {
                EditorGUILayout.HelpBox("Grid size has changed. Deferring resize and save to next frame.", MessageType.Info);
                EditorApplication.delayCall += ExecuteDelayedResizeAndSave;
                _needsResizeAndSave = false; 
            }
            
            EditorGUILayout.LabelField("--- GRID CONTENTS & SIZE ---", EditorStyles.boldLabel);
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(rowsProp);
            EditorGUILayout.PropertyField(columnsProp);
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                TargetLevel.Rows = Mathf.Max(1, rowsProp.intValue);
                TargetLevel.Columns = Mathf.Max(1, columnsProp.intValue);
                
                if (TargetLevel.GridContents == null || TargetLevel.GridContents.Count != requiredLength)
                {
                    _needsResizeAndSave = true;
                }
            }

            if (TargetLevel.GridContents != null && TargetLevel.GridContents.Count == requiredLength)
            {
                DrawGridEditor();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Adds BusDefinitionData into Bus Contents List
        /// </summary>
        private void DrawBusContentInitializer()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Add Bus Content Manually:", EditorStyles.miniBoldLabel);
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Add New Bus Definition", EditorStyles.miniButton))
            {
                TargetLevel.BusContents.Add(new BusContent()); 
                
                EditorUtility.SetDirty(TargetLevel);
                serializedObject.ApplyModifiedProperties();
                Repaint(); 
            }
            
            if (TargetLevel.BusContents.Count > 0 && GUILayout.Button("Clear All Buses", EditorStyles.miniButton))
            {
                if(EditorUtility.DisplayDialog("Confirm Clear", "Are you sure you want to clear ALL bus definitions?", "Yes", "No"))
                {
                    TargetLevel.BusContents.Clear();
                    EditorUtility.SetDirty(TargetLevel);
                    serializedObject.ApplyModifiedProperties();
                    Repaint(); 
                }
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// Executes the grid resizing and saving on the next editor frame.
        /// </summary>
        private void ExecuteDelayedResizeAndSave()
        {
            if (target == null) return;
            
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
            int columns = CurrentColumns;
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.BeginVertical("Box");
            for (int i = 0; i < CurrentRows; i++)
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
        /// <summary>
        private void DrawCellButton(int index, int columns)
        {
            CellContent currentContent = TargetLevel.GridContents[index];
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            Color baseColor = GetColorForCellType(currentContent);

            if (index == _selectedCellIndex)
            {
                buttonStyle.normal.textColor = Color.yellow;
                buttonStyle.fontStyle = FontStyle.Bold;
                baseColor *= 1.2f;
            }

            GUI.backgroundColor = baseColor;

            string buttonText = $"{currentContent.GetTypeName()}\n({index / columns}, {index % columns})";
            float inspectorWidth = EditorGUIUtility.currentViewWidth;

            float totalPadding = 30f;
            float availableWidth = inspectorWidth - totalPadding;

            float calculatedWidth = availableWidth / columns;

            float buttonHeight = calculatedWidth;

            buttonHeight = Mathf.Max(20f, buttonHeight);
            buttonHeight = Mathf.Min(60f, buttonHeight);
            if (GUILayout.Button(buttonText, buttonStyle, GUILayout.Height(buttonHeight), GUILayout.ExpandWidth(true)))
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
            
            EditorGUILayout.LabelField($"--- Cell Details: ({index / CurrentColumns}, {index % CurrentColumns}) ---", EditorStyles.boldLabel);
            
            // Draw Type selection
            CellContentType selectedType = (CellContentType)EditorGUILayout.EnumPopup("Cell Type", currentContent.Type);
            
            if (selectedType != currentContent.Type)
            {
                TargetLevel.GridContents[index] = CreateNewContentObject(selectedType);
                currentContent = TargetLevel.GridContents[index];
            }

            EditorGUILayout.Space(5);
            
            // Draw Type-Specific Details
            if (currentContent.Type == CellContentType.Passenger)
            {
                PassengerContent passenger = (PassengerContent)currentContent;
                passenger.ColorType = (ColorType)EditorGUILayout.EnumPopup("Passenger Color", passenger.ColorType);
                passenger.PassengerType = (PassengerType)EditorGUILayout.EnumPopup("Passenger Type", passenger.PassengerType); 
            }
            else if (currentContent.Type == CellContentType.Tunnel)
            {
                TunnelContent tunnel = (TunnelContent)currentContent;
                EditorGUILayout.LabelField("Passenger Sequence:", EditorStyles.miniBoldLabel);
                
                // SEQUENCE MANAGEMENT BUTTONS
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add New Passenger", EditorStyles.miniButton))
                {
                    tunnel.PassengerSequence.Add(new PassengerContent { ColorType = ColorType.Red, Type = CellContentType.Passenger });
                    EditorUtility.SetDirty(TargetLevel);
                }
                
                if (GUILayout.Button("Clear Sequence", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to clear the tunnel sequence?", "Clear", "Cancel"))
                    {
                        tunnel.PassengerSequence.Clear();
                        EditorUtility.SetDirty(TargetLevel);
                    }
                }
                EditorGUILayout.EndHorizontal();
                for(int i = 0; i < tunnel.PassengerSequence.Count; i++)
                {
                    PassengerContent sequenceItem = tunnel.PassengerSequence[i];
                    EditorGUILayout.BeginVertical("Box");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"Slot {i} ({sequenceItem.ColorType} - {sequenceItem.PassengerType})", EditorStyles.boldLabel);
                    
                    // Remove button
                    if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
                    {
                        tunnel.PassengerSequence.RemoveAt(i);
                        EditorUtility.SetDirty(TargetLevel);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                    sequenceItem.ColorType = (ColorType)EditorGUILayout.EnumPopup("Passenger Color", sequenceItem.ColorType);
                    sequenceItem.PassengerType = (PassengerType)EditorGUILayout.EnumPopup("Passenger Type", sequenceItem.PassengerType);
                    EditorGUILayout.EndVertical(); // Box End
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        /// <summary>
        /// Provides a visual color hint for the cell button based on its content type.
        /// </summary>
        private Color GetColorForCellType(CellContent cellContent)
        {
            switch (cellContent.Type)
            {
                case CellContentType.Passenger:
                    var color = cellContent as PassengerContent;
                    return color.ColorType.GetColor();
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