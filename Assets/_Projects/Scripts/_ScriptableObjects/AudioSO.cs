using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioSO", menuName = "Scriptable Objects/AudioSO")]
public class AudioSO : ScriptableObject
{
    [field:SerializeField] public List<AudioAsset> AudioAssets { get; private set; } = new();
}

[System.Serializable]
public struct AudioAsset
{
    [field:SerializeField] public string Tag { get; private set; }
    [field:SerializeField] public AudioClip AudioClip { get; private set; }
}