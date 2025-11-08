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
    /// Unity wrapper for the Antiprism polyhedra library
    /// </summary>
    public class AntiprismPlugin
    {
        // Platform-specific library names
        #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        private const string LIBRARY_NAME = "antiprism";
        #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        private const string LIBRARY_NAME = "libantiprism";
        #elif UNITY_ANDROID
        private const string LIBRARY_NAME = "antiprism";
        #elif UNITY_IOS
        private const string LIBRARY_NAME = "__Internal";
        #else
        private const string LIBRARY_NAME = "antiprism";
        #endif

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
        /// <param name="useAntiprismNormals">If true, use Antiprism-calculated normals (averaged from face normals). If false, use Unity's RecalculateNormals (default)</param>
        public void ApplyToMesh(Mesh mesh, bool useAntiprismNormals = false)
        {
            CheckDisposed();
            if (mesh == null)
                throw new ArgumentNullException("mesh");

            mesh.Clear();
            mesh.vertices = GetVertices();
            mesh.triangles = GetTriangles();

            if (useAntiprismNormals)
            {
                mesh.normals = GetVertexNormals();
            }
            else
            {
                mesh.RecalculateNormals();
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
                vnormals[i] = new Vector3(
                    (float)normals[i * 3 + 0],
                    (float)normals[i * 3 + 1],
                    (float)normals[i * 3 + 2]
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
                fnormals[i] = new Vector3(
                    (float)normals[i * 3 + 0],
                    (float)normals[i * 3 + 1],
                    (float)normals[i * 3 + 2]
                );
            }

            return fnormals;
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
        private static extern int anti_geometry_get_vertex_normals(IntPtr geom, double[] normals, int max_verts);

        [DllImport(AntiprismPlugin.LIBRARY_NAME)]
        private static extern int anti_geometry_get_face_normals(IntPtr geom, double[] normals, int max_faces);
    }
}
