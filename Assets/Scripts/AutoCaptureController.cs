using UnityEngine;
using UnityEditor;

public class AutoCaptureController : MonoBehaviour {
    [Range(0, 20)]
    public int pointDensity = 10;
    public bool combineMeshes = false;

    // Start is called before the first frame update
    void Start() {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        if (combineMeshes) {
            // if want to combine meshes under this gameObject
            if (!mf) {
                // if game object don't have a mesh filter
                mf = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter; // create a mesh filter component
                MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
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
        }
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

    void SpawnPoints(Mesh mesh) {

    }

    bool IsInside(Mesh mesh, Vector3 point) {
        return true;
    }
}
