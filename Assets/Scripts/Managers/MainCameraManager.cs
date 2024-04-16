using System;
using UI;
using UnityEngine;

namespace Managers
{
    public class MainCameraManager : MonoBehaviour
    {
        [SerializeField] private float cameraStartSize;
        [SerializeField] private float cameraPlaySize;
        [SerializeField] private float animationPlayTime;
        [SerializeField] private GameObject lightObject;
        [SerializeField] private float rotateLightSpeed;

        public delegate void CameraReadyDelegate();
        public static event CameraReadyDelegate OnCameraReady;
        
        private void Awake()
        {
            UiSidebar.OnStartGame += OnStartGame;
            Camera.main.orthographicSize = cameraStartSize;
        }

        private void OnDisable()
        {
            UiSidebar.OnStartGame -= OnStartGame;
        }

        private void OnStartGame()
        {
            LeanTween.value(gameObject, cameraStartSize, cameraPlaySize, animationPlayTime)
                .setOnUpdate((val) =>
                {
                    Camera.main.orthographicSize = val;
                })
                .setEase(LeanTweenType.easeOutQuad).setOnComplete(() => OnCameraReady?.Invoke());
        }

        private void Update()
        {
            lightObject.transform.Rotate(0, 0, rotateLightSpeed * Time.deltaTime);
        }
    }
}