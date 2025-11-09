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
    [Header("Polyhedron Generator")]
    [Tooltip("Type of polyhedron to display")]
    public PolyhedronType polyhedronType = PolyhedronType.Icosahedron;

    [Header("Modifiers")]
    [Tooltip("Apply a modifier operation")]
    public ModifierType modifier = ModifierType.None;

    [Header("Modifier Parameters")]
    [Tooltip("Truncate/Bevel ratio (0.0-1.0, typically 0.333)")]
    [Range(0.1f, 0.9f)]
    public float truncateRatio = 0.3333f;

    [Tooltip("Kis face sides (0 = all faces, 3 = triangles only, 4 = quads only, etc.)")]
    [Range(0, 10)]
    public int kisFaceSides = 0;

    [Tooltip("Needle height multiplier (how far spikes extend)")]
    [Range(1.0f, 5.0f)]
    public float needleHeight = 2.0f;

    [Tooltip("Dual reciprocation radius (affects size/shape of dual)")]
    [Range(0.1f, 3.0f)]
    public float dualRadius = 1.0f;

    [Header("Rendering")]
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
    /// Generate the polyhedron mesh using Antiprism
    /// </summary>
    void GeneratePolyhedron()
    {
        if (mesh == null)
            return;

        using (var geom = new Geometry())
        {
            // Load the base polyhedron
            string resourceName = AntiprismPlugin.GetResourceName(polyhedronType);
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
                Debug.Log($"Before Dual: {geom.VertexCount} vertices, {geom.FaceCount} faces");
                Status dualStatus = geom.Dual(dualRadius);
                Debug.Log($"After Dual: {geom.VertexCount} vertices, {geom.FaceCount} faces, Status: {dualStatus}");
                if (dualStatus != Status.OK)
                    Debug.LogWarning($"Dual operation failed: {dualStatus}");
                break;

            case ModifierType.Truncate:
                // Truncate vertices (cut off corners)
                Status truncStatus = geom.Truncate(truncateRatio);
                if (truncStatus != Status.OK)
                    Debug.LogWarning($"Truncate operation failed: {truncStatus}");
                break;

            case ModifierType.Kis:
                // Place pyramid on each face
                Status kisStatus = geom.Kis(kisFaceSides);
                if (kisStatus != Status.OK)
                    Debug.LogWarning($"Kis operation failed: {kisStatus}");
                break;

            case ModifierType.Ambo:
                // Create vertices at edge midpoints (rectify)
                Status amboStatus = geom.Ambo();
                if (amboStatus != Status.OK)
                    Debug.LogWarning($"Ambo operation failed: {amboStatus}");
                break;

            case ModifierType.Gyro:
                // Rotate and subdivide faces
                Status gyroStatus = geom.Gyro();
                if (gyroStatus != Status.OK)
                    Debug.LogWarning($"Gyro operation failed: {gyroStatus}");
                break;

            case ModifierType.Join:
                // Dual of ambo (creates rhombic faces)
                Status joinStatus = geom.Join();
                if (joinStatus != Status.OK)
                    Debug.LogWarning($"Join operation failed: {joinStatus}");
                break;

            case ModifierType.Needle:
                // Elongated kis (creates sharp spikes)
                Status needleStatus = geom.Needle(needleHeight);
                if (needleStatus != Status.OK)
                    Debug.LogWarning($"Needle operation failed: {needleStatus}");
                break;

            case ModifierType.Zip:
                // Dual of kis
                Status zipStatus = geom.Zip();
                if (zipStatus != Status.OK)
                    Debug.LogWarning($"Zip operation failed: {zipStatus}");
                break;

            case ModifierType.Subdivide:
                // Subdivide faces into smaller quads
                Status subdivideStatus = geom.Subdivide();
                if (subdivideStatus != Status.OK)
                    Debug.LogWarning($"Subdivide operation failed: {subdivideStatus}");
                break;

            case ModifierType.Expand:
                // Double ambo (separates faces)
                Status expandStatus = geom.Expand();
                if (expandStatus != Status.OK)
                    Debug.LogWarning($"Expand operation failed: {expandStatus}");
                break;

            case ModifierType.Meta:
                // Kis + dual (complex stellated form)
                Status metaStatus = geom.Meta();
                if (metaStatus != Status.OK)
                    Debug.LogWarning($"Meta operation failed: {metaStatus}");
                break;

            case ModifierType.Bevel:
                // Truncate + ambo (chamfer edges and vertices)
                Status bevelStatus = geom.Bevel(ratio: truncateRatio);
                if (bevelStatus != Status.OK)
                    Debug.LogWarning($"Bevel operation failed: {bevelStatus}");
                break;

            case ModifierType.Snub:
                // Dual + gyro (creates twisted form)
                Status snubStatus = geom.Snub();
                if (snubStatus != Status.OK)
                    Debug.LogWarning($"Snub operation failed: {snubStatus}");
                break;

            case ModifierType.Ortho:
                // Join + join (double join operation)
                Status orthoStatus = geom.Ortho();
                if (orthoStatus != Status.OK)
                    Debug.LogWarning($"Ortho operation failed: {orthoStatus}");
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
