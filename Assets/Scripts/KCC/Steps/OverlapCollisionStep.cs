using TMPro;
using UnityEngine;

namespace KCC
{
    public class OverlapCollisionStep : ICollisionStep
    {
        private readonly ICollisionResolver _resolver;
        
        private Collider[] _hits = new Collider[10];
        private int _hitCount = default;
        private float _maxGroundDot = default;
        private Vector3 _maxGroundNormal = default;
        private Vector3 _averageGroundNormal = default;
        private float _minGroundDot;

        public OverlapCollisionStep(ICollisionResolver resolver)
        {
            _resolver = resolver;
        }

        public void Execute(KinematicController kcc, KCCData data)
        {
            _minGroundDot = Mathf.Cos(Mathf.Clamp(kcc.Settings.MaxGroundAngle, 0.0f, 90.0f) * Mathf.Deg2Rad);
            _maxGroundDot = default;
            _maxGroundNormal = default;
            _averageGroundNormal = default; 
            data.ResetCollisionData();
            float radius = kcc.Collider.Radius + kcc.Collider.Extent;
            Vector3 targetPosition = data.TargetPosition;
            Vector3 upPos = targetPosition + new Vector3(0, kcc.Collider.Height - radius, 0.0f);
            Vector3 downPos = targetPosition + new Vector3(0.0f, kcc.Collider.Radius, 0.0f);

            _hitCount = Physics.OverlapCapsuleNonAlloc(downPos, upPos, radius, _hits, kcc.LayerMask);

            _resolver.Reset();
            for (int i = 0; i < _hitCount; i++)
            {
                Collider hitCollider = _hits[i];

                bool hasPenetration = Physics.ComputePenetration(kcc.Collider.Collider, targetPosition, 
                    Quaternion.identity, hitCollider, hitCollider.transform.position, hitCollider.transform.rotation,
                    out Vector3 direction, out float distance);
                
                if(!hasPenetration)
                    continue;

                float upDot = Vector3.Dot(direction, Vector3.up);

                if (upDot > _minGroundDot)
                {
                    data.IsGrounded = true;
                    if (upDot >= _maxGroundDot)
                    {
                        _maxGroundDot = upDot;
                        _maxGroundNormal = direction;
                    }
                    _averageGroundNormal += direction * upDot;
                }
                _resolver.AddCorrection(direction, distance);
            }

            Vector3 correction = _resolver.ResolveCorrection();
            targetPosition += correction;

            RecalculateGround(kcc, data, targetPosition);

            data.TargetPosition = targetPosition;
        }

        private void RecalculateGround(KinematicController kcc, KCCData data, Vector3 targetPos)
        {
            if (data.JumpImpulse.IsZero() && data.IsGrounded == false)
            {
                Vector3 closestGroundNormal = Vector3.up;
                float closestDistance = 1000.0f;
                for (int i = 0; i < _hitCount; i++)
                {
                    Collider hit = _hits[i];
                    bool isGrounded = PhysicsHelper.CheckGround(kcc.Collider.Collider, targetPos, hit, 
                        hit.transform.position, hit.transform.rotation, kcc.Collider.Radius, kcc.Collider.Height,
                        kcc.Collider.Extent, _minGroundDot, out Vector3 checkGroundNormal,
                        out float checkGroundDistance, out bool isWithinExtent);

                    if (!isGrounded) continue;
                    
                    data.IsGrounded = true;
                    if (checkGroundDistance < closestDistance)
                    {
                        closestDistance = checkGroundDistance;
                        closestGroundNormal = checkGroundNormal;
                    }
                }

                if (data.IsGrounded)
                {
                    _maxGroundNormal = closestGroundNormal;
                    _averageGroundNormal = closestGroundNormal;
                    data.GroundDistance = closestDistance;
                }
            }

            if(data.IsGrounded == false)
                return;
            
            if(_averageGroundNormal.IsEqual(_maxGroundNormal) == false)
                _averageGroundNormal.Normalize();
            
            data.GroundNormal = _averageGroundNormal;
            data.GroundAngle = Vector3.Angle(data.GroundNormal, Vector3.up);


            if (PhysicsHelper.ProjectOnGround(data.GroundNormal, data.GroundNormal.OnlyXZ(),
                    out Vector3 projectedNormal))
            {
                data.GroundTangent = projectedNormal.normalized;
                return;
            }

            if (PhysicsHelper.ProjectOnGround(data.GroundNormal, data.DesiredVelocity.OnlyXZ(),
                    out Vector3 projectedVelocity))
            {
                data.GroundTangent = projectedVelocity.normalized;
                return;
            }

            data.GroundTangent = data.TransformDirection;
        }
    }
}