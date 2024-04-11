using UnityEngine;

namespace AbilityPackage
{
    public abstract class Ability : MonoBehaviour
    {
        public abstract void UseAbility(Transform self, int damage);
    }
}