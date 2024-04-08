using System.Linq;
using UnityEngine;

namespace DefaultNamespace.TruckPack
{
    public class Truck : MonoBehaviour
    {
        [SerializeField] private float radius;

        private bool _hasEscaped = false;
        
        private void Update()
        {
            if (_hasEscaped) return;
            
            var colliders = Physics2D.OverlapCircleAll(transform.position, radius);
            if (colliders.Length == 0) return;

            if (colliders.ToList().FirstOrDefault(c => c.CompareTag("Player")) == default) return;

            Flee();
        }

        private void Flee()
        {
            _hasEscaped = true;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}