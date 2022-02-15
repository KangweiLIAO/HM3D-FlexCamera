using UnityEngine;
using UnityEditor;

public class AutoCaptureController : MonoBehaviour
{
    public GameObject sourceGroup;  // object that contains a group of mesh

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter mf = sourceGroup.AddComponent(typeof(MeshFilter)) as MeshFilter; // new a mesh filter component
        MeshFilter[] meshFilters = sourceGroup.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
        mf.mesh = new Mesh();   // new combined mesh
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // set UInt32 to contains large scale of verties
        mf.mesh.CombineMeshes(combine);
        UpdateCombinedMesh(mf.mesh);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    static void UpdateCombinedMesh(Mesh mesh) {
        AssetDatabase.CreateAsset(mesh, "Assets/Generated/CombinedMesh.mat");
        Debug.Log(AssetDatabase.GetAssetPath(mesh));    // Print the path of the saved asset
    }
}
