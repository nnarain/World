using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldManager))]
public class WorldManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldManager worldManager = (WorldManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            worldManager.Generate();
        }

        if (GUILayout.Button("Destroy Chunks"))
        {
            worldManager.DestroyChunks();
        }
    }
}
