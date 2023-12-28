using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Intersections
{
    #region Static functions

    /// <summary>
    /// Based on https://gdbooks.gitbooks.io/3dcollisions/content/Chapter2/static_aabb_plane.html
    /// </summary>
    public static bool BoundPlaneIntersect(Mesh mesh, ref Plane plane)
    {
        // Compute projection interval radius
        float r = mesh.bounds.extents.x * Mathf.Abs(plane.normal.x) +
            mesh.bounds.extents.y * Mathf.Abs(plane.normal.y) +
            mesh.bounds.extents.z * Mathf.Abs(plane.normal.z);

        // Compute distance of box center from plane
        float s = Vector3.Dot(plane.normal, mesh.bounds.center) - (-plane.distance);

        // Intersection occurs when distance s falls within [-r,+r] interval
        return Mathf.Abs(s) <= r;
    }

    #endregion

    // Initialize fixed arrays so that we don't initialize them every time we call TrianglePlaneIntersect
    private readonly Vector3[] v;
    private readonly Vector2[] u;
    private readonly Color[] c;
    private readonly int[] t;
    private readonly bool[] positive;

    // Used in intersect method
    private Ray edgeRay;

    public Intersections()
    {
        v = new Vector3[3];
        u = new Vector2[3];
        t = new int[3];
        c = new Color[3];
        positive = new bool[3];
    }

    /// <summary>
    /// Find intersection between a plane and a line segment defined by vectors first and second.
    /// </summary>
    public ValueTuple<Vector3, Vector2, Color> Intersect(Plane plane, Vector3 first, Vector3 second, Vector2 uv1, Vector2 uv2, Color c1, Color c2)
    {
        edgeRay.origin = first;
        edgeRay.direction = (second - first).normalized;
        float dist;
        float maxDist = Vector3.Distance(first, second);

        if (!plane.Raycast(edgeRay, out dist))
            // Intersect in wrong direction...
            throw new UnityException("Line-Plane intersect in wrong direction");
        else if (dist > maxDist)
            // Intersect outside of line segment
            throw new UnityException("Intersect outside of line");

        var returnVal = new ValueTuple<Vector3, Vector2, Color>
        {
            Item1 = edgeRay.GetPoint(dist)
        };

        var relativeDist = dist / maxDist;
        // Compute new uv by doing Linear interpolation between uv1 and uv2
        returnVal.Item2.x = Mathf.Lerp(uv1.x, uv2.x, relativeDist);
        returnVal.Item2.y = Mathf.Lerp(uv1.y, uv2.y, relativeDist);

        float r1 = c1.r - c2.r;
        float g1 = c1.g - c2.g;
        float b1 = c1.b - c2.b; 

        float r = c1.r - relativeDist * r1;
        float g = c1.g - relativeDist * g1;
        float b = c1.b - relativeDist * b1;
        returnVal.Item3 = new Color(r, g, b, 1);
        return returnVal;
    }

    /*
     * Small diagram for reference :)
     *       |      |  /|
     *       |      | / |P1       
     *       |      |/  |         
     *       |    I1|   |
     *       |     /|   |
     *      y|    / |   |
     *       | P0/__|___|P2
     *       |      |I2
     *       |      |
     *       |___________________
     */

    public bool TrianglePlaneIntersect(List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, List<Color> colors, int startIdx, ref Plane plane, TempMesh posMesh, TempMesh negMesh, Vector3[] intersectVectors, Color[] intersectColor)
    {
        int i;

        // Store triangle, vertex and uv from indices
        for(i = 0; i < 3; ++i)
        {
            t[i] = triangles[startIdx + i];
            v[i] = vertices[t[i]];
            u[i] = uvs[t[i]];
            c[i] = colors[t[i]];
        }

        // Store wether the vertex is on positive mesh
        posMesh.ContainsKeys(triangles, startIdx, positive);

        // If they're all on the same side, don't do intersection
        if (positive[0] == positive[1] && positive[1] == positive[2])
        {
            // All points are on the same side. No intersection
            // Add them to either positive or negative mesh
            (positive[0] ? posMesh : negMesh).AddOgTriangle(t);
            return false;
        }

        // Find lonely point
        int lonelyPoint = 0;
        if (positive[0] != positive[1])
            lonelyPoint = positive[0] != positive[2] ? 0 : 1;
        else
            lonelyPoint = 2;

        // Set previous point in relation to front face order
        int prevPoint = lonelyPoint - 1;
        if (prevPoint == -1) prevPoint = 2;
        // Set next point in relation to front face order
        int nextPoint = lonelyPoint + 1;
        if (nextPoint == 3) nextPoint = 0;

        // Get the 2 intersection points
        ValueTuple<Vector3, Vector2, Color> newPointPrev = Intersect(plane, v[lonelyPoint], v[prevPoint], u[lonelyPoint], u[prevPoint], c[lonelyPoint], c[prevPoint]);
        ValueTuple<Vector3, Vector2, Color> newPointNext = Intersect(plane, v[lonelyPoint], v[nextPoint], u[lonelyPoint], u[nextPoint], c[lonelyPoint], c[nextPoint]);

        //Set the new triangles and store them in respective tempmeshes
        (positive[lonelyPoint] ? posMesh : negMesh).AddSlicedTriangle(t[lonelyPoint], newPointNext.Item1, newPointPrev.Item1, newPointNext.Item2, newPointPrev.Item2, newPointNext.Item3, newPointPrev.Item3);

        (positive[prevPoint] ? posMesh : negMesh).AddSlicedTriangle(t[prevPoint], newPointPrev.Item1, newPointPrev.Item2, newPointPrev.Item3, t[nextPoint]);

        (positive[prevPoint] ? posMesh : negMesh).AddSlicedTriangle(t[nextPoint], newPointPrev.Item1, newPointNext.Item1, newPointPrev.Item2, newPointNext.Item2, newPointNext.Item3, newPointPrev.Item3);

        // We return the edge that will be in the correct orientation for the positive side mesh
        if (positive[lonelyPoint])
        {
            intersectVectors[0] = newPointPrev.Item1;
            intersectVectors[1] = newPointNext.Item1;
            intersectColor[0] = newPointPrev.Item3;
            intersectColor[1] = newPointNext.Item3;
        } else
        {
            intersectVectors[0] = newPointNext.Item1;
            intersectVectors[1] = newPointPrev.Item1;
            intersectColor[0] = newPointNext.Item3;
            intersectColor[1] = newPointPrev.Item3;
        }
        return true;
    }



}
