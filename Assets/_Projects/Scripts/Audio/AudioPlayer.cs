using UnityEngine;
using System.Collections.Generic;

public class AudioPlayer : MonoBehaviour
{
    // シングルトン用のインスタンス
    public static AudioPlayer Instance { get; private set; } = null;

    [Header("Audio Resources")]
    [SerializeField] AudioSO audios;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource seSource;

    readonly Dictionary<string, AudioClip> audioCache = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Initialize()
    {
        // 起動時にScriptableObjectの内容を辞書に登録しておく
        foreach (AudioAsset audioAsset in audios.AudioAssets)
        {
            if (!audioCache.ContainsKey(audioAsset.Tag))
            {
                audioCache.Add(audioAsset.Tag, audioAsset.AudioClip);
            }
        }
    }

    // BGMの再生（ループあり、差し替え）
    public void PlayBGM(string tag)
    {
        AudioClip clip = GetAudioClip(tag);
        if (clip == null)
        {
            return;
        }

        // すでに同じ曲が流れている場合は何もしない
        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    // SEの再生（重ねて再生可能）
    public void PlaySE(string tag, AudioSource audioSource = null)
    {
        AudioClip clip = GetAudioClip(tag);
        if (clip == null)
        {
            return;
        }

        // 第2引数が渡されているかどうかで、再生するAudioSourceを切替
        AudioSource targetAudioSource = audioSource == null ? seSource : audioSource;
        targetAudioSource.PlayOneShot(clip);
    }

    // 辞書から素早くClipを取り出す
    AudioClip GetAudioClip(string tag)
    {
        if (audioCache.TryGetValue(tag, out AudioClip clip))
        {
            return clip;
        }
        Debug.LogWarning($"Tag '{tag}' は見つかりませんでした。");
        return null;
    }
}
