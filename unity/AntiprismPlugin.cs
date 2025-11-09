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
    /// </summary>
    public enum PolyhedronType
    {
        // === PARAMETERIZED TYPES (require n parameter) ===
        Prism,              // N-sided prism (n >= 3)
        Antiprism,          // N-sided antiprism (n >= 3)
        Pyramid,            // N-sided pyramid (n >= 3)
        Dipyramid,          // N-sided dipyramid/bipyramid (n >= 3)
        Cupola,             // N-sided cupola (n >= 2)

        // === PLATONIC SOLIDS ===
        Tetrahedron,
        Cube,
        Octahedron,
        Dodecahedron,
        Icosahedron,

        // === ARCHIMEDEAN SOLIDS ===
        TruncatedTetrahedron,
        TruncatedCube,
        TruncatedOctahedron,
        TruncatedDodecahedron,
        TruncatedIcosahedron,
        Cuboctahedron,
        Icosidodecahedron,
        Rhombicuboctahedron,
        TruncatedCuboctahedron,
        Rhombicosidodecahedron,
        TruncatedIcosidodecahedron,
        SnubCube,
        SnubDodecahedron,

        // === CATALAN SOLIDS (duals of Archimedean) ===
        RhombicDodecahedron,
        RhombicTriacontahedron,

        // === KEPLER-POINSOT POLYHEDRA ===
        SmallStellatedDodecahedron,
        GreatDodecahedron,
        GreatStellatedDodecahedron,
        GreatIcosahedron,

        // === JOHNSON SOLIDS (selected) ===
        // Cupolae and Rotundae
        PentagonalRotunda,

        // Elongated Pyramids
        ElongatedTriangularPyramid,
        ElongatedSquarePyramid,
        ElongatedPentagonalPyramid,

        // Gyroelongated Pyramids
        GyroelongatedSquarePyramid,
        GyroelongatedPentagonalPyramid,

        // Bicupolae
        TriangularOrthobicupola,
        SquareOrthobicupola,
        SquareGyrobicupola,
        PentagonalOrthobicupola,
        PentagonalGyrobicupola,
        PentagonalOrthocupolarotunda,
        PentagonalGyrocupolarotunda,
        PentagonalOrthobirotunda,
        PentagonalGyrobirotunda,

        // Elongated Cupolae
        ElongatedTriangularCupola,
        ElongatedSquareCupola,
        ElongatedPentagonalCupola,

        // Elongated Bicupolae
        ElongatedTriangularOrthobicupola,
        ElongatedSquareGyrobicupola,
        ElongatedPentagonalOrthobicupola,

        // Special Johnson Solids
        Gyrobifastigium,
        SnubSquareAntiprism,
        TriangularHebesphenorotunda,

        // === UNIFORM POLYHEDRA ===
        TruncatedGreatDodecahedron,
        RhombicosidodecahedronVariant,
        GreatIcosidodecahedron,
        TruncatedGreatIcosahedron,
        Tetrahemihexahedron,
        Cubohemioctahedron,
        GreatRhombicuboctahedron,
        SmallRhombihexahedron,
        GreatRhombihexahedron,

        // === UNIFORM COMPOUNDS ===
        StellaOctangula,
        CompoundCubeOctahedron,
        CompoundDodecahedronIcosahedron,
        CompoundTwoTetrahedra,
        CompoundFiveTetrahedra,

        // === WENNINGER STELLATIONS ===
        WenningerW1,
        WenningerW2,
        WenningerW3,
        WenningerW9,
        WenningerW20,
        WenningerW22,

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
                // Platonic Solids
                case PolyhedronType.Tetrahedron: return "tet";
                case PolyhedronType.Cube: return "cube";
                case PolyhedronType.Octahedron: return "oct";
                case PolyhedronType.Dodecahedron: return "dodec";
                case PolyhedronType.Icosahedron: return "ico";

                // Archimedean Solids
                case PolyhedronType.TruncatedTetrahedron: return "tr_tet";
                case PolyhedronType.TruncatedCube: return "tr_cube";
                case PolyhedronType.TruncatedOctahedron: return "tr_oct";
                case PolyhedronType.TruncatedDodecahedron: return "tr_dodec";
                case PolyhedronType.TruncatedIcosahedron: return "tr_ico";
                case PolyhedronType.Cuboctahedron: return "cubo";
                case PolyhedronType.Icosidodecahedron: return "id";
                case PolyhedronType.Rhombicuboctahedron: return "srid";
                case PolyhedronType.TruncatedCuboctahedron: return "tr_cubo";
                case PolyhedronType.Rhombicosidodecahedron: return "sn_cubo";
                case PolyhedronType.TruncatedIcosidodecahedron: return "tr_id";
                case PolyhedronType.SnubCube: return "sn_cube";
                case PolyhedronType.SnubDodecahedron: return "sn_dodec";

                // Catalan Solids
                case PolyhedronType.RhombicDodecahedron: return "rhombic_d12";
                case PolyhedronType.RhombicTriacontahedron: return "rhombic_t30";

                // Kepler-Poinsot Polyhedra
                case PolyhedronType.SmallStellatedDodecahedron: return "st_dodec";
                case PolyhedronType.GreatDodecahedron: return "great_dodec";
                case PolyhedronType.GreatStellatedDodecahedron: return "grt_st_dodec";
                case PolyhedronType.GreatIcosahedron: return "great_ico";

                // Johnson Solids
                case PolyhedronType.PentagonalRotunda: return "J6";
                case PolyhedronType.ElongatedTriangularPyramid: return "J7";
                case PolyhedronType.ElongatedSquarePyramid: return "J8";
                case PolyhedronType.ElongatedPentagonalPyramid: return "J9";
                case PolyhedronType.GyroelongatedSquarePyramid: return "J10";
                case PolyhedronType.GyroelongatedPentagonalPyramid: return "J11";
                case PolyhedronType.TriangularOrthobicupola: return "J27";
                case PolyhedronType.SquareOrthobicupola: return "J28";
                case PolyhedronType.SquareGyrobicupola: return "J29";
                case PolyhedronType.PentagonalOrthobicupola: return "J30";
                case PolyhedronType.PentagonalGyrobicupola: return "J31";
                case PolyhedronType.PentagonalOrthocupolarotunda: return "J32";
                case PolyhedronType.PentagonalGyrocupolarotunda: return "J33";
                case PolyhedronType.PentagonalOrthobirotunda: return "J34";
                case PolyhedronType.PentagonalGyrobirotunda: return "J35";
                case PolyhedronType.ElongatedTriangularCupola: return "J18";
                case PolyhedronType.ElongatedSquareCupola: return "J19";
                case PolyhedronType.ElongatedPentagonalCupola: return "J20";
                case PolyhedronType.ElongatedTriangularOrthobicupola: return "J36";
                case PolyhedronType.ElongatedSquareGyrobicupola: return "J37";
                case PolyhedronType.ElongatedPentagonalOrthobicupola: return "J38";
                case PolyhedronType.Gyrobifastigium: return "J26";
                case PolyhedronType.SnubSquareAntiprism: return "J84";
                case PolyhedronType.TriangularHebesphenorotunda: return "J92";

                // Uniform Polyhedra
                case PolyhedronType.TruncatedGreatDodecahedron: return "U37";
                case PolyhedronType.RhombicosidodecahedronVariant: return "U38";
                case PolyhedronType.GreatIcosidodecahedron: return "U54";
                case PolyhedronType.TruncatedGreatIcosahedron: return "U55";
                case PolyhedronType.Tetrahemihexahedron: return "U4";
                case PolyhedronType.Cubohemioctahedron: return "U15";
                case PolyhedronType.GreatRhombicuboctahedron: return "U17";
                case PolyhedronType.SmallRhombihexahedron: return "U18";
                case PolyhedronType.GreatRhombihexahedron: return "U21";

                // Uniform Compounds
                case PolyhedronType.StellaOctangula: return "UC1";
                case PolyhedronType.CompoundCubeOctahedron: return "UC2";
                case PolyhedronType.CompoundDodecahedronIcosahedron: return "UC3";
                case PolyhedronType.CompoundTwoTetrahedra: return "UC4";
                case PolyhedronType.CompoundFiveTetrahedra: return "UC5";

                // Wenninger Stellations
                case PolyhedronType.WenningerW1: return "W1";
                case PolyhedronType.WenningerW2: return "W2";
                case PolyhedronType.WenningerW3: return "W3";
                case PolyhedronType.WenningerW9: return "W9";
                case PolyhedronType.WenningerW20: return "W20";
                case PolyhedronType.WenningerW22: return "W22";

                // Miscellaneous
                case PolyhedronType.RhombicEnneacontahedron: return "rhombic_e90";
                case PolyhedronType.RhombicHexecontahedron: return "rhombic_h60";
                case PolyhedronType.Csaszar: return "csaszar";
                case PolyhedronType.Szilassi: return "szilassi";
                case PolyhedronType.TetrahedralPrism: return "tet_prism";
                case PolyhedronType.OctahedralPrism: return "oct_prism";

                // Parameterized types - these should use Create methods instead
                case PolyhedronType.Prism:
                case PolyhedronType.Antiprism:
                case PolyhedronType.Pyramid:
                case PolyhedronType.Dipyramid:
                case PolyhedronType.Cupola:
                    throw new ArgumentException($"{type} is parameterized - use Create{type}(n) instead");

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
        /// Create a symmetrohedron using Kaplan-Hart notation
        /// </summary>
        /// <param name="sym">Symmetry type: 'T' (tetrahedral), 'O' (octahedral), 'I' (icosahedral)</param>
        /// <param name="p">First Schläfli parameter</param>
        /// <param name="q">Second Schläfli parameter</param>
        /// <param name="l">Multiplier for first axis (0 = no polygon on this axis)</param>
        /// <param name="m">Multiplier for second axis (0 = no polygon on this axis)</param>
        /// <param name="symId">Symmetry ID number (typically 1)</param>
        /// <returns>New Geometry containing the symmetrohedron</returns>
        public static Geometry CreateSymmetroKaplanHart(char sym, int p, int q, int l, int m, int symId = 1)
        {
            if (sym != 'T' && sym != 'O' && sym != 'I')
                throw new ArgumentException("Symmetry must be 'T', 'O', or 'I'");

            if (p < 2 || q < 2)
                throw new ArgumentException("Schläfli parameters must be >= 2");

            if (l < 0 || m < 0)
                throw new ArgumentException("Multipliers must be >= 0");

            Geometry geom = new Geometry();
            Status status = anti_make_symmetro_kaplan_hart(geom.handle, sym, p, q, l, m, symId);
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
        public Status Truncate(double ratio = 0.3333)
        {
            CheckDisposed();
            return anti_conway_truncate(handle, ratio);
        }

        /// <summary>
        /// Conway Kis operator (place pyramid on each face)
        /// </summary>
        public Status Kis(int faceSides = 0)
        {
            CheckDisposed();
            return anti_conway_kis(handle, faceSides);
        }

        /// <summary>
        /// Conway Ambo operator (rectify - vertices at edge midpoints)
        /// </summary>
        public Status Ambo()
        {
            CheckDisposed();
            return anti_conway_ambo(handle);
        }

        /// <summary>
        /// Conway Gyro operator (rotate and subdivide faces)
        /// </summary>
        public Status Gyro()
        {
            CheckDisposed();
            return anti_conway_gyro(handle);
        }

        /// <summary>
        /// Conway Join operator (dual of ambo)
        /// </summary>
        public Status Join()
        {
            CheckDisposed();
            return anti_conway_join(handle);
        }

        /// <summary>
        /// Conway Needle operator (elongated kis)
        /// </summary>
        public Status Needle(double height = 2.0)
        {
            CheckDisposed();
            return anti_conway_needle(handle, height);
        }

        /// <summary>
        /// Conway Zip operator (dual of kis)
        /// </summary>
        public Status Zip()
        {
            CheckDisposed();
            return anti_conway_zip(handle);
        }

        /// <summary>
        /// Conway Subdivide operator
        /// </summary>
        public Status Subdivide()
        {
            CheckDisposed();
            return anti_conway_subdivide(handle);
        }

        /// <summary>
        /// Conway Expand operator (ambo + ambo)
        /// </summary>
        public Status Expand()
        {
            CheckDisposed();
            return anti_conway_expand(handle);
        }

        /// <summary>
        /// Conway Meta operator (kis + dual)
        /// </summary>
        public Status Meta()
        {
            CheckDisposed();
            return anti_conway_meta(handle);
        }

        /// <summary>
        /// Conway Bevel operator (truncate + ambo)
        /// </summary>
        public Status Bevel(double ratio = 0.3333)
        {
            CheckDisposed();
            return anti_conway_bevel(handle, ratio);
        }

        /// <summary>
        /// Conway Snub operator (dual + gyro)
        /// </summary>
        public Status Snub()
        {
            CheckDisposed();
            return anti_conway_snub(handle);
        }

        /// <summary>
        /// Conway Ortho operator (join + join)
        /// </summary>
        public Status Ortho()
        {
            CheckDisposed();
            return anti_conway_ortho(handle);
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
        private static extern Status anti_conway_truncate(IntPtr geom, double ratio);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_kis(IntPtr geom, int faceSides);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_ambo(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_gyro(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_join(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_needle(IntPtr geom, double height);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_zip(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_subdivide(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_expand(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_meta(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_bevel(IntPtr geom, double ratio);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_snub(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_conway_ortho(IntPtr geom);

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
            IntPtr geom, char sym, int p, int q, int l, int m, int sym_id);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_symmetro_advanced(
            IntPtr geom, char sym, int p, int q, int l, int m,
            int d0, int d1, double rotation, int sym_id);
    }
}
