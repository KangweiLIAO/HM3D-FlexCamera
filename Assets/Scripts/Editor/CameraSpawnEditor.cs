using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraSpawnController))]
public class CameraSpawnEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        CameraSpawnController controller = (CameraSpawnController)target;

        EditorGUILayout.Space();
        if (controller.meshArea > 0) {
            EditorGUILayout.LabelField("Mesh Area: " + controller.meshArea);
        }
        if (GUILayout.Button("Combine meshes")) {
            controller.Combine();
        }
        EditorGUILayout.Space();
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Enter the play mode to spawn debug points", MessageType.Warning);
            EditorGUILayout.Space();
        } else {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max camera count:");
            controller.setMaxSpawn = EditorGUILayout.IntField(controller.setMaxSpawn);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            if (controller.startDebugging) {
                if (GUILayout.Button("Stop Spawning Debug Points")) {
                    controller.startDebugging = false;
                }
            } else {
                if (GUILayout.Button("Spawn Debug Points") && !controller.startSpawning) {
                    controller.startDebugging = true;
                }
            }
            EditorGUILayout.Space();
            if (controller.startSpawning) {
                if (GUILayout.Button("Stop Cameras Spawning")) {
                    controller.startSpawning = false;
                }
            } else {
                if (GUILayout.Button("Spawn Cameras") && !controller.startDebugging) {
                    controller.startSpawning = true;
                }
            }
        }
    }
}
