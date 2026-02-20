using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AudioSO", menuName = "Scriptable Objects/AudioSO")]
public class AudioSO : ScriptableObject
{
    // オーディオクリップをタグと紐付けてリストに格納
    [field:SerializeField] public List<AudioAsset> AudioAssets { get; private set; } = new();
}

[System.Serializable]
public struct AudioAsset
{
    [field:SerializeField] public string Tag { get; private set; }          // 再生時に検索するためのタグ(名前)
    [field:SerializeField] public AudioClip AudioClip { get; private set; } // オーディオクリップ
}