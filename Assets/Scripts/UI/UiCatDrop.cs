using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UiCatDrop : MonoBehaviour
    {
        [SerializeField] private Image catHeadImage;
        [SerializeField] private Image catBodyImage;
        [SerializeField] private Image tierImage;

        private UiCatDropManager _catDropManager;
        
        public void Setup(SOCat cat, Vector2 endPos, UiCatDropManager catDropManager)
        {
            _catDropManager = catDropManager;
            
            transform.localScale = Vector3.zero;
            
            var basicInfo = cat.GetDisplayInfo();
            
            catHeadImage.SetCatMaterialColors(cat);
            catBodyImage.color = basicInfo.CatColor;
            tierImage.color = GameManager.Instance.GetTierColor(basicInfo.CatTier);

            var speed = _catDropManager.SlotLifeTime();
            var tweenMove = transform.LeanMove(endPos, speed);
            var tweenScale1 = transform.LeanScale(Vector3.one, speed / 2).setEaseOutExpo().setOnComplete(() =>
            {
                var tweenScale2 = transform.LeanScale(Vector3.zero, speed / 2).setEaseInExpo().setOnComplete(() =>
                {
                    Destroy(gameObject);
                });
                tweenScale2.setOnUpdate((float val) => tweenScale2.setTime(_catDropManager.SlotLifeTime() / 2f));
            });

            tweenMove.setOnUpdate((float val) => tweenMove.setTime(_catDropManager.SlotLifeTime()));
            tweenScale1.setOnUpdate((float val) => tweenScale1.setTime(_catDropManager.SlotLifeTime() / 2f));
        }

        private void Update()
        {
            if(_catDropManager.SlotLifeTime() >= UiCatDropManager.EndOfAnim) LeanTween.pause(gameObject); 
        }
    }
}