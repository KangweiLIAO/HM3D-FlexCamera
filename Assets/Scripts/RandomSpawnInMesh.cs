using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// <para>Class <c>RandomSpawnInMesh</c> add extension methods corresponding to spawning random points in a mesh
/// to <c>Mesh</c> class.</para> This script only made small changes to the script provided by Anisoropos's answer in 
/// <see href="https://answers.unity.com/questions/296458/random-position-inside-mesh.html.">HERE</see>
/// </summary>
public static class RandomSpawnInMesh {

    /// <summary>
    /// Returns the center of a mesh
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>    
    public static Vector3 GetCenter(this Mesh mesh) {
        Vector3 center = Vector3.zero;
        foreach (Vector3 v in mesh.vertices)
            center += v;
        return center / mesh.vertexCount;
    }

    /// <summary>
    /// Returns a random point inside a CONVEX mesh
    /// </summary> 
    public static Vector3 GetRandomPointInConvex(this Mesh m) {
        // Grab two points on the surface
        Vector3 randomPointOnSurfaceA = m.GetRandomPointOnSurface();
        Vector3 randomPointOnSurfaceB = m.GetRandomPointOnSurface();

        // Interpolate between them
        return Vector3.Lerp(randomPointOnSurfaceA, randomPointOnSurfaceB, Random.Range(0f, 1f));
    }

    /// <summary>
    /// Pick a random point on a triangle in given mesh (http://mathworld.wolfram.com/TrianglePointPicking.html)
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static Vector3 GetRandomPointOnSurface(this Mesh m) {
        // Each triangle is 3 integers in a row in Mesh.triangles, so Random(0.. mesh.triangles.Length / 3) * 3
        // will give a random triangle's origin
        int triangleOrigin = Mathf.FloorToInt(Random.Range(0f, m.triangles.Length) / 3f) * 3;

        // Three vertices that forms a triangle (A = origin):
        Vector3 vertexA = m.vertices[m.triangles[triangleOrigin]];
        Vector3 vertexB = m.vertices[m.triangles[triangleOrigin + 1]];
        Vector3 vertexC = m.vertices[m.triangles[triangleOrigin + 2]];

        // Random point in triangle & its extension triangle (a parallelogram)
        // The equation: x = a1v1 + a2v2
        Vector3 vAB = vertexB - vertexA;
        Vector3 vBC = vertexC - vertexB;

        float rAB = Random.Range(0f, 1f);
        float rBC = Random.Range(0f, 1f);
        // May result in a point ouside of the triangle (https://mathworld.wolfram.com/TriangleInterior.html)
        // A -> (random distance) -> B -> (random distance) -> C:
        Vector3 randPoint = vertexA + rAB * vAB + rBC * vBC;

        // Test if the randPoint in the triangle:
        Vector3 dAB = vertexB - vertexA; // direction of A -> B
        Vector3 dBC = vertexC - vertexB; // direction of B -> C
        Vector3 dCA = vertexA - vertexC; // direction of C -> A

        Vector3 dAP = randPoint - vertexA; // direction of A -> random point
        Vector3 dBP = randPoint - vertexB;
        Vector3 dCP = randPoint - vertexC;

        // Compare the vectors, if xP vectors are on the left side of the edges -> inside triangle
        if (Vector3.Cross(dAB, dAP).z >= 0
            && Vector3.Cross(dBC, dBP).z >= 0
            && Vector3.Cross(dCA, dCP).z >= 0) {
            // Find the symmetric to the center of the parallelogram which is on the intersection of
            // side AC with the bisecting line of angle (BA, BC). Given by:
            Vector3 centralPoint = (vertexA + vertexC) / 2;

            // And the symmetric point is given by the equation c - p = p_Sym - c => p_Sym = 2c - p
            Vector3 symmetricRandPoint = 2 * centralPoint - randPoint;
            randPoint = symmetricRandPoint;
        }
        return randPoint;
    }

    /// <summary>
    /// Calculate the total mesh area of a mesh
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    public static double GetTotalArea(this Mesh mesh) {
        Vector3[] verts = mesh.vertices;
        int[] triangles = mesh.triangles;
        double meshArea = 0;
        List<double> areas = new List<double>();
        for (int i = 0; i < triangles.Length; i += 3) {
            double area = 0.0;
            Vector3 corner = verts[triangles[i]];
            Vector3 edge1 = verts[triangles[i + 1]] - corner;
            Vector3 edge2 = verts[triangles[i + 2]] - corner;
            area += Vector3.Cross(edge1, edge2).magnitude;
            areas.Add(area / 2); // add each triangle's area into the list
            meshArea += area;
        }
        meshArea /= 2;
        return meshArea;
    }
}
