using UnityEngine;

public class AudioFade : MonoBehaviour {

	public float fadeDuration = 3f;

    float maxVolume;
    AudioSource audioSource;
    float startTime;

    float fadeTime;
    bool fadingOut;
    bool fadingIn;


    void Start() {
        audioSource = GetComponent<AudioSource>();
        maxVolume = audioSource.volume;
        audioSource.volume = 0;
		FadeIn(fadeDuration);
    }

    public void FadeIn(float time) {
        fadingOut = false;
        fadingIn = true;
        fadeTime = time;
        startTime = Time.time;
    }

    public void FadeOut(float time) {
        fadingOut = true;
        fadingIn = false;
        fadeTime = time;
        startTime = Time.time;
    }

    void Update() {
        if (fadingIn) {
            if (audioSource.volume < maxVolume) {
                audioSource.volume = ((Time.time-startTime) / fadeTime) * maxVolume;
            } else {
                fadingIn = false;
            }
        } else if (fadingOut) {
            if (audioSource.volume > 0) {
                audioSource.volume = maxVolume - (((Time.time-startTime) / fadeTime) * maxVolume);
            } else {
                fadingOut = false;
            }
        }
    }
}
