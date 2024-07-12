using UnityEngine;

namespace KCC
{
    public interface ICollisionResolver
    {
        public void Reset();
        public void AddCorrection(Vector3 direction, float distance);
        public Vector3 ResolveCorrection();
    }
}