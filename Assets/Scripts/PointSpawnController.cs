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
                // create a mesh filter component for root object
                mf = gameObject.AddComponent<MeshFilter>();
            }
            Mesh cm = Resources.Load("CombinedMeshes/" + gameObject.name) as Mesh;
            if (cm) {
                // if combined mesh exist assign to root mesh filter
                mf.mesh = cm;
                Debug.Log("Combined mesh for " + gameObject.name + " loaded");
            } else {
                MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
                CombineInstance[] combine = new CombineInstance[meshFilters.Length];
                for (int i = 0; i < meshFilters.Length; i++) {
                    if (meshFilters[i].sharedMesh) {
                        combine[i].mesh = meshFilters[i].sharedMesh;
                        combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    }
                }
                if (combine.Length > 0) {
                    mf.mesh = new Mesh();   // create a new mesh to apply combined mesh
                    // set UInt32 to contains large scale of verties:
                    mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                    mf.mesh.CombineMeshes(combine);
                    CreateCombinedMeshAssets(mf.mesh);
                }
            }
        }
    }

    void CreateCombinedMeshAssets(Mesh mesh) {
        // find asset with name CombinedMesh in folder "Assets/Generated":
        string[] guids = AssetDatabase.FindAssets(gameObject.name, new[] { "Assets/Resources/CombinedMeshes/" });
        foreach (var asset in guids) {
            // delete the old cubemap textures
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
        AssetDatabase.CreateAsset(mesh, "Assets/Resources/CombinedMeshes/" + gameObject.name + ".mat");
        //Debug.Log(AssetDatabase.GetAssetPath(mesh));    // Print the path of the saved asset
    }

    void SpawnPoints(Mesh mesh) {

    }

    bool IsInside(Mesh mesh, Vector3 point) {
        return true;
    }
}
