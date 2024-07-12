using UnityEngine;
using UnityEngine.Profiling;

namespace KCC
{
    public class KCCCollider
    {
        public readonly GameObject GameObject;
        public readonly CapsuleCollider Collider;
        public readonly Transform Transform;
        
        public bool IsSpawned;
        public float Radius;
        public float Height;
        public float Extent;
        public int Layer;

        public KCCCollider(KCCSettings settings, GameObject colliderObj, Transform parentTransform, CapsuleCollider collider)
        {
            GameObject = colliderObj;
            //SetLayer
            Transform = colliderObj.transform;
            Transform.SetParent(parentTransform, false);
            Transform.localPosition = Vector3.zero; 
            Transform.localRotation = Quaternion.identity;
            Transform.localScale = Vector3.one;

            Radius = settings.Radius;
            Height = settings.Height;
            Extent = settings.Extent;

            Collider = collider;
            Collider.direction = 1;
            Collider.radius = Radius;
            Collider.height = Height;
            Collider.center = new Vector3(0.0f, Height * 0.5f, 0.0f);
            IsSpawned = true;
        }
    }
}