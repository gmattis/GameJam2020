using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    public AudioClip[] exploreClips;
    public AudioClip[] combatClips;

    public AudioSource audioSourceMusic;
    public AudioSource audioSourceExplore;
    public AudioSource audioSourceCombat;

    private float crossfadeEnd = 0f;

    private PlayerState previousPlayerState;

    // Start is called before the first frame update
    private void Start()
    {
        if (Instance == null) { Instance = this; }

        previousPlayerState = GameManager.Instance.playerState;

        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSourceMusic = audioSources[0];
        audioSourceExplore = audioSources[1];
        audioSourceCombat = audioSources[2];

        audioSourceExplore.clip = exploreClips[LevelBuilder.Instance.levelDepth - 1];
        audioSourceCombat.clip = combatClips[LevelBuilder.Instance.levelDepth - 1];

        audioSourceExplore.Play();
        audioSourceCombat.Play();
    }

    private void Update()
    {
        if (previousPlayerState != GameManager.Instance.playerState) {
            crossfadeEnd = Time.time + 2f;
            previousPlayerState = GameManager.Instance.playerState;
        }
    }

    private void LateUpdate()
    {
        if (Time.time < crossfadeEnd)
        {
            float percent = (crossfadeEnd - Time.time) / 2f;
            if (GameManager.Instance.playerState == PlayerState.Explore)
            {
                audioSourceExplore.volume = 1f - percent;
                audioSourceCombat.volume = percent;
            }
            else if (GameManager.Instance.playerState == PlayerState.Fight)
            {
                audioSourceExplore.volume = percent;
                audioSourceCombat.volume = 1f - percent;
            }
        }
        else
        {
            if (GameManager.Instance.playerState == PlayerState.Explore)
            {
                audioSourceExplore.volume = 1f;
                audioSourceCombat.volume = 0f;
            }
            else if (GameManager.Instance.playerState == PlayerState.Fight)
            {
                audioSourceExplore.volume = 0f;
                audioSourceCombat.volume = 1f;
            }
        }
    }
}
