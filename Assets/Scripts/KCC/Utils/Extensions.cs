using System.Runtime.CompilerServices;
using UnityEngine;

namespace KCC
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ClampToNormalized(this Vector3 vector)
        {
            if (Vector3.SqrMagnitude(vector) > 1.0f)
            {
                vector.Normalize();
            }

            return vector;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 OnlyXZ(this Vector3 vector)
        {
            vector.y = 0.0f;
            return vector;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEqual(this Vector3 vector, Vector3 other)
        {
            return vector.x == other.x && vector.y == other.y && vector.z == other.z;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlmostZero(this Vector3 vector, float tolerance = 0.01f)
        {
            return vector.x < tolerance && vector.x > -tolerance
                                        && vector.y < tolerance && vector.y > -tolerance
                                        && vector.z < tolerance && vector.z > -tolerance;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsZero(this Vector3 vector)
        {
            return vector.x == 0.0f && vector.y == 0.0f && vector.z == 0.0f;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlmostZero(this float value, float tolerance = 0.01f)
        {
            return value < tolerance && value > -tolerance;
        }
    }
}