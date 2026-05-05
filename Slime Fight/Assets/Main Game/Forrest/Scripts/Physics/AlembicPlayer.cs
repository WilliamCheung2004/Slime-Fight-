using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

[RequireComponent(typeof(AlembicStreamPlayer))]
public class AlembicAutoPlay : MonoBehaviour
{
    public float speed = 1f;
    public bool loop = true;
    public bool playOnEnable = false; 

    private AlembicStreamPlayer player;
    private bool isPlayingOnce;
    private bool isPlaying;

    void Awake()
    {
        player = GetComponent<AlembicStreamPlayer>();
    }

    void OnEnable()
    {
        // start playing only if explicitly requested
        if (playOnEnable && player != null)
        {
            player.CurrentTime = 0f;
            isPlaying = true;
        }
        else
        {
            isPlaying = false;
        }
    }

    void Update()
    {
        if (player == null) return;
        if (!isPlaying) return; 

        if (isPlayingOnce)
        {
            player.CurrentTime += Time.deltaTime * speed;

            if (player.CurrentTime >= player.Duration)
            {
                isPlayingOnce = false;
                isPlaying = false; 
            }

            return;
        }

        // continuous playback (looping) while isPlaying==true and not in single-play mode
        player.CurrentTime += Time.deltaTime * speed;

        if (loop && player.CurrentTime >= player.Duration)
            player.CurrentTime = 0f;
    }

    // Play the Alembic once from the start (non-looping).
    public void PlayOnce()
    {
        if (player == null) return;

        isPlayingOnce = true;
        isPlaying = true;
        player.CurrentTime = 0f;
    }

    // Optional helper to start continuous playback (respects 'loop')
    public void Play()
    {
        if (player == null) return;

        isPlaying = true;
        isPlayingOnce = false;
        if (player.CurrentTime >= player.Duration)
            player.CurrentTime = 0f;
    }

    public void Stop()
    {
        isPlaying = false;
        isPlayingOnce = false;
    }
}