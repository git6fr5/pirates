/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* --- Definitions --- */

/// <summary>
///
/// </summary>
public class SoundController : MonoBehaviour {

    public static SoundController Instance;

    public AudioSource audioSourceA;
    public AudioSource audioSourceB;

    public static AudioClip BoardReset;
    public AudioClip boardReset;

    public static AudioClip BoardGenerate;
    public AudioClip boardGenerate;

    public static AudioClip MapOpen;
    public AudioClip mapOpen;

    public static AudioClip MapClose;
    public AudioClip mapClose;

    public static AudioClip PieceLandSound;
    public AudioClip pieceLandSound;

    public static AudioClip Walking1;
    public AudioClip characterWalking1;

    public static AudioClip Walking2;
    public AudioClip characterWalking2;

    public static AudioClip LoseSound1;
    public AudioClip loseSound1;

    public static AudioClip LoseSound2;
    public AudioClip loseSound2;

    // Start is called before the first frame update
    void Start() {
        Init();
    }

    private void Init() {
        Instance = this;

        BoardReset = boardReset;
        BoardGenerate = boardGenerate;
        MapOpen = mapOpen;
        MapClose = mapClose;
        PieceLandSound = pieceLandSound;
        Walking1 = characterWalking1;
        Walking2 = characterWalking2;
        LoseSound1 = loseSound1;
        LoseSound2 = loseSound2;
    }

    // Update is called once per frame
    public static void PlaySound(AudioClip audioClip, int audioSourceIndex = 0) {
        Instance._PlaySound(audioClip, audioSourceIndex);
    }

    public void _PlaySound(AudioClip audioClip, int audioSourceIndex) {
        AudioSource audioSource = audioSourceA;
        if (audioSourceIndex == 1) {
            audioSource = audioSourceB;
        }

        if (audioSource.clip == audioClip && audioSource.isPlaying) {
            return;
        }
        audioSource.clip = audioClip;
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        audioSource.Play();
    }

}
