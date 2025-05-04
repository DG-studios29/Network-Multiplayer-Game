using UnityEngine;

public class PirateMusicSettings : MonoBehaviour
{
    #region Custom Variables

    public enum MusicState { chill, battle}
    [Header("Enum"), Space(10)]
    [Tooltip("is the game state chill or intense?")]public MusicState state;

    [Header("AudioSources"), Space(10)]
    public AudioSource chillSource;
    public AudioSource battleSource;

    [Header("AudioClips Array"), Space(10)]
    public AudioClip[] chillClips;
    public AudioClip[] battleClips;

    [Header("Floats"), Space(10)]
    [Range(0.05f,.1f)] public float maxVolume;
    private const float minVolume = .01f;

    [Range(0.9f, 1)] public float maxPitch;
    private const float minPitch = .7f;

    #endregion

    #region Built-In Methods

    void Start()
    {
        if (chillSource)
        {
            chillSource.playOnAwake = false;
            chillSource.loop = false;

            chillSource.volume = minVolume;
            chillSource.pitch = minPitch;

            chillSource.clip = chillClips[GetRandomClipIndex(chillClips)];
            chillSource.Play();
            chillSource.time = Random.Range(0, chillClips[GetRandomClipIndex(chillClips)].length);
        }

        if(battleSource)
        {
            battleSource.playOnAwake = false;
            battleSource.loop = false;

            battleSource.pitch = minPitch;
            battleSource.volume = minVolume;
        }
    }

    void Update()
    {
        SetMusic();
    }

    #endregion

    #region Custom Methods

    private int GetRandomClipIndex(AudioClip[] clips)
    {
        return Random.Range(0, clips.Length);
    }

    private void SetMusic()
    {
        if (state == MusicState.chill)
        {
            if (chillSource != null && chillClips.Length > 0)
            {
                if (!chillSource.isPlaying || chillSource.time >= chillSource.clip.length - 0.2f)
                {
                    int clipIndex = GetRandomClipIndex(chillClips);
                    chillSource.clip = chillClips[clipIndex];
                    chillSource.Play();
                    chillSource.time = Random.Range(0, chillClips[clipIndex].length);
                }

                chillSource.volume = Mathf.Lerp(chillSource.volume, maxVolume, Time.deltaTime * 0.2f);
                chillSource.pitch = Mathf.Lerp(chillSource.pitch, maxPitch, Time.deltaTime * .2f);
            }

            if (battleSource) battleSource.volume = Mathf.Lerp(battleSource.volume, 0f, Time.deltaTime * .5f);
            battleSource.pitch = Mathf.Lerp(battleSource.pitch, minPitch, Time.deltaTime * 1f);
        }

        if (state == MusicState.battle)
        {
            if (battleSource != null && battleClips.Length > 0)
            {
                if (!battleSource.isPlaying || battleSource.time >= battleSource.clip.length - 0.2f)
                {
                    int clipIndex = GetRandomClipIndex(battleClips);
                    battleSource.clip = battleClips[clipIndex];
                    battleSource.Play();
                    battleSource.time = Random.Range(10f, battleClips[clipIndex].length);
                }
                battleSource.volume = Mathf.Lerp(battleSource.volume, maxVolume, Time.deltaTime * .2f);
                battleSource.pitch = Mathf.Lerp(battleSource.pitch, maxPitch, Time.deltaTime * .2f);
            }

            if (chillSource) chillSource.volume = Mathf.Lerp(chillSource.volume, minVolume, Time.deltaTime * .5f);
            chillSource.pitch = Mathf.Lerp(chillSource.pitch, minPitch, Time.deltaTime * .2f);
        }
    }

    #endregion
}
