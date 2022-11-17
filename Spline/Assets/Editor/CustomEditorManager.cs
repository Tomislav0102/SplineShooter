using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AstarManager))]
public class CustomEditorManager : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    DrawDefaultInspector();

    //    AstarManager aStar = (AstarManager)target;
    //    if (GUILayout.Button("Run Astar"))
    //    {
    //        aStar.RunAstar();
    //    }
    //}

}

[CustomEditor(typeof(EEM_generateGrid))]
public class CustomEditorEEM_generateGrid : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EEM_generateGrid generateGrid = (EEM_generateGrid)target;
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("Create"))
        {
           generateGrid.CreateGrid();
        }
        if (GUILayout.Button("Destroy"))
        {
           generateGrid.DestroyGrid();
        }

        EditorGUILayout.EndVertical();
    }

}
