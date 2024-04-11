using System;
using Managers;
using Managers.Enum;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Attributes;

namespace Utils
{
    public class StartGameFromAnywhere : MonoBehaviour
    {
        [Help("This component loads GameManager scene"), SerializeField, ReadOnly]
        private ESceneName startScene = ESceneName.GameManager;
        
        private void Awake()
        {
            if (GameManager.Instance != null) return;

            SceneManager.LoadScene((int)startScene);
        }
    }
}