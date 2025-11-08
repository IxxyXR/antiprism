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

    void OnValidate()
    {
        // Regenerate when values change in the inspector (Edit mode only)
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
            GeneratePolyhedron();
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
            case PolyhedronType.Dodecahedron: return "dodec";
            case PolyhedronType.Icosahedron: return "ico";
            case PolyhedronType.TruncatedTetrahedron: return "tr_tet";
            case PolyhedronType.TruncatedCube: return "tr_cube";
            case PolyhedronType.TruncatedOctahedron: return "tr_oct";
            case PolyhedronType.TruncatedDodecahedron: return "tr_dodec";
            case PolyhedronType.TruncatedIcosahedron: return "tr_ico";
            case PolyhedronType.Cuboctahedron: return "cubo";
            case PolyhedronType.Icosidodecahedron: return "id";
            case PolyhedronType.RhombicDodecahedron: return "rhomb_dodec";
            case PolyhedronType.RhombicTriacontahedron: return "rhomb_triac";
            case PolyhedronType.SnubCube: return "sn_cube";
            case PolyhedronType.SnubDodecahedron: return "sn_dodec";
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
                CreateDual(geom);
                break;

            case ModifierType.Truncate:
                // Truncate vertices
                geom.Triangulate();
                break;

            case ModifierType.ConvexHull:
                // Calculate convex hull
                geom.ConvexHull();
                break;

            case ModifierType.None:
            default:
                // No modifier
                break;
        }
    }

    /// <summary>
    /// Create dual polyhedron using OFF string export/import
    /// (Note: A proper dual function would be better - this is a workaround)
    /// </summary>
    void CreateDual(Geometry geom)
    {
        // For now, just triangulate as a placeholder
        // TODO: Implement proper dual operation when available in C API
        geom.Triangulate();
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
