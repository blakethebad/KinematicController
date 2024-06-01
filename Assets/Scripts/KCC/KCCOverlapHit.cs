using System;
using UnityEngine;

namespace KCC
{
    public class KCCOverlapHit
    {
        public Collider Collider;
        public Transform Transform;
        public bool           IsConvex;
        public bool           IsTrigger;
        public bool           IsPrimitive;
        public bool           IsConvertible;
        public bool           IsWithinExtent;
        public bool           HasPenetration;
        public float          MaxPenetration;
        public float          UpDirectionDot;
        public Vector3        CachedPosition;
        public Quaternion     CachedRotation;
        
        private static readonly Type SphereColliderType  = typeof(SphereCollider);
        private static readonly Type CapsuleColliderType = typeof(CapsuleCollider);
        private static readonly Type BoxColliderType     = typeof(BoxCollider);
        private static readonly Type MeshColliderType    = typeof(MeshCollider);
        private static readonly Type TerrainColliderType = typeof(TerrainCollider);

        public bool Set(Collider collider)
        {
            Type colliderType = collider.GetType();

            if (colliderType == BoxColliderType)
            {
                IsConvex      = true;
                IsPrimitive   = true;
                IsConvertible = false;
            }
            else if (colliderType == MeshColliderType)
            {
                MeshCollider meshCollider = (MeshCollider)collider;

                IsConvex      = meshCollider.convex;
                IsPrimitive   = false;
                IsConvertible = false;

                if (IsConvex == true)
                {
                    Mesh mesh = meshCollider.sharedMesh;
                    IsConvertible = mesh != null && mesh.isReadable == true;
                }
            }
            else if (colliderType == TerrainColliderType)
            {
                IsConvex      = false;
                IsPrimitive   = false;
                IsConvertible = false;
            }
            else if (colliderType == SphereColliderType)
            {
                IsConvex      = true;
                IsPrimitive   = true;
                IsConvertible = false;
            }
            else if (colliderType == CapsuleColliderType)
            {
                IsConvex      = true;
                IsPrimitive   = true;
                IsConvertible = false;
            }
            else
            {
                return false;
            }

            Collider       = collider;
            Transform      = collider.transform;
            IsTrigger      = collider.isTrigger;
            IsWithinExtent = default;
            HasPenetration = default;
            MaxPenetration = default;
            UpDirectionDot = default;

            
            return true;
        }

        public void Reset()
        {
            Collider  = default;
            Transform = default;
        }
    }
}