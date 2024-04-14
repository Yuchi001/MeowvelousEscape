using System;
using Managers;
using Managers.Enum;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace UI
{
    [RequireComponent(typeof(Button))]
    public class AnimatedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float defaultScale = 1;
        [SerializeField] private float onHoverScale = 1.2f;
        [SerializeField, Min(0.01f)] private float animationTime = 0.1f;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SpawnClickSound);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(SpawnClickSound);
        }

        private void SpawnClickSound()
        {
            if(AudioManager.Instance == null) return;
            
            AudioManager.Instance.PlaySoundEffect(ESoundEffect.button_click);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            var normalDiff = onHoverScale - defaultScale;
            var currentDiff = onHoverScale - transform.localScale.x;
            var time = animationTime * (currentDiff / normalDiff);
            
            transform.LeanScaleX(onHoverScale, time);
            transform.LeanScaleY(onHoverScale, time);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            var normalDiff = Mathf.Abs(defaultScale - onHoverScale);
            var currentDiff = Mathf.Abs(defaultScale - transform.localScale.x);
            var time = animationTime * (currentDiff / normalDiff);
            
            transform.LeanScaleX(defaultScale, time);
            transform.LeanScaleY(defaultScale, time);
        }
    }
}