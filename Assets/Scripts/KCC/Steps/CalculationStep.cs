using UnityEngine;

namespace KCC
{
    public class CalculationStep : ICalculationStep
    {
        public void Execute(KinematicController kcc, KCCData data)
        {
            Vector3 dynamicVelocity = GetDynamicVelocity(kcc, data);
            data.DynamicVelocity = dynamicVelocity;
            data.KinematicDirection = data.InputDirection.normalized.OnlyXZ();
            
            if (data.IsGrounded)
            {
                if (data.KinematicDirection.IsAlmostZero(0.001f) == false && 
                    PhysicsHelper.ProjectOnGround(data.GroundNormal, data.KinematicDirection, out Vector3 projectedDirection))
                {
                    data.KinematicTangent = projectedDirection.normalized;
                }
                else
                {
                    data.KinematicTangent = data.GroundTangent;
                }
            }
            else
            {
                data.KinematicTangent = data.KinematicDirection.IsAlmostZero(0.001f) == false
                    ? data.KinematicDirection.normalized
                    : data.TransformDirection;
            }

            Vector3 kinematicVelocity = GetKinematicVelocity(kcc, data);
            data.KinematicVelocity = kinematicVelocity;
        }

        private Vector3 GetDynamicVelocity(KinematicController kcc, KCCData data)
        {
            Vector3 dynamicVelocity = data.DynamicVelocity;
            if (data.IsGrounded == false ||
                (data.IsSteppingUp == false && (data.IsSnappingToGround || data.GroundDistance > 0.001f)))
            {
                dynamicVelocity += data.Gravity * data.DeltaTime;
            }

            if (data.JumpImpulse.IsZero() == false && kcc.Settings.JumpMultiplier > 0.0f)
            {
                dynamicVelocity -= Vector3.Scale(dynamicVelocity, data.JumpImpulse.normalized);
                dynamicVelocity += data.JumpImpulse * kcc.Settings.JumpMultiplier / kcc.Rigidbody.mass;
                data.JumpFrames++;
            }
            //External velocity/acceleration/impulse added here

            if (dynamicVelocity.IsZero() == false)
            {
                if (dynamicVelocity.IsAlmostZero(0.001f))
                    dynamicVelocity = default;
                else
                {
                    if (data.IsGrounded)
                    {
                        Vector3 frictionAxis = Vector3.one;
                        if (data.GroundDistance > 0.001f || data.IsSnappingToGround)
                        {
                            frictionAxis.y = default;
                        }
                        
                        dynamicVelocity += PhysicsHelper.GetFriction(dynamicVelocity,dynamicVelocity, frictionAxis, data.GroundNormal,kcc.Settings.KinematicSpeed, 
                            true, 0.0f, 0.0f,kcc.Settings.DynamicGroundFriction, data.FixedDeltaTime);
                    }
                    else
                    {
                        dynamicVelocity += PhysicsHelper.GetFriction(dynamicVelocity, dynamicVelocity, new Vector3(1.0f, 0.0f, 1.0f), kcc.Settings.KinematicSpeed, 
                            true, 0.0f, 0.0f,kcc.Settings.DynamicAirFriction, data.FixedDeltaTime);
                    }
                }
            }

            data.JumpImpulse = default;
            return dynamicVelocity;
        }

        private Vector3 GetKinematicVelocity(KinematicController kcc, KCCData data)
        {
            Vector3 kinematicVelocity = data.KinematicVelocity;
            if (data.IsGrounded)
            {
                if (kinematicVelocity.IsAlmostZero() == false && 
                    PhysicsHelper.ProjectOnGround(data.GroundNormal, kinematicVelocity.normalized, out Vector3 projectedKinematicVelocity) == true)
                {
                    kinematicVelocity = projectedKinematicVelocity.normalized * kinematicVelocity.magnitude;
                }

                if (data.KinematicDirection.IsAlmostZero())
                {
                    kinematicVelocity = kinematicVelocity + PhysicsHelper.GetFriction(kinematicVelocity,
                        kinematicVelocity, Vector3.one, data.GroundNormal, kcc.Settings.KinematicSpeed,
                        true, 0.0f, 0.0f, kcc.Settings.KinematicGroundFriction, data.FixedDeltaTime);
                    return kinematicVelocity;
                }
            }
            else
            {
                if (data.KinematicDirection.IsAlmostZero())
                {
                    kinematicVelocity = kinematicVelocity + PhysicsHelper.GetFriction(kinematicVelocity, kinematicVelocity, new Vector3(1.0f, 0.0f, 1.0f), 
                        kcc.Settings.KinematicSpeed, true, 0.0f, 0.0f, kcc.Settings.KinematicAirFriction, data.FixedDeltaTime);
                }
            }
            
            Vector3 moveDirection = kinematicVelocity;

            Vector3 acceleration;
            Vector3 friction;

            if (data.IsGrounded)
            {
                acceleration = PhysicsHelper.GetAcceleration(kinematicVelocity, data.KinematicTangent, Vector3.one, kcc.Settings.KinematicSpeed, false, 
                    data.KinematicDirection.magnitude, 0.0f,kcc.Settings.KinematicGroundAcceleration, 0.0f, data.FixedDeltaTime);
                friction     = PhysicsHelper.GetFriction(kinematicVelocity, moveDirection, Vector3.one, data.GroundNormal, 
                    kcc.Settings.KinematicSpeed, false, 0.0f, 0.0f, kcc.Settings.KinematicGroundFriction, data.FixedDeltaTime);

            }
            else
            {
                acceleration = PhysicsHelper.GetAcceleration(kinematicVelocity, data.KinematicTangent, Vector3.one, kcc.Settings.KinematicSpeed, 
                    false, data.KinematicDirection.magnitude, 0.0f, kcc.Settings.KinematicAirAcceleration, 0.0f, data.FixedDeltaTime);
                friction     = PhysicsHelper.GetFriction(kinematicVelocity, moveDirection, new Vector3(1.0f, 0.0f, 1.0f), kcc.Settings.KinematicSpeed, 
                    false, 0.0f, 0.0f,kcc.Settings.KinematicAirFriction, data.FixedDeltaTime);
            }

            kinematicVelocity = PhysicsHelper.CombineAccelerationAndFriction(kinematicVelocity, acceleration, friction);

            if (kinematicVelocity.sqrMagnitude > kcc.Settings.KinematicSpeed * kcc.Settings.KinematicSpeed)
            {
                kinematicVelocity = kinematicVelocity / Vector3.Magnitude(kinematicVelocity) * kcc.Settings.KinematicSpeed;
            }

            if (data.JumpFrames > 0 && kinematicVelocity.y < 0.0f)
            {
                kinematicVelocity.y = 0.0f;
            }

            return kinematicVelocity;
        }
    }
}