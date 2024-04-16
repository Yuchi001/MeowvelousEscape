using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace DefaultNamespace.CagePack
{
    public class Bars : MonoBehaviour
    {
        [SerializeField] private float animTime;
        public void ThrowBars()
        {
            transform.LeanRotateY(180, animTime).setEaseOutBounce();
        }
    }
}