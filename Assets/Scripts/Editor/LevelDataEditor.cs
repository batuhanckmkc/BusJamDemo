using BusJamDemo.LevelLoad;
using UnityEditor;
using UnityEngine;

namespace BusJamDemo.Editor
{
    [CustomEditor(typeof(LevelData_SO))]
    public class LevelDataEditor : UnityEditor.Editor
    {
        private LevelData_SO TargetLevel => (LevelData_SO)target;
        private int requiredLength => TargetLevel.Rows * TargetLevel.Columns;

        private EmptyCell_SO _emptyCellAsset;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script", "GridCells");

            EditorGUILayout.Space(5);

            if (_emptyCellAsset == null)
            {
                string[] guids = AssetDatabase.FindAssets("t:EmptyCell_SO");
                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    _emptyCellAsset = AssetDatabase.LoadAssetAtPath<EmptyCell_SO>(path);
                }
            }

            if (_emptyCellAsset == null)
            {
                EditorGUILayout.HelpBox("Error: No EmptyCell_SO asset was found in the project. Please create one (Create -> BusJam/Grid Cells/Empty).", MessageType.Error);
                return;
            }

            if (TargetLevel.GridCells == null || TargetLevel.GridCells.Length != requiredLength)
            {
                EditorGUILayout.HelpBox($"The size of the GridCells array does not match the required length of {requiredLength}. It needs to be reinitialized with EmptyCell references.", MessageType.Error);

                if (GUILayout.Button("INITIALIZE Grid with New Dimensions (using EmptyCell)"))
                {
                    ResizeGridArray(requiredLength);
                }
            }
            else
            {
                EditorGUILayout.LabelField($"Grid Size: {TargetLevel.Rows}x{TargetLevel.Columns} ({requiredLength} Cells)", EditorStyles.boldLabel);
                DrawGridEditor();
            }

            serializedObject.ApplyModifiedProperties();
        }


        private void ResizeGridArray(int newLength)
        {
            TargetLevel.GridCells = new CellData_SO[newLength];

            if (_emptyCellAsset != null)
            {
                for (int i = 0; i < newLength; i++)
                {
                    TargetLevel.GridCells[i] = _emptyCellAsset;
                }
            }

            EditorUtility.SetDirty(TargetLevel);
            AssetDatabase.SaveAssets();
        }

        private void DrawGridEditor()
        {
            EditorGUILayout.Space(10);

            int rows = TargetLevel.Rows;
            int columns = TargetLevel.Columns;

            for (int row = 0; row < rows; row++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int col = 0; col < columns; col++)
                {
                    int index = row * columns + col;

                    TargetLevel.GridCells[index] = (CellData_SO)EditorGUILayout.ObjectField(
                        TargetLevel.GridCells[index],
                        typeof(CellData_SO),
                        false,
                        GUILayout.Width(EditorGUIUtility.currentViewWidth / columns - 10),
                        GUILayout.Height(20)
                    );
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}