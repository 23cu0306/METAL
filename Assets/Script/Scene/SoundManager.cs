using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも保持
        }
        else
        {
            Destroy(gameObject); // 複数存在しないように
        }
    }

    /// <summary>
    /// 一時的にオーディオを再生する
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
}
