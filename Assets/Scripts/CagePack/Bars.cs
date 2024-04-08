using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace.CagePack
{
    public class Bars : MonoBehaviour
    {
        [SerializeField] private AnimationCurve flyCurve;
        [SerializeField] private Vector2 yRange;
        [SerializeField] private Vector2 xRange;
        [SerializeField] private Vector2 rotationSpeed;
        [SerializeField] private float range;

        private bool _throwBars = false;
        private Vector2 _startPos;
        private float _xMod;
        private float _yMod;
        private float _rot;

        private float _timer = 0;

        public void ThrowBars()
        {
            _startPos = transform.position;
            _throwBars = true;
            _xMod = Random.Range(xRange.x, xRange.y);
            _yMod = Random.Range(yRange.x, yRange.y);
            _rot = Random.Range(rotationSpeed.x, rotationSpeed.y);
            _xMod *= Random.Range(0, 2) == 0 ? 1 : -1;
            _yMod *= Random.Range(0, 2) == 0 ? 1 : -1;
            _rot *= Random.Range(0, 2) == 0 ? 1 : -1;
        }
        
        private void Update()
        {
            if (!_throwBars) return;

            transform.Rotate(0, 0, _rot);
            transform.position += new Vector3(_xMod * Time.deltaTime, _yMod * Time.deltaTime, 0);

            if (Vector2.Distance(_startPos, transform.position) < range) return;
            
            Destroy(gameObject);
        }
    }
}