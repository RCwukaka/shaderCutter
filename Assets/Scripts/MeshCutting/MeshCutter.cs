using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshCutter
{

    public TempMesh PositiveMesh { get; private set; }
    public TempMesh NegativeMesh { get; private set; }

    public TempMesh BoundaryMesh { get; private set; }

    private List<Vector3> addedPairs;
    private List<Color> addedColors;

    private readonly List<Vector3> ogVertices;
    private readonly List<int> ogTriangles;
    private readonly List<Vector3> ogNormals;
    private readonly List<Color> ogColors;
    private readonly List<Vector2> ogUvs;

    private readonly Vector3[] intersectPair;
    private readonly Color[] intersectColor;
    private readonly Vector3[] tempTriangle;
    private readonly Color[] tempColor;

    private Intersections intersect;

    private readonly float threshold = 1e-6f;

    public MeshCutter(int initialArraySize)
    {
        PositiveMesh = new TempMesh(initialArraySize);
        NegativeMesh = new TempMesh(initialArraySize);
        BoundaryMesh = new TempMesh(initialArraySize);

        addedPairs = new List<Vector3>(initialArraySize);
        addedColors = new List<Color>(initialArraySize);
        ogVertices = new List<Vector3>(initialArraySize);
        ogNormals = new List<Vector3>(initialArraySize);
        ogUvs = new List<Vector2>(initialArraySize);
        ogColors = new List<Color>(initialArraySize);
        ogTriangles = new List<int>(initialArraySize * 3);

        intersectPair = new Vector3[2];
        intersectColor = new Color[2];
        tempTriangle = new Vector3[3];
        tempColor = new Color[3];

        intersect = new Intersections();
    }

    /// <summary>
    /// Slice a mesh by the slice plane.
    /// We assume the plane is already in the mesh's local coordinate frame
    /// Returns posMesh and negMesh, which are the resuling meshes on both sides of the plane 
    /// (posMesh on the same side as the plane's normal, negMesh on the opposite side)
    /// </summary>
    public bool SliceMesh(Mesh mesh, ref Plane slice)
    {

        // Let's always fill the vertices array so that we can access it even if the mesh didn't intersect
        mesh.GetVertices(ogVertices);

        // 1. Verify if the bounds intersect first
        if (!Intersections.BoundPlaneIntersect(mesh, ref slice))
            return false;

        mesh.GetTriangles(ogTriangles, 0);
        mesh.GetNormals(ogNormals);
        mesh.GetUVs(0, ogUvs);
        mesh.GetColors(ogColors);

        PositiveMesh.Clear();
        NegativeMesh.Clear();
        BoundaryMesh.Clear();
        addedPairs.Clear();
        addedColors.Clear();

        // 2. Separate old vertices in new meshes
        for (int i = 0; i < ogVertices.Count; ++i)
        {
            if (slice.GetDistanceToPoint(ogVertices[i]) >= 0)
                PositiveMesh.AddVertex(ogVertices, ogNormals, ogUvs, ogColors, i);
            else
                NegativeMesh.AddVertex(ogVertices, ogNormals, ogUvs, ogColors, i);
        }

        // 2.5 : If one of the mesh has no vertices, then it doesn't intersect
        if (NegativeMesh.vertices.Count == 0 || PositiveMesh.vertices.Count == 0)
            return false;

        // 3. Separate triangles and cut those that intersect the plane
        for (int i = 0; i < ogTriangles.Count; i += 3)
        {
            if (intersect.TrianglePlaneIntersect(ogVertices, ogUvs, ogTriangles, ogColors, i, ref slice, PositiveMesh, NegativeMesh, intersectPair, intersectColor))
                addedPairs.AddRange(intersectPair);
                addedColors.AddRange(intersectColor);       
        }
        if (addedPairs.Count > 0)
        {
            //FillBoundaryGeneral(addedPairs);
            FillBoundaryFace(addedPairs, addedColors);
            return true;
        } else
        {
            throw new UnityException("Error: if added pairs is empty, we should have returned false earlier");
        }
    }

    public Vector3 GetFirstVertex()
    {
        if (ogVertices.Count == 0)
            throw new UnityException(
                "Error: Either the mesh has no vertices or GetFirstVertex was called before SliceMesh.");
        else
            return ogVertices[0];
    }

    private void FillBoundaryFace(List<Vector3> addedPairs, List<Color> addedColors)
    {
        // 1. Reorder added so in order ot their occurence along the perimeter.
        MeshUtils.ReorderList(addedPairs);

        int j = 0;
        while (j < addedPairs.Count)
        {
            if (addedPairs.Contains(addedPairs[j]))
            {
                addedPairs.RemoveAt(j);
                addedColors.RemoveAt(j);
            }
            j++;
        }

        for (int i = 0; i +2 < addedPairs.Count; i+=1) {
            AddTriangle(addedPairs, addedColors, 0, i+1, i+2);
        }
    }

    /// <summary>
    /// Extract polygon from the pairs of vertices.
    /// Per example, two vectors that are colinear is redundant and only forms one side of the polygon
    /// </summary>
    private List<Vector3> FindRealPolygon(List<Vector3> pairs)
    {
        List<Vector3> vertices = new List<Vector3>();
        Vector3 edge1, edge2;

        // List should be ordered in the correct way
        for (int i = 0; i < pairs.Count; i += 2)
        {
            edge1 = (pairs[i + 1] - pairs[i]);
            if (i == pairs.Count - 2)
                edge2 = pairs[1] - pairs[0];
            else
                edge2 = pairs[i + 3] - pairs[i + 2];

            // Normalize edges
            edge1.Normalize();
            edge2.Normalize();

            if (Vector3.Angle(edge1, edge2) > threshold)
                // This is a corner
                vertices.Add(pairs[i + 1]);
        }
        return vertices;
    }

    private void AddTriangle(List<Vector3> face, List<Color> addedColors, int t1, int t2, int t3)
    {
        tempTriangle[0] = face[t1];
        tempTriangle[1] = face[t2];
        tempTriangle[2] = face[t3];
        tempColor[0] = addedColors[t1];
        tempColor[1] = addedColors[t2];
        tempColor[2] = addedColors[t3];
        PositiveMesh.AddTriangle(tempTriangle, tempColor);
        BoundaryMesh.AddTriangle(tempTriangle, tempColor);

        tempTriangle[1] = face[t3];
        tempTriangle[2] = face[t2];
        tempColor[1] = addedColors[t3];
        tempColor[2] = addedColors[t2];
        NegativeMesh.AddTriangle(tempTriangle, tempColor);
    }

}

