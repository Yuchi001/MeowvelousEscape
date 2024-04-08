using CatPackage;
using UnityEngine;

namespace AttackPackage
{
    public class BaseProjectileAttack : AttackObject
    {
        [SerializeField] private GameObject projectile;
        public override void Attack(Transform self, int damage)
        {
            var projectileObject = Instantiate(projectile, transform.position, transform.rotation);
            projectileObject.GetComponent<BasicProjectile>().Setup(damage);
        }
    }
}