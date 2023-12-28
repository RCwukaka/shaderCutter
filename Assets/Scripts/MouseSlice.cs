using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseSlice : MonoBehaviour {

    public GameObject plane;
    public Transform ObjectContainer;

    // How far away from the slice do we separate resulting objects
    public float separation;

    // Do we draw a plane object associated with the slice
    private Plane slicePlane = new Plane();
    public bool drawPlane;
    
    // Reference to the line renderer
    public ScreenLineRenderer lineRenderer;

    private MeshCutter meshCutter;
    private TempMesh biggerMesh, smallerMesh;

    #region Utility Functions

    void DrawPlane(Vector3 start, Vector3 end, Vector3 normalVec)
    {
        Quaternion rotate = Quaternion.FromToRotation(Vector3.up, normalVec);

        plane.transform.localRotation = rotate;
        plane.transform.position = (end + start) / 2;
        plane.SetActive(true);
    }

    #endregion

    // Use this for initialization
    void Start () {
        // Initialize a somewhat big array so that it doesn't resize
        meshCutter = new MeshCutter(256);
	}

    private void OnEnable()
    {
        lineRenderer.OnLineDrawn += OnLineDrawn;
    }

    private void OnDisable()
    {
        lineRenderer.OnLineDrawn -= OnLineDrawn;
    }

    private void OnLineDrawn(Vector3 start, Vector3 end, Vector3 depth)
    {
        var planeTangent = (end - start).normalized;

        // if we didn't drag, we set tangent to be on x
        if (planeTangent == Vector3.zero)
            planeTangent = Vector3.right;

        var normalVec = Vector3.Cross(depth, planeTangent);

        if (drawPlane) DrawPlane(start, end, normalVec);


        SliceObjects(start, normalVec);
    }
    

    void SliceObjects(Vector3 point, Vector3 normal)
    {
        var toSlice = GameObject.FindGameObjectsWithTag("Sliceable");

        // Put results in positive and negative array so that we separate all meshes if there was a cut made
        List<Transform> positive = new List<Transform>(),
            negative = new List<Transform>();

        GameObject obj;
        bool slicedAny = false;
        for (int i = 0; i < toSlice.Length; ++i)
        {
            obj = toSlice[i];
            //Destroy(obj.GetComponent<ObjectRenderer>());
            // We multiply by the inverse transpose of the worldToLocal Matrix, a.k.a the transpose of the localToWorld Matrix
            // Since this is how normal are transformed
            var transformedNormal = ((Vector3)(obj.transform.localToWorldMatrix.transpose * normal)).normalized;
            
            //Convert plane in object's local frame
            slicePlane.SetNormalAndPosition(transformedNormal, obj.transform.InverseTransformPoint(point));

            slicedAny = SliceObject(ref slicePlane, obj, positive, negative) || slicedAny;

            obj.active = false;
        }

        // Separate meshes if a slice was made
        if (slicedAny)
            SeparateMeshes(positive, negative, normal);
    }

    bool SliceObject(ref Plane slicePlane, GameObject obj, List<Transform> positiveObjects, List<Transform> negativeObjects)
    {
        var mesh = obj.GetComponent<MeshFilter>().mesh;

        if (!meshCutter.SliceMesh(mesh, ref slicePlane))
        {
            // Put object in the respective list
            if (slicePlane.GetDistanceToPoint(meshCutter.GetFirstVertex()) >= 0)
                positiveObjects.Add(obj.transform);
            else
                negativeObjects.Add(obj.transform);

            return false;
        }

        // TODO: Update center of mass

        // Silly condition that labels which mesh is bigger to keep the bigger mesh in the original gameobject
        bool posBigger = meshCutter.PositiveMesh.surfacearea > meshCutter.NegativeMesh.surfacearea;
        if (posBigger)
        {
            biggerMesh = meshCutter.PositiveMesh;
            smallerMesh = meshCutter.NegativeMesh;
        }
        else
        {
            biggerMesh = meshCutter.NegativeMesh;
            smallerMesh = meshCutter.PositiveMesh;
        }

        // Create new Sliced object with the other mesh
        GameObject newObject1 = new GameObject();
        MeshFilter newObject1_meshFilter = newObject1.AddComponent<MeshFilter>();
        newObject1.transform.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);
        newObject1.transform.localScale = obj.transform.localScale;
        var newObject1Mesh = newObject1_meshFilter.mesh;
        Material material = obj.GetComponent<MeshRenderer>().material;
        newObject1.AddComponent<MeshRenderer>();
        newObject1.GetComponent<MeshRenderer>().material = material;

        GameObject newObject2 = new GameObject();
        MeshFilter newObject2_meshFilter = newObject2.AddComponent<MeshFilter>();
        newObject2.transform.SetPositionAndRotation(obj.transform.position, obj.transform.rotation);
        newObject2.transform.localScale = obj.transform.localScale;
        var newObject2Mesh = newObject2_meshFilter.mesh;
        newObject2.AddComponent<MeshRenderer>();
        newObject2.GetComponent<MeshRenderer>().material = material;

        CreateStressPlane(material);

        // Put the bigger mesh in the original object
        // TODO: Enable collider generation (either the exact mesh or compute smallest enclosing sphere)
        ReplaceMesh(newObject2Mesh, biggerMesh);
        ReplaceMesh(newObject1Mesh, smallerMesh);

        (posBigger ? positiveObjects : negativeObjects).Add(newObject2.transform);
        (posBigger ? negativeObjects : positiveObjects).Add(newObject1.transform);

        return true;
    }

    private void CreateStressPlane(Material material)
    {
        GameObject plane = new GameObject("Dynamic Stress");
        plane.AddComponent<MeshRenderer>();
        plane.GetComponent<MeshRenderer>().material = material;
        MeshFilter plane_meshFilter = plane.AddComponent<MeshFilter>();
        var planeMesh = plane_meshFilter.mesh;
        planeMesh.SetVertices(meshCutter.BoundaryMesh.vertices);
        planeMesh.SetTriangles(meshCutter.BoundaryMesh.triangles, 0);
        planeMesh.SetColors(meshCutter.BoundaryMesh.colors);

        Vector3 normal = Vector3.zero;
        int i = 0;
        while (normal == Vector3.zero)
        {
            Vector3 a = planeMesh.vertices[planeMesh.triangles[i]];
            Vector3 b = planeMesh.vertices[planeMesh.triangles[i+1]];
            Vector3 c = planeMesh.vertices[planeMesh.triangles[i+2]];
            Vector3 side1 = Vector3.Normalize(b - a);
            Vector3 side2 = Vector3.Normalize(c - a);
            normal = Vector3.Normalize(Vector3.Cross(side1, side2));
            i += 1;
        }
        plane.transform.localScale *= 2;
        
        Vector3 meshCenterPos = planeMesh.bounds.center;
        Vector3 worldHorizontalNormal = new Vector3(1, 0, 0);
        Quaternion rotation = Quaternion.FromToRotation(normal, worldHorizontalNormal);
        plane.transform.SetPositionAndRotation(new Vector3(-meshCenterPos.x, -meshCenterPos.y - 1, -11), rotation);
        planeMesh.RecalculateTangents();
    }

    /// <summary>
    /// Replace the mesh with tempMesh.
    /// </summary>
    void ReplaceMesh(Mesh mesh, TempMesh tempMesh, MeshCollider collider = null)
    {
        mesh.Clear();
        mesh.SetVertices(tempMesh.vertices);
        mesh.SetColors(tempMesh.colors);
        mesh.SetTriangles(tempMesh.triangles, 0);
        mesh.SetNormals(tempMesh.normals);
        mesh.SetUVs(0, tempMesh.uvs);
        
        //mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        if (collider != null && collider.enabled)
        {
            collider.sharedMesh = mesh;
            collider.convex = true;
        }
    }

    void SeparateMeshes(List<Transform> positives, List<Transform> negatives, Vector3 worldPlaneNormal)
    {
        int i;
        var separationVector = worldPlaneNormal * separation;

        for(i = 0; i <positives.Count; ++i)
            positives[i].transform.position += separationVector;

        for (i = 0; i < negatives.Count; ++i)
            negatives[i].transform.position -= separationVector;
    }
}
