using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class UiSidebar : MonoBehaviour
    {
        [SerializeField] private Transform fadeInPos;
        [SerializeField] private Transform catDropFadeOutPos;
        [SerializeField] private float fadeTime;

        private Vector2 _fadeOutPos;
        
        public delegate void StartGameDelegate();
        public static event StartGameDelegate OnStartGame;
        
        private void Awake()
        {
            _fadeOutPos = transform.localPosition;
            transform.LeanMoveLocal(fadeInPos.localPosition, fadeTime).setEaseOutBounce();
        }

        public void OnPlay()
        {
            OnStartGame?.Invoke();
            gameObject.SetActive(false);
        }

        public void OnCatDrop()
        {
            transform.LeanMoveLocal(catDropFadeOutPos.localPosition, fadeTime).setEaseInBounce();
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}