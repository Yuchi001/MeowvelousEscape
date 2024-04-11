using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers.Enum;
using Managers.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Attributes;

namespace Managers
{
    public class GameManager : MonoBehaviour, IManagedSingleton
    {
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
        public void Init()
        {
            Instance = this;
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
            Debug.Log(loadSceneAsync.isDone);
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