using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshFilter))]
public class CustomMeshEditor : Editor {
    private const string EDITOR_BBX_KEY = "_bounding_box";
    private const string EDITOR_SNM_KEY = "_show_normals";
    private const string EDITOR_NML_KEY = "_normals_length";

    private Mesh mesh;
    private MeshFilter mf;
    private Vector3[] verts;
    private Vector3[] normals;
    private bool showNormals = false;
    private float normalsLength = 1f;
    private bool showBoundingBox = false;

    private void OnEnable() {
        mf = target as MeshFilter;
        if (mf != null) {
            mesh = mf.sharedMesh;
        }
        showBoundingBox = EditorPrefs.GetBool(EDITOR_BBX_KEY);
        showNormals = EditorPrefs.GetBool(EDITOR_SNM_KEY);
        normalsLength = EditorPrefs.GetFloat(EDITOR_NML_KEY);
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

            if (showNormals) {
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
        showNormals = EditorGUILayout.Toggle("Show normals", showNormals);
        normalsLength = EditorGUILayout.Slider(label:"Normals length", normalsLength, 0, 1);
        if (EditorGUI.EndChangeCheck()) {
            EditorPrefs.SetFloat(EDITOR_NML_KEY, normalsLength);
            EditorPrefs.SetBool(EDITOR_BBX_KEY, showBoundingBox);
            EditorPrefs.SetBool(EDITOR_SNM_KEY, showNormals);
        }
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Rotate if the combined mesh does not match.", MessageType.Info);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rotate 90\u00B0")) {
            RotateMeshX(90f);
        }
        if (GUILayout.Button("Rotate -90\u00B0")) {
            RotateMeshX(-90f);
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Rotate the mesh around X-axis with certain angle
    /// </summary>
    /// <param name="degree"></param>
    void RotateMeshX(float degree) {
        // Fixes mesh's rotation after importing it from HM3D.
        MeshFilter meshFilter = (MeshFilter)target;
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] newVertices = new Vector3[vertices.Length];
        Quaternion rotation = Quaternion.Euler(degree, 0f, 0f);
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 vertex = vertices[i];
            newVertices[i] = rotation * vertex;
        }
        mesh.vertices = newVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        EditorUtility.SetDirty(meshFilter);
    }
}