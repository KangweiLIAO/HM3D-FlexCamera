using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraSpawnController))]
public class CameraSpawnEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        CameraSpawnController controller = (CameraSpawnController)target;
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
