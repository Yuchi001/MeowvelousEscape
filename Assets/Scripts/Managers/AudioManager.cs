using System;
using System.Collections.Generic;
using System.Linq;
using Managers.Enum;
using Managers.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviour, IManagedSingleton
    {
        [SerializeField] private List<SoundEffectData> soundEffects = new();
        [SerializeField] private Vector2 pitchDiffRange = new Vector2(-0.1f, 0.1f);
        [SerializeField][Range (0,1)] private float volumeCorrection = 1f;
        

        public static AudioManager Instance { get; private set; }

        public void Init()
        {
            if (Instance != null && Instance != this) Destroy(Instance);
            else Instance = this;
        }

        public void PlaySoundEffect(ESoundEffect soundType)
        {
            var soundEffectsData = soundEffects.FirstOrDefault(s => s.name == soundType);
            if (soundEffectsData == default) return;

            var clips = soundEffectsData.clips;
            var randomSound = clips[Random.Range(0, clips.Count)];
            
            var audioSourceObj = new GameObject($"Audio_{soundType}", typeof(AudioSource)).GetComponent<AudioSource>();
            audioSourceObj.pitch += Random.Range(pitchDiffRange.x, pitchDiffRange.y + 0.01f);
            audioSourceObj.volume = volumeCorrection;
            audioSourceObj.PlayOneShot(randomSound);
            Destroy(audioSourceObj.gameObject, randomSound.length + 0.2f);
        }

        public List<SoundEffectData> GetSoundEffects()
        {
            return soundEffects;
        }

        public void SetSoundEffects(List<SoundEffectData> soundEffects)
        {
            this.soundEffects = soundEffects;
        }
    }
    
    [System.Serializable]
    public class SoundEffectData
    {
        public List<AudioClip> clips = new();
        public ESoundEffect name;
    }
}