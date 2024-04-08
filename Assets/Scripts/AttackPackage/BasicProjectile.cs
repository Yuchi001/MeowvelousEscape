using System;
using UnityEngine;

namespace AttackPackage
{
    public class BasicProjectile : MonoBehaviour
    {
        [SerializeField] protected GameObject flyParticles;
        [SerializeField] protected GameObject destroyParticles;
        [SerializeField] protected float spinSpeed = 0;
        [SerializeField] protected float range;
        [SerializeField] protected float speed;
        
        private int _damage;
        private Vector2 _startPos;
        
        public void Setup(int damage)
        {
            _startPos = transform.position;
            _damage = damage;
            if (flyParticles == null) return;
            Instantiate(flyParticles, transform.position, transform.rotation, transform);
        }

        private void FixedUpdate()
        {
            transform.position += transform.up * speed * Time.deltaTime;
            if (spinSpeed > 0)
            {
                transform.Rotate(0, 0, spinSpeed * Time.deltaTime, Space.Self);
            }
            if (Vector2.Distance(transform.position, _startPos) >= range)
            {
                DestroyProjectile();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Wall"))
            {
                DestroyProjectile();
                return;
            }
            
            if (!other.CompareTag("Enemy")) return;

            //todo: implementacja dostawania obrażeń przez przeciwnika
        }

        private void DestroyProjectile()
        {
            if (destroyParticles != null)
            {
                var particles = Instantiate(destroyParticles, transform.position, Quaternion.identity);
                Destroy(particles, 2);
            }

            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent<ParticleSystem>(out var ps)) continue;
                ps.transform.parent = null;
                ps.Stop(true);
                Destroy(ps.gameObject, 2f);
                break;
            }
            Destroy(gameObject);
        }
    }
}