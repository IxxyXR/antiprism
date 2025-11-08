/*
   Example Unity Script for Antiprism Plugin

   This demonstrates how to use the Antiprism plugin to create
   and display polyhedra in Unity.

   Usage:
   1. Attach this script to a GameObject in your Unity scene
   2. Select polyhedron type from dropdown
   3. Adjust parameters in the inspector
   4. Changes update automatically in Edit mode!
*/

using UnityEngine;
using Antiprism;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PolyhedronExample : MonoBehaviour
{
    public enum PolyhedronType
    {
        Tetrahedron,
        Cube,
        Octahedron,
        Dodecahedron,
        Icosahedron,
        TruncatedTetrahedron,
        TruncatedCube,
        TruncatedOctahedron,
        TruncatedDodecahedron,
        TruncatedIcosahedron,
        Cuboctahedron,
        Icosidodecahedron,
        RhombicDodecahedron,
        RhombicTriacontahedron,
        SnubCube,
        SnubDodecahedron,
        Prism3,
        Prism4,
        Prism5,
        Prism6,
        Prism10,
        Antiprism3,
        Antiprism4,
        Antiprism5
    }

    public enum ModifierType
    {
        None,
        Dual,
        Truncate,
        ConvexHull
    }

    [Header("Polyhedron Generator")]
    [Tooltip("Type of polyhedron to display")]
    public PolyhedronType polyhedronType = PolyhedronType.Icosahedron;

    [Header("Modifiers")]
    [Tooltip("Apply a modifier operation")]
    public ModifierType modifier = ModifierType.None;

    [Tooltip("Use flat shading (hard edges) - recommended for polyhedra")]
    public bool flatShading = true;

    [Header("Transform")]
    [Tooltip("Scale factor for the polyhedron")]
    [Range(0.1f, 5.0f)]
    public float scale = 1.0f;

    [Header("Animation")]
    [Tooltip("Auto-rotate the polyhedron")]
    public bool autoRotate = true;

    [Tooltip("Rotation speed (degrees per second)")]
    public Vector3 rotationSpeed = new Vector3(0, 30, 0);

    [Header("Debug")]
    [Tooltip("Visualize face normals (green lines)")]
    public bool debugShowNormals = false;

    [Tooltip("Length of normal visualization lines")]
    [Range(0.1f, 2.0f)]
    public float normalLength = 0.3f;

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

        if (debugShowNormals && mesh != null)
        {
            DrawNormals();
        }
    }

    /// <summary>
    /// Draw normal vectors for debugging
    /// </summary>
    void DrawNormals()
    {
        if (mesh == null || mesh.vertices.Length == 0)
            return;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        // Draw face normals (averaged from triangle vertices)
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Get triangle vertices
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);

            // Calculate face center
            Vector3 center = (v0 + v1 + v2) / 3f;

            // Calculate face normal
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 normal = Vector3.Cross(edge1, edge2).normalized;

            // Draw normal (green for outward, red for inward relative to origin)
            Color normalColor = Vector3.Dot(normal, center.normalized) > 0 ? Color.green : Color.red;
            Debug.DrawLine(center, center + normal * normalLength, normalColor);
        }

        // Optionally draw vertex normals (cyan)
        if (flatShading == false && normals.Length > 0)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 worldPos = transform.TransformPoint(vertices[i]);
                Vector3 worldNormal = transform.TransformDirection(normals[i]).normalized;
                Debug.DrawLine(worldPos, worldPos + worldNormal * normalLength * 0.7f, Color.cyan);
            }
        }
    }

    void OnValidate()
    {
        // Regenerate when values change in the inspector
        // This works in both Edit mode and Play mode!
        if (meshFilter == null)
            meshFilter = GetComponent<MeshFilter>();

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "Antiprism Polyhedron";
        }

        if (meshFilter != null)
        {
            meshFilter.mesh = mesh;

            // Use delayed call to avoid issues during deserialization
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => {
                if (this != null && mesh != null)
                    GeneratePolyhedron();
            };
            #else
            GeneratePolyhedron();
            #endif
        }
    }

    /// <summary>
    /// Get the resource name for the selected polyhedron type
    /// </summary>
    string GetResourceName()
    {
        switch (polyhedronType)
        {
            case PolyhedronType.Tetrahedron: return "tet";
            case PolyhedronType.Cube: return "cube";
            case PolyhedronType.Octahedron: return "oct";
            case PolyhedronType.Dodecahedron: return "dodecahedron";
            case PolyhedronType.Icosahedron: return "ico";
            case PolyhedronType.TruncatedTetrahedron: return "tr_tet";
            case PolyhedronType.TruncatedCube: return "tr_cube";
            case PolyhedronType.TruncatedOctahedron: return "tr_oct";
            case PolyhedronType.TruncatedDodecahedron: return "tr_dod";
            case PolyhedronType.TruncatedIcosahedron: return "tr_ico";
            case PolyhedronType.Cuboctahedron: return "cubo";
            case PolyhedronType.Icosidodecahedron: return "id";
            case PolyhedronType.RhombicDodecahedron: return "rhombic_dodecahedron";
            case PolyhedronType.RhombicTriacontahedron: return "rhombic_triacontahedron";
            case PolyhedronType.SnubCube: return "sn_cube";
            case PolyhedronType.SnubDodecahedron: return "sn_dod";
            case PolyhedronType.Prism3: return "pri3";
            case PolyhedronType.Prism4: return "pri4";
            case PolyhedronType.Prism5: return "pri5";
            case PolyhedronType.Prism6: return "pri6";
            case PolyhedronType.Prism10: return "pri10";
            case PolyhedronType.Antiprism3: return "ant3";
            case PolyhedronType.Antiprism4: return "ant4";
            case PolyhedronType.Antiprism5: return "ant5";
            default: return "ico";
        }
    }

    /// <summary>
    /// Generate the polyhedron mesh using Antiprism
    /// </summary>
    void GeneratePolyhedron()
    {
        if (mesh == null)
            return;

        using (var geom = new Geometry())
        {
            // Load the base polyhedron
            string resourceName = GetResourceName();
            Status status = geom.LoadResource(resourceName);
            if (status != Status.OK)
            {
                Debug.LogError($"Failed to load polyhedron '{resourceName}': {status}");
                return;
            }

            // Apply modifier
            ApplyModifier(geom);

            // Normalize to unit sphere
            geom.Unitize();

            // Scale
            if (scale != 1.0f)
            {
                geom.Scale(scale);
            }

            // Orient faces consistently
            geom.Orient();

            // Apply to Unity mesh with flat or smooth shading
            geom.ApplyToMesh(mesh, flatShading);

            Debug.Log($"Generated {polyhedronType} ({modifier}): {geom.VertexCount} vertices, {geom.FaceCount} faces");
        }
    }

    /// <summary>
    /// Apply selected modifier to the geometry
    /// </summary>
    void ApplyModifier(Geometry geom)
    {
        switch (modifier)
        {
            case ModifierType.Dual:
                // Create dual polyhedron (vertices become faces, faces become vertices)
                Status dualStatus = geom.Dual();
                if (dualStatus != Status.OK)
                    Debug.LogWarning($"Dual operation failed: {dualStatus}");
                break;

            case ModifierType.Truncate:
                // Truncate vertices (cut off corners)
                Status truncStatus = geom.Truncate(0.3333);
                if (truncStatus != Status.OK)
                    Debug.LogWarning($"Truncate operation failed: {truncStatus}");
                break;

            case ModifierType.ConvexHull:
                // Calculate convex hull
                Status hullStatus = geom.ConvexHull();
                if (hullStatus != Status.OK)
                    Debug.LogWarning($"ConvexHull operation failed: {hullStatus}");
                break;

            case ModifierType.None:
            default:
                // No modifier
                break;
        }
    }

    /// <summary>
    /// Regenerate the polyhedron (can be called from UI or runtime)
    /// </summary>
    public void Regenerate()
    {
        GeneratePolyhedron();
    }

    /// <summary>
    /// Change the polyhedron type at runtime
    /// </summary>
    public void SetPolyhedronType(PolyhedronType type)
    {
        polyhedronType = type;
        GeneratePolyhedron();
    }

    /// <summary>
    /// Toggle flat shading at runtime
    /// </summary>
    public void SetFlatShading(bool enabled)
    {
        flatShading = enabled;
        GeneratePolyhedron();
    }
}
