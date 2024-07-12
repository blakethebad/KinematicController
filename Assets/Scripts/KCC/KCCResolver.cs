using System;
using UnityEngine;

namespace KCC
{
    public class KCCResolver : ICollisionResolver
    {
	    public int Size => _size;
	    public int Iterations => _iterations;
	    
        private int _size;
        private int _iterations;
        private Correction[] _corrections;
        private Vector3 _minCorrection;
        private Vector3 _maxCorrection;
        private Vector3 _targetCorrection;

        public KCCResolver()
        {
            _corrections = new Correction[64];
            for (int i = 0; i < 64; i++)
            {
                _corrections[i] = new Correction();
            }
        }

        public void Reset()
        {
            _size             = default;
            _iterations       = default;
            _minCorrection    = default;
            _maxCorrection    = default;
            _targetCorrection = default;
        }

        public Vector3 ResolveCorrection()
        {
	        switch (Size)
	        {
		        case 1:
			        return GetCorrection(0, out Vector3 direction);
		        case 2:
		        {
			        GetCorrection(0, out Vector3 direction0);
			        GetCorrection(1, out Vector3 direction1);
			        return Vector3.Dot(direction0, direction1) >= 0.0f ? CalculateMinMax() : CalculateBinary();
		        }
		        case > 2:
			        return CalculateGradientDescent(12, 0.0001f);
	        }
	        return Vector3.zero;
        }

        public void AddCorrection(Vector3 direction, float distance)
        {
            Correction correction = _corrections[_size];
            correction.Amount = direction * distance;
            correction.Direction = direction;
            correction.Distance = distance;

            _minCorrection = Vector3.Min(_minCorrection, correction.Amount);
            _maxCorrection = Vector3.Max(_maxCorrection, correction.Amount);

            ++_size;
        }

        public Vector3 GetCorrection(int index, out Vector3 direction)
        {
	        Correction correction = _corrections[index];
	        direction = correction.Direction;
	        return correction.Amount;
        }

        public Vector3 CalculateMinMax()
        {
            _iterations = default;
            _targetCorrection = _minCorrection + _maxCorrection;
            return _targetCorrection;
        }

        public Vector3 CalculateBinary()
        {
            if (_size != 2)
                throw new InvalidOperationException("Size must be 2!");
            _iterations = default;
            _targetCorrection = _minCorrection + _maxCorrection;
            Correction correction0 = _corrections[0];
            Correction correction1 = _corrections[1];

            float correctionDot = Vector3.Dot(correction0.Direction, correction1.Direction);
            if (correctionDot > 0.999f || correctionDot < -0.999f)
                return _targetCorrection;
            Vector3 deltaCorrectionDirection = Vector3.Cross(Vector3.Cross(correction0.Direction, correction1.Direction), correction0.Direction).normalized;
            float   deltaCorrectionDistance  = (correction1.Distance - correction0.Distance * correctionDot) / Mathf.Sqrt(1.0f - correctionDot * correctionDot);

            _targetCorrection = correction0.Amount + deltaCorrectionDirection * deltaCorrectionDistance;
            return _targetCorrection;
        }

        public Vector3 CalculateGradientDescent(int maxIterations, float maxError)
		{
			_iterations       = default;
			_targetCorrection = _minCorrection + _maxCorrection;

			if (_size <= 1)
				return _targetCorrection;

			Vector3      error;
			float        errorDot;
			float        errorCorrection;
			float        errorCorrectionSize;
			Vector3      desiredCorrection = _targetCorrection;
			Correction[] corrections = _corrections;
			Correction   correction;

			while (_iterations < maxIterations)
			{
				error               = default;
				errorCorrection     = default;
				errorCorrectionSize = default;

				for (int i = 0, count = _size; i < count; ++i)
				{
					correction = corrections[i];

					// Calculate error of desired correction relative to single correction.
					correction.Error = correction.Direction.x * desiredCorrection.x + correction.Direction.y * desiredCorrection.y + correction.Direction.z * desiredCorrection.z - correction.Distance;

					// Accumulate error of all corrections.
					error += correction.Direction * correction.Error;
				}

				// The accumulated error is almost zero which means we hit a local minimum.
				if (error.IsAlmostZero(maxError) == true)
					break;

				// Normalize the error => now we know what is the wrong direction => desired correction needs to move in opposite direction to lower the error.
				error.Normalize();

				for (int i = 0, count = _size; i < count; ++i)
				{
					correction = corrections[i];

					// Compare single correction direction with the accumulated error direction.
					errorDot = correction.Direction.x * error.x + correction.Direction.y * error.y + correction.Direction.z * error.z;

					// Accumulate error correction based on relative correction errors.
					// Corrections with direction aligned to accumulated error have more impact.
					errorCorrection += correction.Error * errorDot;

					if (errorDot >= 0.0f)
					{
						errorCorrectionSize += errorDot;
					}
					else
					{
						errorCorrectionSize -= errorDot;
					}
				}

				if (errorCorrectionSize < 0.000001f)
					break;

				// The error correction is almost zero and desired correction won't change.
				errorCorrection /= errorCorrectionSize;
				if (errorCorrection.IsAlmostZero(maxError) == true)
					break;

				// Move desired correction in opposite way of the accumulated error.
				desiredCorrection -= error * errorCorrection;

				++_iterations;
			}

			_targetCorrection = desiredCorrection;

			return desiredCorrection;
		}


        private sealed class Correction
        {
            public Vector3 Amount;
            public Vector3 Direction;
            public float Distance;
            public float Error;
        }
    }
    
}