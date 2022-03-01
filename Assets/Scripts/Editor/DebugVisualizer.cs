using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshFilter))]
public class DebugVisualizer : Editor {

    private const string EDITOR_NML_KEY = "_normals_length";
    private const string EDITOR_BBX_KEY = "_bounding_box";
    private Mesh mesh;
    private MeshFilter mf;
    private Vector3[] verts;
    private Vector3[] normals;
    private float normalsLength = 1f;
    private bool showBoundingBox = false;

    private void OnEnable() {
        mf = target as MeshFilter;
        if (mf != null) {
            mesh = mf.sharedMesh;
        }
        normalsLength = EditorPrefs.GetFloat(EDITOR_NML_KEY);
        showBoundingBox = EditorPrefs.GetBool(EDITOR_BBX_KEY);
    }

    private void OnSceneGUI() {
        if (mesh == null) {
            return;
        } else {
            Handles.matrix = mf.transform.localToWorldMatrix;
            Handles.color = Color.yellow;
            verts = mesh.vertices;
            normals = mesh.normals;
            int len = mesh.vertexCount;

            // Draw the bounding box of selected mesh:
            if (showBoundingBox) {
                Handles.DrawWireCube(mesh.bounds.center, mesh.bounds.size);
            }

            if (normalsLength != 0) {
                for (int i = 0; i < len; i++) {
                    Handles.DrawLine(verts[i], verts[i] + normals[i] * normalsLength);
                }
            }
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        showBoundingBox = EditorGUILayout.Toggle("Show bounding box", showBoundingBox);
        normalsLength = EditorGUILayout.FloatField("Normals length", normalsLength);
        if (EditorGUI.EndChangeCheck()) {
            EditorPrefs.SetFloat(EDITOR_NML_KEY, normalsLength);
            EditorPrefs.SetBool(EDITOR_BBX_KEY, showBoundingBox);
        }
    }
}