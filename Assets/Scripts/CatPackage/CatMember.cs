using System.Collections;
using System.Linq;
using Managers;
using NaughtyAttributes;
using UnityEngine;

namespace CatPackage
{
    public class CatMember : MonoBehaviour
    {
        public event CatDamageEventHandler OnCatDamaged;

        [SerializeField] private Transform shootPos;

        [SerializeField, ReadOnly]
        private ActiveCatData _activeCat;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        private float _timer = 0;
        private KeyCode _attackKey;

        public int CurrentHealth => _activeCat.health;

        private Color catColor;

        public void SetCat(ActiveCatData activeCatData, KeyCode attackKey)
        {
            _attackKey = attackKey;
            _activeCat = activeCatData;
            gameObject.SetActive(true);
            GetComponentInParent<SpriteRenderer>().color = catColor =
                PlayerManager.Instance.KeyColors.FirstOrDefault(c => c.key == _attackKey).color;
        }

        public ActiveCatData GetCat()
        {
            return _activeCat;
        }

        public void LevelUp()
        {
            _activeCat.level++;
        }

        public void TakeDamage(int damage)
        {
            _activeCat.health -= damage;
            if (spriteRenderer)
                StartCoroutine(AnimateDamageCo());
    
            OnCatDamaged?.Invoke(this, damage);
        }

        public static readonly Color DamagedColor = Color.red;

        private IEnumerator AnimateDamageCo()
        {
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = DamagedColor;;
            for (float progress = 0;  progress < 1; progress += Time.deltaTime * 3)
            {
                yield return null;
                spriteRenderer.color = Color.Lerp(DamagedColor, catColor, progress);
            }
            spriteRenderer.color = catColor;
        }

        private void Update()
        {
            if (_activeCat == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _timer += Time.deltaTime;
            if (!Input.GetKey(_attackKey) || _timer < _activeCat.cat.GetSpecificInfo(_activeCat.level).cooldown) return;

            _timer = 0;
            _activeCat.cat.SpawnAttackPrefab(shootPos.position, transform, _activeCat.level);
        }
    }
}
