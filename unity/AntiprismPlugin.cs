/*
   Antiprism Unity Plugin Wrapper
   Copyright (c) 2025, Antiprism

   This file provides a C# wrapper for the Antiprism C API
   for use in Unity projects.

   Usage:
   1. Build the Antiprism library as a shared library (.dll/.so/.dylib)
   2. Place the library in your Unity project's Plugins folder
   3. Use this class to interact with the Antiprism library
*/

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Antiprism
{
    /// <summary>
    /// Status codes returned by Antiprism API functions
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
    /// Available polyhedron types that can be generated
    /// </summary>
    public enum PolyhedronType
    {
        // Platonic Solids
        Tetrahedron,
        Cube,
        Octahedron,
        Dodecahedron,
        Icosahedron,

        // Archimedean Solids (truncated forms)
        TruncatedTetrahedron,
        TruncatedCube,
        TruncatedOctahedron,
        TruncatedDodecahedron,
        TruncatedIcosahedron,

        // Archimedean Solids (other)
        Cuboctahedron,
        Icosidodecahedron,
        Rhombicuboctahedron,
        TruncatedCuboctahedron,
        Rhombicosidodecahedron,
        TruncatedIcosidodecahedron,
        SnubCube,
        SnubDodecahedron,

        // Catalan Solids (duals of Archimedean)
        RhombicDodecahedron,
        RhombicTriacontahedron,

        // Prisms
        Prism3,
        Prism4,
        Prism5,
        Prism6,
        Prism7,
        Prism8,
        Prism9,
        Prism10,
        Prism12,

        // Antiprisms
        Antiprism3,
        Antiprism4,
        Antiprism5,
        Antiprism6,
        Antiprism8,
        Antiprism10,

        // Pyramids
        TriangularPyramid,
        SquarePyramid,
        PentagonalPyramid,
        HexagonalPyramid,

        // Dipyramids (Bipyramids)
        TriangularDipyramid,
        SquareDipyramid,
        PentagonalDipyramid,
        HexagonalDipyramid,
        OctagonalDipyramid,
        DecagonalDipyramid,

        // Kepler-Poinsot Polyhedra (stellated regular)
        SmallStellatedDodecahedron,
        GreatDodecahedron,
        GreatStellatedDodecahedron,
        GreatIcosahedron,

        // Geodesic Spheres (icosahedral subdivision)
        GeodesicIco2,
        GeodesicIco3,
        GeodesicIco4,
        GeodesicIco5,
        GeodesicIco6,
        GeodesicIco8,

        // Johnson Solids - Cupolae and Rotundae (J3-J6)
        TriangularCupola,
        SquareCupola,
        PentagonalCupola,
        PentagonalRotunda,

        // Johnson Solids - Elongated Pyramids (J7-J9)
        ElongatedTriangularPyramid,
        ElongatedSquarePyramid,
        ElongatedPentagonalPyramid,

        // Johnson Solids - Gyroelongated Pyramids (J10-J12)
        GyroelongatedSquarePyramid,
        GyroelongatedPentagonalPyramid,

        // Johnson Solids - Bicupolae (J27-J35)
        TriangularOrthobicupola,
        SquareOrthobicupola,
        SquareGyrobicupola,
        PentagonalOrthobicupola,
        PentagonalGyrobicupola,
        PentagonalOrthocupolarotunda,
        PentagonalGyrocupolarotunda,
        PentagonalOrthobirotunda,
        PentagonalGyrobirotunda,

        // Johnson Solids - Elongated Cupolae (J18-J20)
        ElongatedTriangularCupola,
        ElongatedSquareCupola,
        ElongatedPentagonalCupola,

        // Johnson Solids - Elongated Bicupolae (J36-J38)
        ElongatedTriangularOrthobicupola,
        ElongatedSquareGyrobicupola,
        ElongatedPentagonalOrthobicupola,

        // Johnson Solids - Special (J26, J84, J92)
        Gyrobifastigium,
        SnubSquareAntiprism,
        TriangularHebesphenorotunda,

        // Uniform Polyhedra - Stellated Forms (U34-U41)
        SmallStellatedDodecahedron,
        GreatDodecahedron,
        GreatStellatedDodecahedron,
        GreatIcosahedron,
        TruncatedGreatDodecahedron,
        RhombicosidodecahedronVariant,
        GreatIcosidodecahedron,
        TruncatedGreatIcosahedron,

        // Uniform Polyhedra - Special Forms
        Tetrahemihexahedron,
        Cubohemioctahedron,
        GreatRhombicuboctahedron,
        SmallRhombihexahedron,
        GreatRhombihexahedron,

        // Uniform Compounds (UC1-UC5 - most famous)
        StellaOctangula,
        CompoundCubeOctahedron,
        CompoundDodecahedronIcosahedron,
        CompoundTwoTetrahedra,
        CompoundFiveTetrahedra,

        // Wenninger Stellations (famous ones)
        WenningerW1,
        WenningerW2,
        WenningerW3,
        WenningerW9,
        WenningerW20,
        WenningerW22,

        // Miscellaneous Special Polyhedra
        RhombicEnneacontahedron,
        RhombicHexecontahedron,
        Csaszar,
        Szilassi
    }

    /// <summary>
    /// Conway polyhedron notation operators for modifying polyhedra
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
    /// Unity wrapper for the Antiprism polyhedra library
    /// </summary>
    public class AntiprismPlugin
    {
        // Platform-specific library names
        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        internal const string LIBRARY_NAME = "antiprism";
        #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        internal const string LIBRARY_NAME = "libantiprism";
        #elif UNITY_ANDROID
        internal const string LIBRARY_NAME = "antiprism";
        #elif UNITY_IOS
        internal const string LIBRARY_NAME = "__Internal";
        #else
        internal const string LIBRARY_NAME = "antiprism";
        #endif

        /*-----------------------------------------------------------------------
         * Public Utility Methods
         *-----------------------------------------------------------------------*/

        /// <summary>
        /// Get the Antiprism resource name for a given polyhedron type
        /// </summary>
        /// <param name="type">The polyhedron type</param>
        /// <returns>Resource name string for use with anti_geometry_read_resource()</returns>
        public static string GetResourceName(PolyhedronType type)
        {
            switch (type)
            {
                // Platonic Solids
                case PolyhedronType.Tetrahedron: return "tet";
                case PolyhedronType.Cube: return "cube";
                case PolyhedronType.Octahedron: return "oct";
                case PolyhedronType.Dodecahedron: return "dodecahedron";
                case PolyhedronType.Icosahedron: return "ico";

                // Archimedean Solids (truncated forms)
                case PolyhedronType.TruncatedTetrahedron: return "tr_tet";
                case PolyhedronType.TruncatedCube: return "tr_cube";
                case PolyhedronType.TruncatedOctahedron: return "tr_oct";
                case PolyhedronType.TruncatedDodecahedron: return "tr_dod";
                case PolyhedronType.TruncatedIcosahedron: return "tr_ico";

                // Archimedean Solids (other)
                case PolyhedronType.Cuboctahedron: return "cubo";
                case PolyhedronType.Icosidodecahedron: return "id";
                case PolyhedronType.Rhombicuboctahedron: return "rhombicubo";
                case PolyhedronType.TruncatedCuboctahedron: return "tr_cubo";
                case PolyhedronType.Rhombicosidodecahedron: return "rhombicosid";
                case PolyhedronType.TruncatedIcosidodecahedron: return "tr_icosid";
                case PolyhedronType.SnubCube: return "sn_cube";
                case PolyhedronType.SnubDodecahedron: return "sn_dod";

                // Catalan Solids (duals of Archimedean)
                case PolyhedronType.RhombicDodecahedron: return "rhombic_dodecahedron";
                case PolyhedronType.RhombicTriacontahedron: return "rhombic_triacontahedron";

                // Prisms
                case PolyhedronType.Prism3: return "pri3";
                case PolyhedronType.Prism4: return "pri4";
                case PolyhedronType.Prism5: return "pri5";
                case PolyhedronType.Prism6: return "pri6";
                case PolyhedronType.Prism7: return "pri7";
                case PolyhedronType.Prism8: return "pri8";
                case PolyhedronType.Prism9: return "pri9";
                case PolyhedronType.Prism10: return "pri10";
                case PolyhedronType.Prism12: return "pri12";

                // Antiprisms
                case PolyhedronType.Antiprism3: return "ant3";
                case PolyhedronType.Antiprism4: return "ant4";
                case PolyhedronType.Antiprism5: return "ant5";
                case PolyhedronType.Antiprism6: return "ant6";
                case PolyhedronType.Antiprism8: return "ant8";
                case PolyhedronType.Antiprism10: return "ant10";

                // Pyramids
                case PolyhedronType.TriangularPyramid: return "pyr3";
                case PolyhedronType.SquarePyramid: return "pyr4";
                case PolyhedronType.PentagonalPyramid: return "pyr5";
                case PolyhedronType.HexagonalPyramid: return "pyr6";

                // Dipyramids (Bipyramids)
                case PolyhedronType.TriangularDipyramid: return "dip3";
                case PolyhedronType.SquareDipyramid: return "dip4";
                case PolyhedronType.PentagonalDipyramid: return "dip5";
                case PolyhedronType.HexagonalDipyramid: return "dip6";
                case PolyhedronType.OctagonalDipyramid: return "dip8";
                case PolyhedronType.DecagonalDipyramid: return "dip10";

                // Kepler-Poinsot Polyhedra (stellated regular)
                case PolyhedronType.SmallStellatedDodecahedron: return "u34";
                case PolyhedronType.GreatDodecahedron: return "u35";
                case PolyhedronType.GreatStellatedDodecahedron: return "u52";
                case PolyhedronType.GreatIcosahedron: return "u53";

                // Geodesic Spheres (icosahedral subdivision)
                case PolyhedronType.GeodesicIco2: return "geo_i_2";
                case PolyhedronType.GeodesicIco3: return "geo_i_3";
                case PolyhedronType.GeodesicIco4: return "geo_i_4";
                case PolyhedronType.GeodesicIco5: return "geo_i_5";
                case PolyhedronType.GeodesicIco6: return "geo_i_6";
                case PolyhedronType.GeodesicIco8: return "geo_i_8";

                // Johnson Solids - Cupolae and Rotundae (J3-J6)
                case PolyhedronType.TriangularCupola: return "j3";
                case PolyhedronType.SquareCupola: return "j4";
                case PolyhedronType.PentagonalCupola: return "j5";
                case PolyhedronType.PentagonalRotunda: return "j6";

                // Johnson Solids - Elongated Pyramids (J7-J9)
                case PolyhedronType.ElongatedTriangularPyramid: return "j7";
                case PolyhedronType.ElongatedSquarePyramid: return "j8";
                case PolyhedronType.ElongatedPentagonalPyramid: return "j9";

                // Johnson Solids - Gyroelongated Pyramids (J10-J12)
                case PolyhedronType.GyroelongatedSquarePyramid: return "j10";
                case PolyhedronType.GyroelongatedPentagonalPyramid: return "j11";

                // Johnson Solids - Bicupolae (J27-J35)
                case PolyhedronType.TriangularOrthobicupola: return "j27";
                case PolyhedronType.SquareOrthobicupola: return "j28";
                case PolyhedronType.SquareGyrobicupola: return "j29";
                case PolyhedronType.PentagonalOrthobicupola: return "j30";
                case PolyhedronType.PentagonalGyrobicupola: return "j31";
                case PolyhedronType.PentagonalOrthocupolarotunda: return "j32";
                case PolyhedronType.PentagonalGyrocupolarotunda: return "j33";
                case PolyhedronType.PentagonalOrthobirotunda: return "j34";
                case PolyhedronType.PentagonalGyrobirotunda: return "j35";

                // Johnson Solids - Elongated Cupolae (J18-J20)
                case PolyhedronType.ElongatedTriangularCupola: return "j18";
                case PolyhedronType.ElongatedSquareCupola: return "j19";
                case PolyhedronType.ElongatedPentagonalCupola: return "j20";

                // Johnson Solids - Elongated Bicupolae (J36-J38)
                case PolyhedronType.ElongatedTriangularOrthobicupola: return "j36";
                case PolyhedronType.ElongatedSquareGyrobicupola: return "j37";
                case PolyhedronType.ElongatedPentagonalOrthobicupola: return "j38";

                // Johnson Solids - Special (J26, J84, J92)
                case PolyhedronType.Gyrobifastigium: return "j26";
                case PolyhedronType.SnubSquareAntiprism: return "j85";
                case PolyhedronType.TriangularHebesphenorotunda: return "j92";

                // Uniform Polyhedra - Stellated Forms
                case PolyhedronType.TruncatedGreatDodecahedron: return "u37";
                case PolyhedronType.RhombicosidodecahedronVariant: return "u38";
                case PolyhedronType.GreatIcosidodecahedron: return "u54";
                case PolyhedronType.TruncatedGreatIcosahedron: return "u55";

                // Uniform Polyhedra - Special Forms
                case PolyhedronType.Tetrahemihexahedron: return "u4";
                case PolyhedronType.Cubohemioctahedron: return "u15";
                case PolyhedronType.GreatRhombicuboctahedron: return "u17";
                case PolyhedronType.SmallRhombihexahedron: return "u18";
                case PolyhedronType.GreatRhombihexahedron: return "u21";

                // Uniform Compounds (UC1-UC5)
                case PolyhedronType.StellaOctangula: return "uc1";
                case PolyhedronType.CompoundCubeOctahedron: return "uc2";
                case PolyhedronType.CompoundDodecahedronIcosahedron: return "uc3";
                case PolyhedronType.CompoundTwoTetrahedra: return "uc4";
                case PolyhedronType.CompoundFiveTetrahedra: return "uc5";

                // Wenninger Stellations
                case PolyhedronType.WenningerW1: return "w1";
                case PolyhedronType.WenningerW2: return "w2";
                case PolyhedronType.WenningerW3: return "w3";
                case PolyhedronType.WenningerW9: return "w9";
                case PolyhedronType.WenningerW20: return "w20";
                case PolyhedronType.WenningerW22: return "w22";

                // Miscellaneous Special Polyhedra
                case PolyhedronType.RhombicEnneacontahedron: return "rhombic_enneacontahedron";
                case PolyhedronType.RhombicHexecontahedron: return "rhombic_hexecontahedron";
                case PolyhedronType.Csaszar: return "csaszar";
                case PolyhedronType.Szilassi: return "szilassi";

                default: return "ico";
            }
        }

        /*-----------------------------------------------------------------------
         * Native API Imports
         *-----------------------------------------------------------------------*/

        // Library Information
        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr anti_get_version();

        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr anti_get_info();

        // Geometry Creation/Destruction
        [DllImport(LIBRARY_NAME)]
        private static extern IntPtr anti_geometry_create();

        [DllImport(LIBRARY_NAME)]
        private static extern void anti_geometry_destroy(IntPtr geom);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_clear(IntPtr geom);

        // Geometry Import/Export
        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_read_off_string(IntPtr geom, string off_data);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_write_off_string(IntPtr geom, out IntPtr buffer, int sig_digits);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_read_resource(IntPtr geom, string name);

        [DllImport(LIBRARY_NAME)]
        private static extern void anti_free_string(IntPtr str);

        // Geometry Query
        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_num_verts(IntPtr geom);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_num_edges(IntPtr geom);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_num_faces(IntPtr geom);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_get_vert(IntPtr geom, int v_idx,
            out double x, out double y, out double z);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_get_all_verts(IntPtr geom, double[] coords, int max_verts);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_face_num_verts(IntPtr geom, int f_idx);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_get_face(IntPtr geom, int f_idx, int[] indices, int max_indices);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_get_all_faces(IntPtr geom, int[] buffer, int buffer_size);

        // Geometry Modification
        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_add_vert(IntPtr geom, double x, double y, double z);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_add_face(IntPtr geom, int[] indices, int num_indices);

        // Transformations
        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_translate(IntPtr geom, double dx, double dy, double dz);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_scale(IntPtr geom, double scale);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_scale_xyz(IntPtr geom, double sx, double sy, double sz);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_rotate_x(IntPtr geom, double angle);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_rotate_y(IntPtr geom, double angle);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_rotate_z(IntPtr geom, double angle);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_unitize(IntPtr geom);

        // Operations
        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_convex_hull(IntPtr geom);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_triangulate(IntPtr geom);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_orient(IntPtr geom);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_orient_reverse(IntPtr geom);

        // Polyhedra Generators
        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_conway_notation(IntPtr geom, string notation);

        // Geometry Information
        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_centroid(IntPtr geom, out double x, out double y, out double z);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_volume(IntPtr geom, out double volume);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_is_oriented(IntPtr geom);

        // Normals
        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_get_face_normals(IntPtr geom, double[] normals, int max_faces);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_get_face_normal(IntPtr geom, int f_idx, out double nx, out double ny, out double nz);

        [DllImport(LIBRARY_NAME)]
        private static extern int anti_geometry_get_vertex_normals(IntPtr geom, double[] normals, int max_verts);

        [DllImport(LIBRARY_NAME)]
        private static extern Status anti_geometry_get_vertex_normal(IntPtr geom, int v_idx, out double nx, out double ny, out double nz);

        /*-----------------------------------------------------------------------
         * High-Level C# API
         *-----------------------------------------------------------------------*/

        /// <summary>
        /// Get the Antiprism library version
        /// </summary>
        public static string GetVersion()
        {
            return Marshal.PtrToStringAnsi(anti_get_version());
        }

        /// <summary>
        /// Get Antiprism library information
        /// </summary>
        public static string GetInfo()
        {
            return Marshal.PtrToStringAnsi(anti_get_info());
        }
    }

    /// <summary>
    /// Represents a 3D polyhedron geometry that can be manipulated and rendered
    /// </summary>
    public class Geometry : IDisposable
    {
        private IntPtr handle;
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

        /// <summary>
        /// Create a zonohedron from a star of vectors.
        /// A zonohedron is formed by the Minkowski sum of line segments.
        /// </summary>
        /// <param name="starVectors">Array of direction vectors</param>
        /// <returns>New geometry containing the zonohedron</returns>
        public static Geometry CreateZonohedron(Vector3[] starVectors)
        {
            if (starVectors == null || starVectors.Length == 0)
                throw new ArgumentException("Star vectors cannot be null or empty");

            Geometry geom = new Geometry();

            // Flatten vectors to double array
            double[] flatVectors = new double[starVectors.Length * 3];
            for (int i = 0; i < starVectors.Length; i++)
            {
                flatVectors[i * 3 + 0] = starVectors[i].x;
                flatVectors[i * 3 + 1] = starVectors[i].y;
                flatVectors[i * 3 + 2] = starVectors[i].z;
            }

            Status status = anti_make_zonohedron(geom.handle, flatVectors, starVectors.Length);
            if (status != Status.OK)
                throw new Exception($"Failed to create zonohedron: {status}");

            return geom;
        }

        /// <summary>
        /// Create a polar zonohedron from an ordered star of vectors.
        /// </summary>
        /// <param name="starVectors">Array of ordered direction vectors</param>
        /// <param name="step">Step this many places to get to next vector (default: 1)</param>
        /// <param name="spiralStep">Step between ridges of spirallohedron, 0 for regular (default: 0)</param>
        /// <returns>New geometry containing the polar zonohedron</returns>
        public static Geometry CreatePolarZonohedron(Vector3[] starVectors, int step = 1, int spiralStep = 0)
        {
            if (starVectors == null || starVectors.Length == 0)
                throw new ArgumentException("Star vectors cannot be null or empty");

            Geometry geom = new Geometry();

            // Flatten vectors to double array
            double[] flatVectors = new double[starVectors.Length * 3];
            for (int i = 0; i < starVectors.Length; i++)
            {
                flatVectors[i * 3 + 0] = starVectors[i].x;
                flatVectors[i * 3 + 1] = starVectors[i].y;
                flatVectors[i * 3 + 2] = starVectors[i].z;
            }

            Status status = anti_make_polar_zonohedron(geom.handle, flatVectors, starVectors.Length, step, spiralStep);
            if (status != Status.OK)
                throw new Exception($"Failed to create polar zonohedron: {status}");

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
            if (count == 0)
                return new Vector3[0];

            double[] coords = new double[count * 3];
            int actual = anti_geometry_get_all_verts(handle, coords, count);

            Vector3[] vertices = new Vector3[actual];
            for (int i = 0; i < actual; i++)
            {
                vertices[i] = new Vector3(
                    (float)coords[i * 3 + 0],
                    (float)coords[i * 3 + 1],
                    (float)coords[i * 3 + 2]
                );
            }

            return vertices;
        }

        /// <summary>
        /// Get all face indices (for Unity mesh)
        /// Returns triangles array suitable for Unity Mesh.triangles
        /// </summary>
        public int[] GetTriangles()
        {
            CheckDisposed();

            // First triangulate to ensure all faces are triangles
            anti_geometry_triangulate(handle);

            int faceCount = FaceCount;
            if (faceCount == 0)
                return new int[0];

            // Allocate buffer (worst case: each face has size header + indices)
            int[] buffer = new int[faceCount * 10];
            int total = anti_geometry_get_all_faces(handle, buffer, buffer.Length);

            // Convert to triangle indices
            System.Collections.Generic.List<int> triangles = new System.Collections.Generic.List<int>();
            int pos = 0;
            while (pos < total)
            {
                int faceSize = buffer[pos++];
                if (faceSize == 3)
                {
                    // Unity uses clockwise winding, so reverse the order
                    triangles.Add(buffer[pos + 2]);
                    triangles.Add(buffer[pos + 1]);
                    triangles.Add(buffer[pos + 0]);
                }
                pos += faceSize;
            }

            return triangles.ToArray();
        }

        /// <summary>
        /// Apply to a Unity Mesh
        /// </summary>
        /// <param name="mesh">Unity mesh to populate</param>
        /// <param name="flatShaded">If true, create hard edges (faceted polyhedron look). If false, smooth shading across edges</param>
        public void ApplyToMesh(Mesh mesh, bool flatShaded = true)
        {
            CheckDisposed();
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            mesh.Clear();

            if (flatShaded)
            {
                // For flat shading, duplicate vertices so each face has its own vertices
                // This creates hard edges - the correct look for polyhedra!
                Vector3[] baseVerts = GetVertices();
                int[] baseTris = GetTriangles();
                Vector3[] faceNormals = GetFaceNormals();

                // Each triangle gets its own copy of vertices
                int numTriangles = baseTris.Length / 3;
                Vector3[] verts = new Vector3[numTriangles * 3];
                Vector3[] normals = new Vector3[numTriangles * 3];
                int[] triangles = new int[numTriangles * 3];

                for (int i = 0; i < numTriangles; i++)
                {
                    int faceIdx = i; // After triangulation, each triangle is its own face
                    Vector3 faceNormal = faceIdx < faceNormals.Length ? faceNormals[faceIdx] : Vector3.up;

                    for (int j = 0; j < 3; j++)
                    {
                        int vertIdx = i * 3 + j;
                        verts[vertIdx] = baseVerts[baseTris[i * 3 + j]];
                        normals[vertIdx] = faceNormal;
                        triangles[vertIdx] = vertIdx;
                    }
                }

                mesh.vertices = verts;
                mesh.normals = normals;
                mesh.triangles = triangles;
            }
            else
            {
                // Smooth shading - share vertices, average normals
                mesh.vertices = GetVertices();
                mesh.triangles = GetTriangles();
                mesh.normals = GetVertexNormals();
            }

            mesh.RecalculateBounds();
        }

        /// <summary>
        /// Scale the geometry uniformly
        /// </summary>
        public Status Scale(float scale)
        {
            CheckDisposed();
            return anti_geometry_scale(handle, scale);
        }

        /// <summary>
        /// Translate the geometry
        /// </summary>
        public Status Translate(Vector3 offset)
        {
            CheckDisposed();
            return anti_geometry_translate(handle, offset.x, offset.y, offset.z);
        }

        /// <summary>
        /// Rotate around X axis (in radians)
        /// </summary>
        public Status RotateX(float angle)
        {
            CheckDisposed();
            return anti_geometry_rotate_x(handle, angle);
        }

        /// <summary>
        /// Rotate around Y axis (in radians)
        /// </summary>
        public Status RotateY(float angle)
        {
            CheckDisposed();
            return anti_geometry_rotate_y(handle, angle);
        }

        /// <summary>
        /// Rotate around Z axis (in radians)
        /// </summary>
        public Status RotateZ(float angle)
        {
            CheckDisposed();
            return anti_geometry_rotate_z(handle, angle);
        }

        /// <summary>
        /// Normalize to unit sphere (centered at origin, radius 1)
        /// </summary>
        public Status Unitize()
        {
            CheckDisposed();
            return anti_geometry_unitize(handle);
        }

        /// <summary>
        /// Calculate convex hull
        /// </summary>
        public Status ConvexHull()
        {
            CheckDisposed();
            return anti_geometry_convex_hull(handle);
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
        /// Triangulate all faces (convert to triangles)
        /// </summary>
        public Status Triangulate()
        {
            CheckDisposed();
            return anti_geometry_triangulate(handle);
        }

        /// <summary>
        /// Create the dual polyhedron (vertices become faces, faces become vertices)
        /// </summary>
        /// <param name="recipRadius">Reciprocation radius (1.0 for standard dual, 0 collapses all vertices to center)</param>
        public Status Dual(double recipRadius = 1.0)
        {
            CheckDisposed();
            return anti_geometry_dual(handle, recipRadius);
        }

        /// <summary>
        /// Truncate vertices (cut off corners)
        /// </summary>
        /// <param name="ratio">Truncation ratio (0.0-1.0, typically 0.3-0.5)</param>
        /// <param name="order">Truncate only vertices with this vertex order (0 for all)</param>
        public Status Truncate(double ratio = 0.3333, int order = 0)
        {
            CheckDisposed();
            return anti_geometry_truncate(handle, ratio, order);
        }

        /// <summary>
        /// Kis operation - place a pyramid on each face
        /// </summary>
        /// <param name="n">Only kis faces with n sides (0 for all faces)</param>
        public Status Kis(int n = 0)
        {
            CheckDisposed();
            return anti_geometry_kis(handle, n);
        }

        /// <summary>
        /// Ambo operation - create vertices at edge midpoints (also known as rectify)
        /// </summary>
        public Status Ambo()
        {
            CheckDisposed();
            return anti_geometry_ambo(handle);
        }

        /// <summary>
        /// Gyro operation - rotate and subdivide faces creating pentagons
        /// </summary>
        /// <param name="n">Reserved for future use</param>
        public Status Gyro(int n = 1)
        {
            CheckDisposed();
            return anti_geometry_gyro(handle, n);
        }

        /// <summary>
        /// Join operation - dual of ambo (creates rhombic faces)
        /// </summary>
        public Status Join()
        {
            CheckDisposed();
            return anti_geometry_join(handle);
        }

        /// <summary>
        /// Needle operation - elongated kis (creates sharp spikes)
        /// </summary>
        /// <param name="height">Height multiplier for needle points</param>
        public Status Needle(double height = 2.0)
        {
            CheckDisposed();
            return anti_geometry_needle(handle, height);
        }

        /// <summary>
        /// Zip operation - dual of kis (creates vertices where kis placed pyramids)
        /// </summary>
        public Status Zip()
        {
            CheckDisposed();
            return anti_geometry_zip(handle);
        }

        /// <summary>
        /// Subdivide operation - subdivide each face into smaller quadrilateral faces
        /// </summary>
        /// <param name="n">Reserved for future use</param>
        /// <param name="m">Reserved for future use</param>
        public Status Subdivide(int n = 2, int m = 0)
        {
            CheckDisposed();
            return anti_geometry_subdivide(handle, n, m);
        }

        /// <summary>
        /// Expand operation - double ambo (separates faces with squares)
        /// </summary>
        /// <param name="n">Reserved for future use</param>
        /// <param name="m">Reserved for future use</param>
        public Status Expand(int n = 2, int m = 0)
        {
            CheckDisposed();
            return anti_geometry_expand(handle, n, m);
        }

        /// <summary>
        /// Meta operation - kis + dual (creates complex stellated forms)
        /// </summary>
        /// <param name="n">Reserved for future use</param>
        public Status Meta(int n = 2)
        {
            CheckDisposed();
            return anti_geometry_meta(handle, n);
        }

        /// <summary>
        /// Bevel operation - truncate + ambo (chamfers edges and vertices)
        /// </summary>
        /// <param name="n">Reserved for future use</param>
        /// <param name="ratio">Truncation ratio</param>
        public Status Bevel(int n = 2, double ratio = 0.3333)
        {
            CheckDisposed();
            return anti_geometry_bevel(handle, n, ratio);
        }

        /// <summary>
        /// Snub operation - dual + gyro (creates twisted forms)
        /// </summary>
        /// <param name="n">Reserved for future use</param>
        public Status Snub(int n = 2)
        {
            CheckDisposed();
            return anti_geometry_snub(handle, n);
        }

        /// <summary>
        /// Ortho operation - join + join
        /// </summary>
        /// <param name="n">Reserved for future use</param>
        /// <param name="m">Reserved for future use</param>
        public Status Ortho(int n = 2, int m = 0)
        {
            CheckDisposed();
            return anti_geometry_ortho(handle, n, m);
        }

        /// <summary>
        /// Get all vertex normals (averaged from surrounding face normals)
        /// </summary>
        public Vector3[] GetVertexNormals()
        {
            CheckDisposed();
            int count = VertexCount;
            if (count == 0)
                return new Vector3[0];

            double[] normals = new double[count * 3];
            int actual = anti_geometry_get_vertex_normals(handle, normals, count);

            Vector3[] vnormals = new Vector3[actual];
            for (int i = 0; i < actual; i++)
            {
                // Negate normals because we reverse triangle winding for Unity
                vnormals[i] = new Vector3(
                    -(float)normals[i * 3 + 0],
                    -(float)normals[i * 3 + 1],
                    -(float)normals[i * 3 + 2]
                );
            }

            return vnormals;
        }

        /// <summary>
        /// Get all face normals
        /// </summary>
        public Vector3[] GetFaceNormals()
        {
            CheckDisposed();
            int count = FaceCount;
            if (count == 0)
                return new Vector3[0];

            double[] normals = new double[count * 3];
            int actual = anti_geometry_get_face_normals(handle, normals, count);

            Vector3[] fnormals = new Vector3[actual];
            for (int i = 0; i < actual; i++)
            {
                // Negate normals because we reverse triangle winding for Unity
                fnormals[i] = new Vector3(
                    -(float)normals[i * 3 + 0],
                    -(float)normals[i * 3 + 1],
                    -(float)normals[i * 3 + 2]
                );
            }

            return fnormals;
        }

        /// <summary>
        /// Get polyhedron data in a clean format for mesh libraries.
        /// Preserves original face structure (no triangulation) - pentagons stay pentagons!
        /// </summary>
        /// <param name="vertices">All vertices as Vector3</param>
        /// <param name="faceIndices">Each face as array of vertex indices</param>
        public void GetPolyhedronData(out Vector3[] vertices, out int[][] faceIndices)
        {
            CheckDisposed();

            // Get vertices
            vertices = GetVertices();

            // Get faces in original form (not triangulated)
            int faceCount = FaceCount;
            if (faceCount == 0)
            {
                faceIndices = new int[0][];
                return;
            }

            // Allocate buffer for face data (estimate size)
            int bufferSize = faceCount * 10; // Most faces won't exceed 10 vertices
            int[] buffer = new int[bufferSize];
            int totalSize = anti_geometry_get_all_faces(handle, buffer, bufferSize);

            if (totalSize < 0)
            {
                faceIndices = new int[0][];
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
        }

        // P/Invoke imports
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
        private static extern int anti_geometry_get_all_verts(IntPtr geom, double[] coords, int max_verts);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_get_all_faces(IntPtr geom, int[] buffer, int buffer_size);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_triangulate(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_scale(IntPtr geom, double scale);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_translate(IntPtr geom, double dx, double dy, double dz);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_rotate_x(IntPtr geom, double angle);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_rotate_y(IntPtr geom, double angle);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_rotate_z(IntPtr geom, double angle);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_unitize(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_convex_hull(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_orient(IntPtr geom);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_geometry_dual(IntPtr geom, double recip_rad);

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

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_get_vertex_normals(IntPtr geom, double[] normals, int max_verts);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_get_face_normals(IntPtr geom, double[] normals, int max_faces);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_zonohedron(IntPtr geom, double[] star_vectors, int num_vectors);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern Status anti_make_polar_zonohedron(IntPtr geom, double[] star_vectors, int num_vectors, int step, int spiral_step);
    }
}
