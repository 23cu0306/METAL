using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("BGM�ݒ�")]
    public AudioSource bgmSource;
    public float bgmFadeDuration = 1.0f;

    [Header("���ʉ��ݒ�")]
    public GameObject tempAudioPrefab; // ���GameObject��AudioSource�����v���n�u�ioptional�j

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // BGM AudioSource���Ȃ��ꍇ��������
            if (bgmSource == null)
            {
                GameObject bgmObj = new GameObject("BGM Source");
                bgmObj.transform.SetParent(transform);
                bgmSource = bgmObj.AddComponent<AudioSource>();
                bgmSource.loop = true;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���ʉ����Đ��i�ʒu�w��j
    /// </summary>
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.volume = volume;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }

    /// <summary>
    /// BGM �𑦎��Đ��i�t�F�[�h�Ȃ��j
    /// </summary>
    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    /// <summary>
    /// BGM���t�F�[�h�C���ōĐ�
    /// </summary>
    public void PlayBGMWithFade(AudioClip clip, float targetVolume = 1f)
    {
        if (clip == null) return;
        StartCoroutine(FadeInBGM(clip, targetVolume));
    }

    /// <summary>
    /// ���݂�BGM���t�F�[�h�A�E�g���Ē�~
    /// </summary>
    public void StopBGMWithFade()
    {
        StartCoroutine(FadeOutBGM());
    }

    /// <summary>
    /// BGM�t�F�[�h�C��
    /// </summary>
    IEnumerator FadeInBGM(AudioClip clip, float targetVolume)
    {
        if (bgmSource.isPlaying)
        {
            yield return FadeOutBGM();
        }

        bgmSource.clip = clip;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float t = 0;
        while (t < bgmFadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0, targetVolume, t / bgmFadeDuration);
            yield return null;
        }

        bgmSource.volume = targetVolume;
    }

    /// <summary>
    /// BGM�t�F�[�h�A�E�g
    /// </summary>
    IEnumerator FadeOutBGM()
    {
        float startVolume = bgmSource.volume;
        float t = 0;
        while (t < bgmFadeDuration)
        {
            t += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / bgmFadeDuration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = null;
        bgmSource.volume = startVolume;
    }
}
