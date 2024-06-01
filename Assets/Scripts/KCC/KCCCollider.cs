using UnityEngine;
using UnityEngine.Profiling;

namespace KCC
{
    public class KCCCollider
    {
        public bool IsSpawned;
        public GameObject GameObject;
        public Transform Transform;
        public CapsuleCollider Collider;
        public bool IsTrigger;
        public float Radius;
        public float Height;
        public float Extent = 0.035f;
        public int Layer;

        public void InitCollider(KinematicController kcc)
        {
            GameObject = new GameObject("Collider");
            //SetLayer
            Transform = GameObject.transform;
            Transform.SetParent(kcc.Transform, false);
            Transform.localPosition = Vector3.zero; 
            Transform.localRotation = Quaternion.identity;
            Transform.localScale = Vector3.one;

            Radius = 0.5f;
            Height = 2f;
            Extent = 0.035f;

            Collider = GameObject.AddComponent<CapsuleCollider>();
            Collider.direction = 1;
            Collider.isTrigger = IsTrigger;
            Collider.radius = Radius;
            Collider.height = Height;
            Collider.center = new Vector3(0.0f, Height * 0.5f, 0.0f);
            IsSpawned = true;
        }
    }
}