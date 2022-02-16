using UnityEngine;
using UnityEditor;

public class AutoCaptureController : MonoBehaviour {
    public bool combineMeshes = false;
    public GameObject sourceGroup;  // HM3D dataset model that contains a group of sub-meshes

    // Start is called before the first frame update
    void Start() {
        MeshFilter mf = sourceGroup.GetComponent<MeshFilter>();
        if (!mf && combineMeshes) {
            mf = sourceGroup.AddComponent(typeof(MeshFilter)) as MeshFilter; // create a mesh filter component
            MeshFilter[] meshFilters = sourceGroup.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length) {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                i++;
            }
            mf.mesh = new Mesh();   // create a new mesh to apply combined mesh
            mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // set UInt32 to contains large scale of verties
            mf.mesh.CombineMeshes(combine);
            CreateCombinedMeshAssets(mf.mesh);
        }
        SpawnInnerRandomPoints(mf.mesh, false);
    }

    // Update is called once per frame
    void Update() {

    }

    static void CreateCombinedMeshAssets(Mesh mesh) {
        // find asset with name CombinedMesh in folder "Assets/Generated":
        string[] guids = AssetDatabase.FindAssets("CombinedMesh", new[] { "Assets/Generated" });
        foreach (var asset in guids) {
            // delete the old cubemap textures
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(mesh, "Assets/Generated/CombinedMesh.mat");
        Debug.Log(AssetDatabase.GetAssetPath(mesh));    // Print the path of the saved asset
    }

    void SpawnInnerRandomPoints(Mesh mesh, bool convexity) {

    }

    bool IsInside(Mesh mesh, Vector3 point) {
        if (!mesh.bounds.Contains(point)) {
            return false;
        }
        return false;
    }
}
