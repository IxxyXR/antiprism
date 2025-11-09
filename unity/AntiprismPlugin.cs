/*
   Antiprism Unity Plugin - C# API

   C# wrapper for the Antiprism polyhedra library.
   Provides easy-to-use Unity integration for creating and manipulating polyhedra.

   Copyright (c) 2003-2025, Adrian Rossiter
   Antiprism - http://www.antiprism.com
*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Antiprism
{
    /// <summary>
    /// Simplified polyhedron types - parameterized types use separate parameter fields
    /// Covers 200+ polyhedra via ~23 enum entries through parameterization
    /// </summary>
    /// <remarks>
    /// All uniform polyhedra (U1-U80) are accessed via UniformPolyhedron with their U-number.
    /// This includes:
    /// - Platonic solids: U1 (tet), U5 (oct), U6 (cube), U22 (ico), U23 (dod)
    /// - Archimedean solids: U2-U14, U15-U21, U24-U29
    /// - Kepler-Poinsot polyhedra: U34, U35, U52, U53
    /// </remarks>
    public enum PolyhedronType
    {
        // === PARAMETERIZED TYPES (require integer parameter) ===
        Prism,              // N-sided prism (n >= 3)
        Antiprism,          // N-sided antiprism (n >= 3)
        Pyramid,            // N-sided pyramid (n >= 3)
        Dipyramid,          // N-sided dipyramid/bipyramid (n >= 3)
        Cupola,             // N-sided cupola (n >= 2)
        Geodesic,           // Geodesic sphere (requires frequency and method)
        Symmetrohedra,      // Symmetrohedra using Kaplan-Hart notation
        JohnsonSolid,       // Johnson solid by number (J1-J92)
        UniformPolyhedron,  // Uniform polyhedron by number (U1-U80) - includes all Archimedean & Kepler-Poinsot
        Wenninger,          // Wenninger stellation by number (W1-W119)

        // === CATALAN SOLIDS (duals of Archimedean - not in uniform series) ===
        RhombicDodecahedron,
        RhombicTriacontahedron,

        // === UNIFORM COMPOUNDS (famous ones kept for convenience) ===
        StellaOctangula,
        CompoundCubeOctahedron,
        CompoundDodecahedronIcosahedron,
        CompoundTwoTetrahedra,
        CompoundFiveTetrahedra,

        // === MISCELLANEOUS ===
        RhombicEnneacontahedron,
        RhombicHexecontahedron,
        Csaszar,
        Szilassi,
        TetrahedralPrism,
        OctahedralPrism,
    }

    /// <summary>
    /// Geodesic subdivision methods
    /// </summary>
    public enum GeodesicMethod
    {
        Icosahedron,    // Subdivide icosahedron
        Octahedron,     // Subdivide octahedron
        Tetrahedron,    // Subdivide tetrahedron
    }

    /// <summary>
    /// Modifier/Conway operator types
    /// </summary>
    public enum ModifierType
    {
        None,
        Dual,
        Truncate,
        Kis,
        Ambo,
        Gyro,
        Join,
        Needle,
        Zip,
        Subdivide,
        Expand,
        Meta,
        Bevel,
        Snub,
        Ortho,
        ConvexHull
    }

    /// <summary>
    /// Status codes returned by Antiprism operations
    /// </summary>
    public enum Status
    {
        OK = 0,
        ErrorMemory = -1,
        ErrorInvalidHandle = -2,
        ErrorInvalidIndex = -3,
        ErrorParse = -4,
        ErrorFile = -5,
        ErrorUnknown = -99
    }

    /// <summary>
    /// Main Antiprism plugin class
    /// </summary>
    public static class AntiprismPlugin
    {
        public const string LIBRARY_NAME = "antiprism";

        /// <summary>
        /// Get the Antiprism library version
        /// </summary>
        public static string GetVersion()
        {
            IntPtr ptr = anti_get_version();
            return Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Get resource name for non-parameterized polyhedron types
        /// </summary>
        public static string GetResourceName(PolyhedronType type)
        {
            switch (type)
            {
                // Catalan Solids (duals of Archimedean - not in uniform series)
                case PolyhedronType.RhombicDodecahedron: return "rhombic_dodecahedron";
                case PolyhedronType.RhombicTriacontahedron: return "rhombic_triacontahedron";

                // Uniform Compounds
                case PolyhedronType.StellaOctangula: return "UC1";
                case PolyhedronType.CompoundCubeOctahedron: return "UC2";
                case PolyhedronType.CompoundDodecahedronIcosahedron: return "UC3";
                case PolyhedronType.CompoundTwoTetrahedra: return "UC4";
                case PolyhedronType.CompoundFiveTetrahedra: return "UC5";

                // Miscellaneous
                case PolyhedronType.RhombicEnneacontahedron: return "rhombic_e90";
                case PolyhedronType.RhombicHexecontahedron: return "rhombic_h60";
                case PolyhedronType.Csaszar: return "csaszar";
                case PolyhedronType.Szilassi: return "szilassi";
                case PolyhedronType.TetrahedralPrism: return "tet_prism";
                case PolyhedronType.OctahedralPrism: return "oct_prism";

                // Parameterized types - these should use Create methods or be handled in CreateBasePolyhedron
                case PolyhedronType.Prism:
                case PolyhedronType.Antiprism:
                case PolyhedronType.Pyramid:
                case PolyhedronType.Dipyramid:
                case PolyhedronType.Cupola:
                case PolyhedronType.Geodesic:
                case PolyhedronType.Symmetrohedra:
                case PolyhedronType.JohnsonSolid:
                case PolyhedronType.UniformPolyhedron:
                case PolyhedronType.Wenninger:
                    throw new ArgumentException($"{type} is parameterized - should be handled in CreateBasePolyhedron()");

                default:
                    return "ico"; // Fallback
            }
        }

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr anti_get_version();
    }

    /// <summary>
    /// Geometry class - represents a 3D polyhedron
    /// </summary>
    public class Geometry : IDisposable
    {
        internal IntPtr handle;
        private bool disposed = false;

        /// <summary>
        /// Create a new empty geometry
        /// </summary>
        public Geometry()
        {
            handle = anti_geometry_create();
            if (handle == IntPtr.Zero)
                throw new Exception("Failed to create geometry");
        }

        ~Geometry()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (handle != IntPtr.Zero)
                {
                    anti_geometry_destroy(handle);
                    handle = IntPtr.Zero;
                }
                disposed = true;
            }
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException("Geometry");
        }

        // === FACTORY METHODS FOR PARAMETERIZED POLYHEDRA ===

        /// <summary>
        /// Create an N-sided prism
        /// </summary>
        /// <param name="n">Number of sides (must be >= 3)</param>
        public static Geometry CreatePrism(int n)
        {
            if (n < 3)
                throw new ArgumentException("Prism must have at least 3 sides");

            Geometry geom = new Geometry();
            Status status = anti_make_prism(geom.handle, n);
            if (status != Status.OK)
                throw new Exception($"Failed to create prism: {status}");

            return geom;
        }

        /// <summary>
        /// Create an N-sided antiprism
        /// </summary>
        /// <param name="n">Number of sides (must be >= 3)</param>
        public static Geometry CreateAntiprism(int n)
        {
            if (n < 3)
                throw new ArgumentException("Antiprism must have at least 3 sides");

            Geometry geom = new Geometry();
            Status status = anti_make_antiprism(geom.handle, n);
            if (status != Status.OK)
                throw new Exception($"Failed to create antiprism: {status}");

            return geom;
        }

        /// <summary>
        /// Create an N-sided pyramid
        /// </summary>
        /// <param name="n">Number of sides (must be >= 3)</param>
        public static Geometry CreatePyramid(int n)
        {
            if (n < 3)
                throw new ArgumentException("Pyramid must have at least 3 sides");

            Geometry geom = new Geometry();
            Status status = anti_make_pyramid(geom.handle, n);
            if (status != Status.OK)
                throw new Exception($"Failed to create pyramid: {status}");

            return geom;
        }

        /// <summary>
        /// Create an N-sided dipyramid (bipyramid)
        /// </summary>
        /// <param name="n">Number of sides (must be >= 3)</param>
        public static Geometry CreateDipyramid(int n)
        {
            if (n < 3)
                throw new ArgumentException("Dipyramid must have at least 3 sides");

            Geometry geom = new Geometry();
            Status status = anti_make_dipyramid(geom.handle, n);
            if (status != Status.OK)
                throw new Exception($"Failed to create dipyramid: {status}");

            return geom;
        }

        /// <summary>
        /// Create an N-sided cupola
        /// </summary>
        /// <param name="n">Number of sides (must be >= 2)</param>
        public static Geometry CreateCupola(int n)
        {
            if (n < 2)
                throw new ArgumentException("Cupola must have at least 2 sides");

            Geometry geom = new Geometry();
            Status status = anti_make_cupola(geom.handle, n);
            if (status != Status.OK)
                throw new Exception($"Failed to create cupola: {status}");

            return geom;
        }

        /// <summary>
        /// Create a geodesic sphere by subdividing a base polyhedron
        /// </summary>
        /// <param name="frequency">Subdivision frequency (1-10 recommended)</param>
        /// <param name="method">Base polyhedron to subdivide</param>
        public static Geometry CreateGeodesic(int frequency, GeodesicMethod method = GeodesicMethod.Icosahedron)
        {
            if (frequency < 1)
                throw new ArgumentException("Frequency must be >= 1");

            Geometry geom = new Geometry();
            Status status = anti_make_geodesic(geom.handle, frequency, (int)method);
            if (status != Status.OK)
                throw new Exception($"Failed to create geodesic sphere: {status}");

            return geom;
        }

        /// <summary>
        /// Create a symmetrohedron using Kaplan-Hart notation (matches CLI: -k sym,mult0,mult1,mult2)
        /// </summary>
        /// <param name="sym">Symmetry type: 'T' (tetrahedral), 'O' (octahedral), 'I' (icosahedral)</param>
        /// <param name="mult0">Multiplier for primary axis (0 = skip)</param>
        /// <param name="mult1">Multiplier for secondary axis (0 = skip)</param>
        /// <param name="mult2">Multiplier for tertiary axis (0 = skip)</param>
        /// <returns>New Geometry containing the symmetrohedron</returns>
        /// <remarks>
        /// Axis orders: T=[3,3,2], O=[4,3,2], I=[5,3,2]
        /// Examples:
        ///   - Cuboctahedron: sym='O', mult0=1, mult1=1, mult2=0
        ///   - Snub Cube: sym='O', mult0=1, mult1=0, mult2=1
        /// At least one and at most two multipliers must be non-zero.
        /// </remarks>
        public static Geometry CreateSymmetroKaplanHart(char sym, int mult0, int mult1, int mult2)
        {
            if (sym != 'T' && sym != 'O' && sym != 'I')
                throw new ArgumentException("Symmetry must be 'T', 'O', or 'I'");

            if (mult0 < 0 || mult1 < 0 || mult2 < 0)
                throw new ArgumentException("Multipliers must be >= 0");

            int numMultipliers = (mult0 > 0 ? 1 : 0) + (mult1 > 0 ? 1 : 0) + (mult2 > 0 ? 1 : 0);
            if (numMultipliers == 0 || numMultipliers == 3)
                throw new ArgumentException("At least one and at most two multipliers must be non-zero");

            Geometry geom = new Geometry();
            Status status = anti_make_symmetro_kaplan_hart(geom.handle, sym, mult0, mult1, mult2);
            if (status != Status.OK)
                throw new Exception($"Failed to create symmetrohedron: {status}");

            return geom;
        }

        /// <summary>
        /// Create a symmetrohedron with advanced parameters
        /// </summary>
        /// <param name="sym">Symmetry type: 'T', 'O', 'I', 'D', 'S', 'C', 'V', 'H'</param>
        /// <param name="p">First Schläfli parameter</param>
        /// <param name="q">Second Schläfli parameter</param>
        /// <param name="l">Multiplier for first axis</param>
        /// <param name="m">Multiplier for second axis</param>
        /// <param name="d0">D value for first axis (default 1)</param>
        /// <param name="d1">D value for second axis (default 1)</param>
        /// <param name="rotation">Rotation angle in degrees (default 0)</param>
        /// <param name="symId">Symmetry ID number (typically 1)</param>
        /// <returns>New Geometry containing the symmetrohedron</returns>
        public static Geometry CreateSymmetroAdvanced(char sym, int p, int q, int l, int m,
            int d0 = 1, int d1 = 1, double rotation = 0.0, int symId = 1)
        {
            if (sym != 'T' && sym != 'O' && sym != 'I' && sym != 'D' &&
                sym != 'S' && sym != 'C' && sym != 'V' && sym != 'H')
                throw new ArgumentException("Invalid symmetry type");

            if (p < 2 || q < 2)
                throw new ArgumentException("Schläfli parameters must be >= 2");

            if (l < 0 || m < 0)
                throw new ArgumentException("Multipliers must be >= 0");

            Geometry geom = new Geometry();
            Status status = anti_make_symmetro_advanced(geom.handle, sym, p, q, l, m, d0, d1, rotation, symId);
            if (status != Status.OK)
                throw new Exception($"Failed to create symmetrohedron: {status}");

            return geom;
        }

        /// <summary>
        /// Create a zonohedron from a star of vectors
        /// </summary>
        /// <param name="starVectors">Star of vectors defining the zones</param>
        /// <returns>New Geometry containing the zonohedron</returns>
        public static Geometry CreateZonohedron(Vector3[] starVectors)
        {
            if (starVectors == null || starVectors.Length < 1)
                throw new ArgumentException("Star must contain at least one vector");

            // Flatten Vector3[] to double[]
            double[] flatVectors = new double[starVectors.Length * 3];
            for (int i = 0; i < starVectors.Length; i++)
            {
                flatVectors[i * 3 + 0] = starVectors[i].x;
                flatVectors[i * 3 + 1] = starVectors[i].y;
                flatVectors[i * 3 + 2] = starVectors[i].z;
            }

            Geometry geom = new Geometry();
            Status status = anti_make_zonohedron(geom.handle, flatVectors, starVectors.Length);
            if (status != Status.OK)
                throw new Exception($"Failed to create zonohedron: {status}");

            return geom;
        }

        /// <summary>
        /// Create a polar zonohedron from an ordered star of vectors
        /// </summary>
        /// <param name="starVectors">Ordered star of vectors</param>
        /// <param name="step">Step this many places to get to next vector (default: 1)</param>
        /// <param name="spiralStep">Step between ridges of spirallohedron, 0 for regular (default: 0)</param>
        /// <returns>New Geometry containing the polar zonohedron</returns>
        public static Geometry CreatePolarZonohedron(Vector3[] starVectors, int step = 1, int spiralStep = 0)
        {
            if (starVectors == null || starVectors.Length < 1)
                throw new ArgumentException("Star must contain at least one vector");

            if (step < 1)
                throw new ArgumentException("Step must be >= 1");

            // Flatten Vector3[] to double[]
            double[] flatVectors = new double[starVectors.Length * 3];
            for (int i = 0; i < starVectors.Length; i++)
            {
                flatVectors[i * 3 + 0] = starVectors[i].x;
                flatVectors[i * 3 + 1] = starVectors[i].y;
                flatVectors[i * 3 + 2] = starVectors[i].z;
            }

            Geometry geom = new Geometry();
            Status status = anti_make_polar_zonohedron(geom.handle, flatVectors, starVectors.Length, step, spiralStep);
            if (status != Status.OK)
                throw new Exception($"Failed to create polar zonohedron: {status}");

            return geom;
        }

        /// <summary>
        /// Create a zonohedron from the vertices of a seed polyhedron
        /// </summary>
        /// <param name="seed">Seed polyhedron whose vertices define the star</param>
        /// <returns>New Geometry containing the zonohedron</returns>
        /// <remarks>
        /// Examples:
        /// - Cube vertices → Rhombic Dodecahedron
        /// - Dodecahedron vertices → Rhombic Triacontahedron
        /// - Icosahedron vertices → Rhombic Hexecontahedron
        /// </remarks>
        public static Geometry CreateZonohedronFromVertices(Geometry seed)
        {
            if (seed == null)
                throw new ArgumentNullException(nameof(seed));

            // Use vertices as star
            Vector3[] star = seed.GetVertices();
            return CreateZonohedron(star);
        }

        /// <summary>
        /// Load a built-in polyhedron (e.g., "cube", "tet", "ico", "dodec")
        /// </summary>
        public Status LoadResource(string name)
        {
            CheckDisposed();
            return anti_geometry_read_resource(handle, name);
        }

        /// <summary>
        /// Get the number of vertices
        /// </summary>
        public int VertexCount
        {
            get
            {
                CheckDisposed();
                return anti_geometry_num_verts(handle);
            }
        }

        /// <summary>
        /// Get the number of faces
        /// </summary>
        public int FaceCount
        {
            get
            {
                CheckDisposed();
                return anti_geometry_num_faces(handle);
            }
        }

        /// <summary>
        /// Get all vertices as a Vector3 array
        /// </summary>
        public Vector3[] GetVertices()
        {
            CheckDisposed();
            int count = VertexCount;
            Vector3[] vertices = new Vector3[count];

            for (int i = 0; i < count; i++)
            {
                double x, y, z;
                if (anti_geometry_get_vert(handle, i, out x, out y, out z) == Status.OK)
                {
                    vertices[i] = new Vector3((float)x, (float)y, (float)z);
                }
            }

            return vertices;
        }

        /// <summary>
        /// Get all face indices as arrays
        /// </summary>
        public int[][] GetFaces()
        {
            CheckDisposed();
            int faceCount = FaceCount;
            int[][] faces = new int[faceCount][];

            for (int i = 0; i < faceCount; i++)
            {
                int faceSize = anti_geometry_face_num_verts(handle, i);
                faces[i] = new int[faceSize];
                anti_geometry_get_face(handle, i, faces[i], faceSize);
            }

            return faces;
        }

        /// <summary>
        /// Get polyhedron data (vertices and faces) for use with mesh libraries
        /// This is more efficient than GetVertices() + GetFaces() separately
        /// </summary>
        /// <param name="vertices">Output array of vertex positions</param>
        /// <param name="faceIndices">Output array of face index arrays</param>
        public void GetPolyhedronData(out Vector3[] vertices, out int[][] faceIndices)
        {
            Color[] dummyColors;
            GetPolyhedronData(out vertices, out faceIndices, out dummyColors);
        }

        /// <summary>
        /// Get polyhedron data including face colors for use with mesh libraries
        /// This is more efficient than GetVertices() + GetFaces() + individual color queries
        /// </summary>
        /// <param name="vertices">Output array of vertex positions</param>
        /// <param name="faceIndices">Output array of face index arrays</param>
        /// <param name="faceColors">Output array of face colors (null entries for uncolored faces)</param>
        public void GetPolyhedronData(out Vector3[] vertices, out int[][] faceIndices, out Color[] faceColors)
        {
            CheckDisposed();

            // Get vertices
            vertices = GetVertices();

            // Get faces in original form (not triangulated)
            int faceCount = FaceCount;
            if (faceCount == 0)
            {
                faceIndices = new int[0][];
                faceColors = new Color[0];
                return;
            }

            // Allocate buffer for face data (estimate size)
            int bufferSize = faceCount * 10; // Most faces won't exceed 10 vertices
            int[] buffer = new int[bufferSize];
            int totalSize = anti_geometry_get_all_faces(handle, buffer, bufferSize);

            if (totalSize < 0)
            {
                faceIndices = new int[0][];
                faceColors = new Color[0];
                return;
            }

            // Parse the buffer into face arrays
            System.Collections.Generic.List<int[]> faces = new System.Collections.Generic.List<int[]>();
            int offset = 0;
            while (offset < totalSize && faces.Count < faceCount)
            {
                int faceSize = buffer[offset];
                offset++;

                int[] face = new int[faceSize];
                for (int i = 0; i < faceSize; i++)
                {
                    face[i] = buffer[offset];
                    offset++;
                }
                faces.Add(face);
            }

            faceIndices = faces.ToArray();

            // Get face colors
            faceColors = new Color[faceCount];
            for (int i = 0; i < faceCount; i++)
            {
                int r, g, b, a;
                Status status = anti_geometry_get_face_color(handle, i, out r, out g, out b, out a);
                if (status == Status.OK)
                {
                    // Convert from 0-255 to 0-1 range
                    faceColors[i] = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                }
                else
                {
                    // No color set for this face - use white/clear
                    faceColors[i] = new Color(1, 1, 1, 0);
                }
            }
        }

        /// <summary>
        /// Apply geometry to a Unity Mesh with optional flat shading
        /// </summary>
        public void ApplyToMesh(Mesh mesh, bool flatShading = true)
        {
            CheckDisposed();

            if (mesh == null)
                throw new ArgumentNullException("mesh");

            mesh.Clear();

            Vector3[] vertices = GetVertices();
            int[][] faces = GetFaces();

            if (flatShading)
            {
                // Flat shading: duplicate vertices for each triangle so normals are per-face
                System.Collections.Generic.List<Vector3> vertList = new System.Collections.Generic.List<Vector3>();
                System.Collections.Generic.List<int> triList = new System.Collections.Generic.List<int>();

                foreach (int[] face in faces)
                {
                    // Triangulate face using fan triangulation
                    for (int i = 1; i < face.Length - 1; i++)
                    {
                        int baseIdx = vertList.Count;
                        vertList.Add(vertices[face[0]]);
                        vertList.Add(vertices[face[i]]);
                        vertList.Add(vertices[face[i + 1]]);

                        triList.Add(baseIdx);
                        triList.Add(baseIdx + 1);
                        triList.Add(baseIdx + 2);
                    }
                }

                mesh.vertices = vertList.ToArray();
                mesh.triangles = triList.ToArray();
                mesh.RecalculateNormals();
            }
            else
            {
                // Smooth shading: share vertices
                mesh.vertices = vertices;

                System.Collections.Generic.List<int> triList = new System.Collections.Generic.List<int>();
                foreach (int[] face in faces)
                {
                    // Triangulate face using fan triangulation
                    for (int i = 1; i < face.Length - 1; i++)
                    {
                        triList.Add(face[0]);
                        triList.Add(face[i]);
                        triList.Add(face[i + 1]);
                    }
                }

                mesh.triangles = triList.ToArray();
                mesh.RecalculateNormals();
            }

            mesh.RecalculateBounds();
        }

        // === GEOMETRY OPERATIONS ===

        /// <summary>
        /// Create the dual polyhedron
        /// </summary>
        public Status Dual(double radius = 1.0)
        {
            CheckDisposed();
            return anti_geometry_dual(handle, radius);
        }

        /// <summary>
        /// Unitize: scale to unit sphere
        /// </summary>
        public Status Unitize()
        {
            CheckDisposed();
            return anti_geometry_unitize(handle);
        }

        /// <summary>
        /// Scale by a factor
        /// </summary>
        public Status Scale(double factor)
        {
            CheckDisposed();
            return anti_geometry_scale(handle, factor);
        }

        /// <summary>
        /// Orient faces consistently
        /// </summary>
        public Status Orient()
        {
            CheckDisposed();
            return anti_geometry_orient(handle);
        }

        /// <summary>
        /// Apply convex hull
        /// </summary>
        public Status ConvexHull()
        {
            CheckDisposed();
            return anti_geometry_convex_hull(handle);
        }

        // === CONWAY OPERATORS ===

        /// <summary>
        /// Conway Truncate operator
        /// </summary>
        /// <param name="ratio">Truncation ratio (0.0-1.0, typically 0.333)</param>
        /// <param name="order">Truncate only vertices with this order (0 for all)</param>
        public Status Truncate(double ratio = 0.3333, int order = 0)
        {
            CheckDisposed();
            return anti_geometry_truncate(handle, ratio, order);
        }

        /// <summary>
        /// Conway Kis operator (place pyramid on each face)
        /// </summary>
        /// <param name="faceSides">Only kis faces with n sides (0 for all faces, 3 for triangles, 4 for quads, etc.)</param>
        public Status Kis(int faceSides = 0)
        {
            CheckDisposed();
            return anti_geometry_kis(handle, faceSides);
        }

        /// <summary>
        /// Conway Ambo operator (rectify - vertices at edge midpoints)
        /// </summary>
        public Status Ambo()
        {
            CheckDisposed();
            return anti_geometry_ambo(handle);
        }

        /// <summary>
        /// Conway Gyro operator (rotate and subdivide faces)
        /// </summary>
        /// <param name="n">Gyro subscript parameter (default 1)</param>
        public Status Gyro(int n = 1)
        {
            CheckDisposed();
            return anti_geometry_gyro(handle, n);
        }

        /// <summary>
        /// Conway Join operator (dual of ambo)
        /// </summary>
        public Status Join()
        {
            CheckDisposed();
            return anti_geometry_join(handle);
        }

        /// <summary>
        /// Conway Needle operator (elongated kis)
        /// </summary>
        /// <param name="height">Height multiplier for needle points (default 2.0)</param>
        public Status Needle(double height = 2.0)
        {
            CheckDisposed();
            return anti_geometry_needle(handle, height);
        }

        /// <summary>
        /// Conway Zip operator (dual of kis)
        /// </summary>
        public Status Zip()
        {
            CheckDisposed();
            return anti_geometry_zip(handle);
        }

        /// <summary>
        /// Conway Subdivide operator
        /// </summary>
        /// <param name="n">First subscript parameter (default 2)</param>
        /// <param name="m">Second subscript parameter (default 0)</param>
        public Status Subdivide(int n = 2, int m = 0)
        {
            CheckDisposed();
            return anti_geometry_subdivide(handle, n, m);
        }

        /// <summary>
        /// Conway Expand operator (ambo + ambo)
        /// </summary>
        /// <param name="n">First subscript parameter (default 2)</param>
        /// <param name="m">Second subscript parameter (default 0)</param>
        public Status Expand(int n = 2, int m = 0)
        {
            CheckDisposed();
            return anti_geometry_expand(handle, n, m);
        }

        /// <summary>
        /// Conway Meta operator (kis + dual)
        /// </summary>
        /// <param name="n">Meta subscript parameter (default 2)</param>
        public Status Meta(int n = 2)
        {
            CheckDisposed();
            return anti_geometry_meta(handle, n);
        }

        /// <summary>
        /// Conway Bevel operator (truncate + ambo)
        /// </summary>
        /// <param name="n">Bevel subscript parameter (default 2)</param>
        /// <param name="ratio">Truncation ratio (0.0-1.0, typically 0.333)</param>
        public Status Bevel(int n = 2, double ratio = 0.3333)
        {
            CheckDisposed();
            return anti_geometry_bevel(handle, n, ratio);
        }

        /// <summary>
        /// Conway Snub operator (dual + gyro)
        /// </summary>
        /// <param name="n">Snub subscript parameter (default 2)</param>
        public Status Snub(int n = 2)
        {
            CheckDisposed();
            return anti_geometry_snub(handle, n);
        }

        /// <summary>
        /// Conway Ortho operator (join + join)
        /// </summary>
        /// <param name="n">First subscript parameter (default 2)</param>
        /// <param name="m">Second subscript parameter (default 0)</param>
        public Status Ortho(int n = 2, int m = 0)
        {
            CheckDisposed();
            return anti_geometry_ortho(handle, n, m);
        }

        // === P/INVOKE DECLARATIONS ===

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern IntPtr anti_geometry_create();

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern void anti_geometry_destroy(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_read_resource(IntPtr geom, string name);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_num_verts(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_num_faces(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_get_vert(IntPtr geom, int idx,
            out double x, out double y, out double z);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_face_num_verts(IntPtr geom, int faceIdx);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_get_face(IntPtr geom, int faceIdx, int[] indices, int maxIndices);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_get_all_faces(IntPtr geom, int[] buffer, int bufferSize);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_get_face_color(IntPtr geom, int faceIdx,
            out int r, out int g, out int b, out int a);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_unitize(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_scale(IntPtr geom, double factor);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_orient(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_dual(IntPtr geom, double radius);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_convex_hull(IntPtr geom);

        // Conway operators
        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_truncate(IntPtr geom, double ratio, int order);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_kis(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_ambo(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_gyro(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_join(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_needle(IntPtr geom, double height);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_zip(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_subdivide(IntPtr geom, int n, int m);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_expand(IntPtr geom, int n, int m);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_meta(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_bevel(IntPtr geom, int n, double ratio);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_snub(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_ortho(IntPtr geom, int n, int m);

        // Parameterized polyhedra generators
        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_prism(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_antiprism(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_pyramid(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_dipyramid(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_cupola(IntPtr geom, int n);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_geodesic(IntPtr geom, int frequency, int method);

        // Symmetrohedra generators
        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_symmetro_kaplan_hart(
            IntPtr geom, char sym, int mult0, int mult1, int mult2);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_symmetro_advanced(
            IntPtr geom, char sym, int p, int q, int l, int m,
            int d0, int d1, double rotation, int sym_id);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_zonohedron(
            IntPtr geom, double[] star_vectors, int num_vectors);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_polar_zonohedron(
            IntPtr geom, double[] star_vectors, int num_vectors, int step, int spiral_step);
    }
}
