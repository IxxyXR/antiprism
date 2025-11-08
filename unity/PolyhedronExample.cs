/*
   Example Unity Script for Antiprism Plugin

   This demonstrates how to use the Antiprism plugin to create
   and display polyhedra in Unity.

   Usage:
   1. Attach this script to a GameObject in your Unity scene
   2. The script will create a polyhedron mesh and display it
*/

using UnityEngine;
using Antiprism;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PolyhedronExample : MonoBehaviour
{
    [Header("Polyhedron Settings")]
    [Tooltip("Type of polyhedron to display (cube, tet, ico, dodec, oct)")]
    public string polyhedronType = "ico";

    [Tooltip("Scale factor for the polyhedron")]
    public float scale = 1.0f;

    [Tooltip("Auto-rotate the polyhedron")]
    public bool autoRotate = true;

    [Tooltip("Rotation speed (degrees per second)")]
    public Vector3 rotationSpeed = new Vector3(0, 30, 0);

    private MeshFilter meshFilter;
    private Mesh mesh;

    void Start()
    {
        // Get components
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "Antiprism Polyhedron";
        meshFilter.mesh = mesh;

        // Log library version
        Debug.Log("Antiprism Version: " + AntiprismPlugin.GetVersion());

        // Generate the polyhedron
        GeneratePolyhedron();
    }

    void Update()
    {
        if (autoRotate)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Generate the polyhedron mesh using Antiprism
    /// </summary>
    void GeneratePolyhedron()
    {
        using (var geom = new Geometry())
        {
            // Load the polyhedron
            Status status = geom.LoadResource(polyhedronType);
            if (status != Status.OK)
            {
                Debug.LogError($"Failed to load polyhedron '{polyhedronType}': {status}");
                return;
            }

            // Normalize to unit sphere
            geom.Unitize();

            // Scale
            if (scale != 1.0f)
            {
                geom.Scale(scale);
            }

            // Orient faces consistently
            geom.Orient();

            // Apply to Unity mesh
            geom.ApplyToMesh(mesh);

            Debug.Log($"Generated {polyhedronType}: {geom.VertexCount} vertices, {geom.FaceCount} faces");
        }
    }

    /// <summary>
    /// Regenerate the polyhedron (can be called from UI or editor)
    /// </summary>
    public void Regenerate()
    {
        GeneratePolyhedron();
    }

    /// <summary>
    /// Change the polyhedron type at runtime
    /// </summary>
    public void SetPolyhedronType(string type)
    {
        polyhedronType = type;
        GeneratePolyhedron();
    }
}
