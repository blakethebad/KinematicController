using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace KCC
{
    [RequireComponent(typeof(Rigidbody))]
    public class KinematicController : MonoBehaviour
    {
        [SerializeField] private KCCSettings _settings;
        [SerializeField] private LayerMask CollisionMask;

        public LayerMask LayerMask => CollisionMask;
        public Rigidbody Rigidbody => _rigidbody;
        public Transform Transform => _transform;
        public KCCCollider Collider => _collider;
        public KCCSettings Settings => _settings;
        
        private Transform _transform;
        private Rigidbody _rigidbody;
        private KCCCollider _collider;
        private KCCData _currentData;

        private Dictionary<CollisionType, ICollisionStep> _collisionSteps;
        private Dictionary<MovementType, ICalculationStep> _movementSteps;
        private Dictionary<Type, IKCCStep> _runningSteps;

        public KCCData Data => _currentData;

        private void Start()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = true;
            _currentData = new KCCData(Settings);
            _collider = new KCCCollider();
            _collider.InitCollider(this);
            
            _collisionSteps = new Dictionary<CollisionType, ICollisionStep>()
            {
                { CollisionType.OverlapCollision , new OverlapCollisionStep()},
            };

            _movementSteps = new Dictionary<MovementType, ICalculationStep>()
            {
                { MovementType.SimpleMovement, new CalculationStep() }
            };
            
            _runningSteps = new Dictionary<Type, IKCCStep>()
            {
                { typeof(IBeginMoveStep) , new BeginMoveStep()},
                { typeof(ICalculationStep) , _movementSteps[_settings.MovementType]},
                { typeof(ICollisionStep) , _collisionSteps[_settings.CollisionType]}
            };
        }

        public void Move(Vector3 inputDirection) => Data.InputDirection = inputDirection;

        public void Rotate(float pitch, float yaw) => Data.AddLookRotation(pitch, yaw);

        public void Jump(Vector3 impulse) => Data.JumpImpulse += impulse;

        private void Update()
        {
            ExecuteStage<IBeginMoveStep>(Data);
            ExecuteStage<ICalculationStep>(Data);
            
            Vector3 deltaPosition = Data.DesiredVelocity * Data.DeltaTime; // + ExternalDelta??
            Data.TargetPosition = Data.CurrentPosition + deltaPosition;
            
            ExecuteStage<ICollisionStep>(Data);
            Data.CurrentPosition = Data.TargetPosition;
            
            _rigidbody.position = Data.TargetPosition;
            _transform.SetPositionAndRotation(Data.TargetPosition, Data.TransformRotation);
            ExecuteStage<IAfterMoveStep>(Data);
            ExecuteStage<IEndMoveStep>(Data);
        }

        private void ExecuteStage<TStage>(KCCData data) where TStage : IKCCStep
        {
            IKCCStep executedStage = null;

            if (_runningSteps.ContainsKey(typeof(TStage)))
                executedStage = _runningSteps[typeof(TStage)];

            executedStage?.Execute(this, data);
        }
    }

}

