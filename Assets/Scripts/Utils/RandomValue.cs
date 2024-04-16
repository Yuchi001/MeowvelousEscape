using NaughtyAttributes;
using UnityEngine;

namespace Utils
{
    [System.Serializable]
    public class RandomValue
    {
        [SerializeField]
        public ParticleSystem.MinMaxCurve range;

        private bool _hasValue;

        [SerializeField, ReadOnly, AllowNesting] 
        private float value;
        public float Value
        {
            get
            {
                if (_hasValue) return value;
                value = Random.Range(range.constantMin, range.constantMax);
                _hasValue = true;
                return value;
            }
        }
    }
}