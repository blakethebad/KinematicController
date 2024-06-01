using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

namespace KCC
{
    public sealed class KCCData
    {

        public Vector3 Gravity;
        public Vector3 InputDirection;
        public Vector3 CurrentPosition;
        public Vector3 TargetPosition;
        public float DeltaTime => Time.deltaTime;
        public float FixedDeltaTime => Time.fixedDeltaTime;
        
        //Ground Properties
        public bool IsGrounded;
        public bool IsSteppingUp;
        public bool IsSnappingToGround;
        public float GroundAngle;
        public float GroundDistance;
        public Vector3 GroundNormal;
        public Vector3 GroundTangent;
        
        //Movement Properties
        public float JumpFrames;
        public Vector3 JumpImpulse;
        public Vector3 KinematicTangent;
        public Vector3 KinematicVelocity;
        public Vector3 KinematicDirection;
        public Vector3 DynamicVelocity;
        public Vector3 DesiredVelocity => DynamicVelocity + KinematicVelocity;
        public Vector3 RealVelocity => (TargetPosition - CurrentPosition) / DeltaTime;
        
        //Rotation Properties
        private float _lookPitch;
        private float _lookYaw;
        private bool _lookRotationCalculated;
        private bool _lookDirectionCalculated;
        private bool _transformRotationCalculated;
        private bool _transformDirectionCalculated;
        private Quaternion _lookRotation;
        private Vector3 _lookDirection;
        private Quaternion _transformRotation;
        private Vector3 _transformDirection;
        public Quaternion LookRotation => GetLookRotation();
        public Vector3 LookDirection => GetLookDirection();
        public Quaternion TransformRotation => GetTransformRotation();
        public Vector3 TransformDirection => GetTransformDirection();


        public KCCData(KCCSettings settings)
        {
            Gravity = new Vector3(0.0f, settings.Gravity, 0.0f);
            Gravity = new Vector3(0.0f, settings.Gravity, 0.0f);
            GroundNormal = Vector3.up;
            IsGrounded = true;
            IsSteppingUp = default;
            IsSnappingToGround = default;
            GroundDistance = 0;
            JumpFrames = 0;
        }
        
        private Quaternion GetLookRotation()
        {
            if (_lookRotationCalculated != false) return _lookRotation;
            _lookRotation = Quaternion.Euler(_lookPitch, _lookYaw, 0.0f);
            _lookRotationCalculated = true;
            return _lookRotation;
        }

        private Vector3 GetLookDirection()
        {
            if (_lookDirectionCalculated != false) return _lookDirection;
            _lookDirection = LookRotation * Vector3.forward;
            _lookDirectionCalculated = true;
            return _lookDirection;
        }

        private Quaternion GetTransformRotation()
        {
            if (_transformRotationCalculated != false) return _transformRotation;
            _transformRotation = Quaternion.Euler(0.0f, _lookYaw, 0.0f);
            _transformRotationCalculated = true;
            return _transformRotation;
        }

        private Vector3 GetTransformDirection()
        {
            if (_transformDirectionCalculated) return _transformDirection;
            _transformDirection = TransformRotation * Vector3.forward;
            _transformDirectionCalculated = true;
            return _transformDirection;
        }

        public void ResetCollisionData()
        {
            IsGrounded = default;
            IsSteppingUp = default;
            IsSnappingToGround = default;
            GroundNormal = default;
            GroundTangent = default;
            GroundDistance = default;
            GroundAngle = default;
        }
        
        public void AddLookRotation(float pitch, float yaw)
        {
            if (pitch != 0.0f)
            {
                _lookPitch = Mathf.Clamp(_lookPitch + pitch, -90.0f, 90.0f);
                _lookRotationCalculated = false;
                _lookDirectionCalculated = false;
            }

            if (yaw != 0.0f)
            {
                float lookYaw = _lookYaw + yaw;
                while (lookYaw > 180.0f)
                {
                    lookYaw -= 360.0f;
                }

                while (lookYaw < -180.0f)
                {
                    lookYaw += 360.0f;
                }

                _lookYaw = lookYaw;
                _lookRotationCalculated = false;
                _lookDirectionCalculated = false;
                _transformRotationCalculated = false;
            }
        }
    }
}