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

        private float _fadeOutPosX;
        private void Awake()
        {
            _fadeOutPosX = transform.localPosition.x;
            transform.LeanMoveLocalX(fadeInPos.localPosition.x, fadeTime).setEaseOutBounce();
        }

        public void OnPlay()
        {
            transform.LeanMoveLocalX(_fadeOutPosX, fadeTime).setEaseInBounce();
        }

        public void OnCatDrop()
        {
            transform.LeanMoveLocalX(catDropFadeOutPos.localPosition.x, fadeTime).setEaseInBounce();
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}