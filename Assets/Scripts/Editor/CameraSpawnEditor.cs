using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraSpawnController))]
public class CameraSpawnEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        CameraSpawnController controller = (CameraSpawnController)target;
        
        if (!controller.startCombineMesh) {
            if (GUILayout.Button("Combine meshes under this object")) {
                controller.Combine();
                EditorGUILayout.LabelField("Mesh Area: " + controller.meshArea);
            }
        }
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Enter the play mode to spawn debug points", MessageType.Warning);
        } else {
            if (controller.startSpawning) {
                if (GUILayout.Button("Stop Points Spawning")) {
                    controller.startSpawning = false;
                }
            } else {
                if (GUILayout.Button("Spawn Debug Points")) {
                    controller.startSpawning = true;
                }
            }
        }
    }
}
