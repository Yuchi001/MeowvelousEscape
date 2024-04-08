using System;
using System.Linq;
using Managers.Enum;
using UnityEditor;
using UnityEngine;

namespace Managers.Editor
{
    [CustomEditor(typeof(AudioManager))]
    public class CustomAudioManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty soundEffectsField;
        private SerializedProperty pitchDiffRangeField;
        private SerializedProperty volumeCorrectionField;

        private AudioManager _audioManagerScript;
        private void OnEnable()
        {
            pitchDiffRangeField = serializedObject.FindProperty("pitchDiffRange");
            volumeCorrectionField = serializedObject.FindProperty("volumeCorrection");
            soundEffectsField = serializedObject.FindProperty("soundEffects");
            
            _audioManagerScript = target as AudioManager;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(soundEffectsField);
            EditorGUILayout.PropertyField(pitchDiffRangeField);
            EditorGUILayout.PropertyField(volumeCorrectionField);
            serializedObject.ApplyModifiedProperties();
            
            if (!GUILayout.Button("Generate audio files")) return;

            var audioClips = Resources.LoadAll<AudioClip>("SoundEffects").ToList();
            foreach (var soundType in (ESoundEffect[])System.Enum.GetValues(typeof(ESoundEffect)))
            {
                //var clips = audioClips.Where(c => c.name.ToLower().Contains(soundType.ToString().ToLower()));
                var clips = audioClips.Where(c => c.name.ToLower().StartsWith(soundType.ToString().ToLower()));
                Debug.Log(audioClips.ToList().Count);
                var soundEffects = _audioManagerScript.GetSoundEffects();
                var soundEffectData = soundEffects.FirstOrDefault(s => s.name == soundType);
                if (soundEffectData != default)
                {
                    soundEffectData.clips = clips.ToList();
                    _audioManagerScript.SetSoundEffects(soundEffects);
                    continue;   
                }
                soundEffects.Add(new SoundEffectData
                {
                    clips = clips.ToList(),
                    name = soundType
                });
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}