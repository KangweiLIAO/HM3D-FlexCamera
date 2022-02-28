using UnityEngine;
using UnityEditor;

public class PointSpawnController : MonoBehaviour {
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
                for (int i = 0; i < meshFilters.Length; i++) {
                    combine[i].mesh = meshFilters[i].sharedMesh;
                    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    i++;
                }
                mf.mesh = new Mesh();   // create a new mesh to apply combined mesh
                // set UInt32 to contains large scale of verties:
                mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; 
                mf.mesh.CombineMeshes(combine);
                CreateCombinedMeshAssets(mf.mesh);
            }
        }
    }

    void CreateCombinedMeshAssets(Mesh mesh) {
        // find asset with name CombinedMesh in folder "Assets/Generated":
        string[] guids = AssetDatabase.FindAssets(gameObject.name, new[] { "Assets/Resources/CombinedMesh/" });
        foreach (var asset in guids) {
            // delete the old cubemap textures
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(mesh, "Assets/Resources/CombinedMesh/" + gameObject.name + ".mat");
        Debug.Log(AssetDatabase.GetAssetPath(mesh));    // Print the path of the saved asset
    }

    void SpawnPoints(Mesh mesh) {

    }

    bool IsInside(Mesh mesh, Vector3 point) {
        return true;
    }
}
