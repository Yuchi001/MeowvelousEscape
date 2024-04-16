using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers.Enum;
using Managers.Interfaces;
using Managers.SavingProgress;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Attributes;

namespace Managers
{
    public class GameManager : MonoBehaviour, IManagedSingleton
    {
        #region Singleton

        [Help("Add objects to Non Singleton List only if they doesnt implement IManagedSingleton interface but still should live thru scenes.")]
        [SerializeField] private ESceneName startingScene;
        
        [Space]
        [SerializeField] private List<GameObject> nonSingletonObjects;

        private readonly List<IManagedSingleton> _singletons = new();
        
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Destroy(gameObject);
                Debug.LogError("There was more than one gameManager instance!");
                return;
            }
            
            Init();            
        }

        #endregion

        [SerializeField] private SOCat basicCat;

        private const string SaveFilePath = "/Saves/saveFile.*";
        
        private JsonDataService _jsonDataService;
        
        public List<SOCat> allCats { get; private set; }

        public SaveFile SaveFile
        {
            get
            {
                if (_saveFile != null) return _saveFile;

                if (!_jsonDataService.FileExists(SaveFilePath))
                    _jsonDataService.SaveData(SaveFilePath, new SaveFile(basicCat), true);
                
                _saveFile = _jsonDataService.LoadData<SaveFile>(SaveFilePath, true);
                return _saveFile;
            }
        }
        private SaveFile _saveFile = null;

        public List<SOCat> AvailableCats
        {
            get
            {
                if (_availableCats != null) return _availableCats;
                
                _availableCats = SaveFile.UnlockedCats.List
                    .Select(s => 
                        allCats.FirstOrDefault(c => c.GetDisplayInfo().CatName == s.CatName))
                    .ToList();
                return _availableCats;
            }
        }
        private List<SOCat> _availableCats = null;

        public void SaveData(SaveFile data)
        {
            _jsonDataService.SaveData(SaveFilePath, data,true);
            _saveFile = _jsonDataService.LoadData<SaveFile>(SaveFilePath, true);
        }
        
        public void Init()
        {
            Instance = this;

            _jsonDataService = new JsonDataService();

            _saveFile = _jsonDataService.FileExists(SaveFilePath)
                ? _jsonDataService.LoadData<SaveFile>(SaveFilePath, true)
                : new SaveFile(basicCat);

            allCats = Resources.LoadAll<SOCat>("SoCats").ToList();
            
            // maksymalna ilość animacji na scenie
            LeanTween.init(100, 100);
            
            // set singletons to live forever! LONG LIVE THE KING
            DontDestroyOnLoad(gameObject);
            foreach (Transform child in transform)
            {
                if(nonSingletonObjects.FirstOrDefault
                       (n => n.name == child.name) != default) 
                    continue;
                
                if (!child.TryGetComponent<IManagedSingleton>(out var singleton))
                {
                    Debug.LogError($"Object {child.name} was not of type IManagedSingleton");
                    continue;
                }
                
                _singletons.Add(singleton);
            }

            StartCoroutine(PrepareScene());
        }
        private IEnumerator PrepareScene()
        {
            var loadSceneAsync = SceneManager.LoadSceneAsync((int)startingScene);
            while (!loadSceneAsync.isDone) yield return null;
            
            _singletons.ForEach(s =>
            {
                if (ReferenceEquals(s, this)) return;
                s.Init();
            });
            
            Debug.Log("INITIALIZED SCENE");
        }
    }
}