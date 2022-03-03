using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PointSpawnController))]
public class PointSpawnEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        PointSpawnController controller = (PointSpawnController)target;
        if (Application.isPlaying) {
            EditorGUILayout.LabelField("Mesh Area: " + controller.meshArea);
            if (controller.spawnStatus) {
                if (GUILayout.Button("Stop Spawning")) {
                    controller.spawnStatus = false;
                }
            } else {
                if (GUILayout.Button("Spawn Points")) {
                    controller.spawnStatus = true;
                }
            }
        } else {
            EditorGUILayout.HelpBox("Random point spawn only avaliable in play mode!", MessageType.Warning);
        }
    }
}
