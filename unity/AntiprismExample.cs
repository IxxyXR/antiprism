using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AntiprismExample : MonoBehaviour
{
    [DllImport("antiprism_unity", CallingConvention = CallingConvention.Cdecl)]
    private static extern int antiprism_command(string cmd, out IntPtr output);

    [DllImport("antiprism_unity", CallingConvention = CallingConvention.Cdecl)]
    private static extern void antiprism_free(IntPtr buffer);

    void Start()
    {
        IntPtr ptr;
        int err = antiprism_command("off_align -P I", out ptr);
        if (err != 0)
        {
            Debug.LogError($"antiprism_command failed with code {err}");
            return;
        }

        string offData = Marshal.PtrToStringAnsi(ptr);
        antiprism_free(ptr);
        Debug.Log(offData.Substring(0, Math.Min(offData.Length, 200)));
    }
}
