using System;
using UnityEngine;

namespace KCC
{
    [Serializable]
    public class KCCSettings
    {
        public MovementType MovementType;
        public float KinematicSpeed = 8.0f;
        public float JumpMultiplier = 1;
        public Vector2 Sensitivity = new Vector2(5.0f,5.0f);
        [Range(-90.0f, 0)] public float Gravity;
        
        [Header("Friction Settings")]
        public float KinematicGroundFriction = 35.0f;
        public float KinematicGroundAcceleration = 50.0f;

        public float KinematicAirFriction = 2.0f;
        public float KinematicAirAcceleration = 5.0f;

        public float DynamicGroundFriction = 20.0f;
        public float DynamicAirFriction = 2.0f;
        
        [Header("Collision Settings")]
        
        public CollisionType CollisionType;
        [Range(5.0f, 85.0f)] public float MaxGroundAngle = 60.0f;
        [Range(1.0f, 10.0f)] public float MaxWallAngle = 5.0f;
        [Range(5.0f, 60.0f)] public float MaxHangAngle = 30.0f;
        
        [Range(1,8)] public int CollisionPrecision;

        [Header("Collider Settings")] 
        public float Radius = 0.5f;
        public float Height = 2f;
        public float Extent = 0.035f;
    }
}