using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.CagePack;
using Managers;
using Managers.SavingProgress;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class UiCage : MonoBehaviour
    {
        [SerializeField] private GameObject title;
        [SerializeField] private float cameraAnimTime;
        [SerializeField] private float catHeadRemoveAnimTime;
        [SerializeField] private GameObject bars;
        [SerializeField] private SpriteRenderer catBody;
        [SerializeField] private SpriteRenderer catHead;
        [SerializeField] private GameObject leftArrow;
        [SerializeField] private GameObject rightArrow;
        [SerializeField] private UiCatInfo uiCatInfo;

        private int _currentIndex = 0;
        
        private List<SOCat> _availableCats = new();

        private SaveFile _saveFile;

        private void Awake()
        {
            //MainCameraManager.OnCameraReady += OpenCage;
            UiSidebar.OnStartGame += StartGame;
            
            _saveFile = GameManager.Instance.SaveFile;
            _availableCats = GameManager.Instance.AvailableCats;
            
            SOCat pickedCat = null;
            for (var i = 0; i < _availableCats.Count; i++)
            {
                if (_availableCats[i].GetDisplayInfo().CatName != _saveFile.PickedCat.CatName)
                    continue;
                _currentIndex = i;
                pickedCat = _availableCats[i];
            }
            
            if (pickedCat == null) return;
            SetCat(pickedCat);
        }

        private void OnDisable()
        {
            //MainCameraManager.OnCameraReady -= OpenCage;
            UiSidebar.OnStartGame -= StartGame;
        }

        private void StartGame()
        {
            LeanTween.color(catHead.gameObject, Color.clear, catHeadRemoveAnimTime);
            Camera.main.transform.LeanMove(catBody.transform.position, cameraAnimTime);
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            title.SetActive(false);
            OpenCage();
        }

        private void OpenCage()
        {
            bars.GetComponent<Bars>().ThrowBars();
            PlayerManager.Instance.StartRun(_availableCats[_currentIndex], catBody.transform.position);
            Destroy(catBody.gameObject);
        }


        public void OnArrowClick(int direction)
        {
            _currentIndex = Mathf.Clamp(_currentIndex + direction, 0, _availableCats.Count - 1);
            SetCat(_availableCats[_currentIndex]);
        }

        private void SetCat(SOCat cat)
        {
            leftArrow.SetActive(_currentIndex > 0);
            rightArrow.SetActive(_currentIndex + 1 < _availableCats.Count);
            uiCatInfo.SetCatInfo(cat);
            catHead.SetCatMaterialColors(cat);
            catBody.color = cat.GetDisplayInfo().CatColor;
        }
    }
}