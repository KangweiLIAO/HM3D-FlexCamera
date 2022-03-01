using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PointSpawnController))]
public class PointSpawnEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        PointSpawnController controller = (PointSpawnController)target;
        EditorGUILayout.LabelField("Mesh Area: " + controller.meshArea);
        if (GUILayout.Button("Spawn Points")) {
            controller.spawnStatus = true;
        } else if (GUILayout.Button("Stop Spawning")) {
            controller.spawnStatus = false;
        }
    }
}
