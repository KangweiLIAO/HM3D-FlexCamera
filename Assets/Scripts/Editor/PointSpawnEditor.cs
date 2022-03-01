using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PointSpawnController))]
public class PointSpawnEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        PointSpawnController controller = (PointSpawnController)target;
        if (GUILayout.Button("Spawn Points")) {
            controller.SpawnPoints();
        } else if (GUILayout.Button("Stop Spawning")) {
            controller.StopSpawnPoints();
        }
    }
}
