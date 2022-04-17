using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraSpawnController))]
public class CameraSpawnEditor : Editor {

    CameraSpawnController controller;

    private void OnEnable() {
        controller = (CameraSpawnController)target;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUILayout.Space();
        if (GUILayout.Button("Combine meshes")) {
            controller.Combine();
        }
        EditorGUILayout.Space();
        if (!Application.isPlaying) {
            EditorGUILayout.HelpBox("Enter the play mode to spawn debug points or cameras", MessageType.Warning);
            EditorGUILayout.Space();
        } else {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Max camera count:");
            controller.maxCameraSpawn = EditorGUILayout.IntField(controller.maxCameraSpawn);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (controller.state == CameraSpawnController.State.Idle) {
                // if controller state is idle
                if (GUILayout.Button("Spawn Debug Points")) {
                    controller.state = CameraSpawnController.State.Debuging;
                }
                EditorGUILayout.Space();
                if (GUILayout.Button("Spawn Cameras")) {
                    controller.CleanAllCaches(); // clear all cubemap caches
                    controller.state = CameraSpawnController.State.Spawning;
                }
            }

            if (controller.state == CameraSpawnController.State.Debuging) {
                // if controller state is debugging
                if (GUILayout.Button("Stop Spawning Debug Points")) {
                    controller.state = CameraSpawnController.State.Finish;
                }
            }
        }
    }
}
