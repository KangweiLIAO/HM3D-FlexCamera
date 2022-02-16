using UnityEngine;

public class GizmosDebug : MonoBehaviour
{   
    void OnDrawGizmosSelected() {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        if (mf) {
            Mesh mesh = mf.mesh;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(mesh.bounds.center, 0.1f);  // draw bound center
            Gizmos.DrawWireCube(mesh.bounds.center, mesh.bounds.size);
        }
    }
}
