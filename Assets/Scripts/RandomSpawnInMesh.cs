using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class <c>RandomSpawnInMesh</c> add extension methods corresponding to spawning random
/// points in a mesh to <c>Mesh</c> class. This script only made small changes to the script
/// provided by Anisoropos's answer in https://answers.unity.com/questions/296458/random-position-inside-mesh.html.
/// </summary>
public static class RandomSpawnInMesh {
    /// <summary>
    /// Picks a random point inside a CONVEX mesh.
    /// Taking advantage of Convexity, we can produce more evenly distributed points
    /// </summary> 
    public static Vector3 GetRandomPointInConvex(this Mesh m) {
        // Grab two points on the surface
        Vector3 randomPointOnSurfaceA = m.GetRandomPointOnSurface();
        Vector3 randomPointOnSurfaceB = m.GetRandomPointOnSurface();

        // Interpolate between them
        return Vector3.Lerp(randomPointOnSurfaceA, randomPointOnSurfaceB, Random.Range(0f, 1f));
    }

    /// <summary>
    /// Picks a random point inside a NON-CONVEX mesh.
    /// The only way to get good approximations is by providing a point (if there is one)
    /// that has line of sight to most other points in the non-convex shape.
    /// </summary> 
    public static Vector3 GetRandomPointInNonConvex(this Mesh m, Vector3 pointWhichSeesAll) {
        // Grab one point (and the center which we assume has line of sight with this point)
        Vector3 randomPointOnSurface = m.GetRandomPointOnSurface();

        // Interpolate between them
        return Vector3.Lerp(pointWhichSeesAll, randomPointOnSurface, Random.Range(0f, 1f));
    }

    public static Vector3 GetRandomPointOnSurface(this Mesh m) {
        // Pick a random point on the triangle (http://mathworld.wolfram.com/TrianglePointPicking.html)
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
    /// Returns the mesh's center.
    /// </summary> 
    public static Vector3 GetCenter(this Mesh m) {
        Vector3 center = Vector3.zero;
        foreach (Vector3 v in m.vertices)
            center += v;
        return center / m.vertexCount;
    }

    public static double GetTotalArea(this Mesh m) {
        Vector3[] verts = m.vertices;
        int[] triangles = m.triangles;
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
