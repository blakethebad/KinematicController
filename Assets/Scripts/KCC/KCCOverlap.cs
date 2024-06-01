using UnityEngine;

namespace KCC
{
    public class KCCOverlap
    {
        public Vector3 Position;
        public float Radius;
        public float Height;
        public float Extent;
        public LayerMask LayerMask;
        public QueryTriggerInteraction TriggerInteraction;
        public KCCOverlapHit[] AllHits;
        public int AllHitCount;
        public KCCOverlapHit[] ColliderHits;
        public int ColliderHitCount;
        public KCCOverlapHit[] TriggerHits;
        public int TriggerHitCount;

        public KCCOverlap()
        {
            AllHits = new KCCOverlapHit[20];
            ColliderHits = new KCCOverlapHit[20];
            TriggerHits = new KCCOverlapHit[20];
            
            for (int i = 0; i < 20; ++i)
            {
                AllHits[i] = new KCCOverlapHit();
            }
        }

        public void AddHit(Collider collider)
        {
            if(AllHitCount == AllHits.Length)
                return;
            KCCOverlapHit hit = AllHits[AllHitCount];
            if (hit.Set(collider))
            {
                ++AllHitCount;

                if (hit.IsTrigger)
                {
                    TriggerHits[TriggerHitCount] = hit;
                    ++TriggerHitCount;
                }
                else
                {
                    ColliderHits[ColliderHitCount] = hit;
                    ++ColliderHitCount;
                }
            }
        }

        public void Reset(bool hardReset)
        {
            Position           = default;
            Radius             = default;
            Height             = default;
            Extent             = default;
            LayerMask          = default;
            TriggerInteraction = QueryTriggerInteraction.Collide;
            AllHitCount        = default;
            TriggerHitCount    = default;
            ColliderHitCount   = default;

            if (hardReset == true)
            {
                for (int i = 0, count = AllHits.Length; i < count; ++i)
                {
                    AllHits[i].Reset();
                }
            }
        }
    }
}