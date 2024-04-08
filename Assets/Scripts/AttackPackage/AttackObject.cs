using UnityEngine;

namespace CatPackage
{
    public abstract class AttackObject : MonoBehaviour
    {
        public abstract void Attack(Transform self, int damage);
    }
}