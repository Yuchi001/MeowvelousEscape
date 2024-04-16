using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace UI
{
    public class UiCatDropManager : MonoBehaviour
    {
        [SerializeField] private GameObject catDropPrefab;
        [SerializeField] private Transform catDropSpawnPosition;
        [SerializeField] private Transform catDropEndPosition;
        [SerializeField] private float startSlotLifeTime;
        [SerializeField] private float maxCatsQuantity;
        [SerializeField] private float spinTime;
        
        private float _slotLifeTime;

        private float _timer = 0;
        
        public const int EndOfAnim = 999;
        
        private List<(SOCat cat, int weight)> _catTuples = new();

        private void Awake()
        {
            var allCats = GameManager.Instance.allCats;
            var unlockedCats = GameManager.Instance.SaveFile.UnlockedCats.List;

            foreach (var cat in allCats)
            {
                var catDisplayInfo = cat.GetDisplayInfo();

                var unlockedCat = unlockedCats.FirstOrDefault(c => c.CatName == catDisplayInfo.CatName);

                var weight = (int)Mathf.Pow(Mathf.Abs((int)catDisplayInfo.CatTier - 5), 3);
                weight = GetDetailedWeight(weight, unlockedCat?.DropAmount ?? 0);
                _catTuples.Add((cat, weight));
            }

            _catTuples.Sort((a, b) => a.weight - b.weight);
        }

        public void StartSpin()
        {
            _slotLifeTime = startSlotLifeTime;
            foreach (var catDropSlot in FindObjectsOfType<UiCatDrop>())
            {
                LeanTween.pause(catDropSlot.gameObject);
                Destroy(catDropSlot.gameObject);
            }
            LeanTween.value(gameObject, startSlotLifeTime, 10, spinTime).setOnUpdate((float val) =>
            {
                _slotLifeTime = val;
            }).setEaseInSine().setOnComplete(() => _slotLifeTime = EndOfAnim);
            StartCoroutine(EnumStartSpin());
        }

        private IEnumerator EnumStartSpin()
        {
            while (_slotLifeTime < 999)
            {
                SpawnCatDrop();
                yield return new WaitForSeconds(1 / (maxCatsQuantity / _slotLifeTime));
            }
        } 
        
        public float SlotLifeTime()
        {
            return _slotLifeTime;
        }
        
        private void SpawnCatDrop()
        {
            var catDropInstance = Instantiate(catDropPrefab, catDropSpawnPosition.position, Quaternion.identity, transform);
            catDropInstance.transform.SetAsFirstSibling();
            var catDropScript = catDropInstance.GetComponent<UiCatDrop>();
            catDropScript.Setup(GetRandomCat(), catDropEndPosition.position, this);
        }

        private SOCat GetRandomCat()
        {
            var weightSum = _catTuples.Sum(tuple => tuple.weight);
            var randomNumber = Random.Range(0, weightSum + 1);
            foreach (var catTuple in _catTuples)
            {
                if (randomNumber <= catTuple.weight) return catTuple.cat;
                randomNumber -= catTuple.weight;
            }

            return _catTuples[^1].cat;
        }

        private static int GetDetailedWeight(int weight, int timesDropped)
        {
            for (var i = 0; i < weight; i++)
            {
                weight = Mathf.CeilToInt(weight * 0.75f);
            }
            return weight;
        }
    }
}