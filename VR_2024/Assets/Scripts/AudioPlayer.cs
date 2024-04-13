using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioPlayer : MonoBehaviour
{
    public AudioShotManager audioShotManager;
    
    public AudioMixerGroup mixerGroup;
    private AudioSource audioSource;
    private UnityEvent _onComplete;
    private Coroutine _waitForEndCoroutine, _waitForStartCoroutine;
    private WaitForSeconds _clipLength, _delay;
    private int _shotIndex;
    
    private void Awake()
    {
        ResetAllAudioShots();
    }

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.outputAudioMixerGroup = mixerGroup;
        
        PlayAwakeAudio();
    }

    void ConfigureAudioSource(int priority, float volume, float pitch)
    {
        StopAudio();
        audioSource.priority = priority;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }
    
    private void PlayAwakeAudio()
    {
        foreach (var audioShot in audioShotManager.audioShots)
        {
            if (!audioShot.playOnAwake) continue;
            ConfigureAudioSource(audioShot.priority, audioShot.volume, audioShot.pitch);
            audioSource.PlayOneShot(audioShot.clip);
        }
    }

    public void PlayAudioShot(string id)
    {
        _shotIndex = audioShotManager.audioShots.FindIndex(shot => shot.id == id);
        PlayAudioShot(_shotIndex);
    }
    
    public void PlayAudioShot(int index)
    {
        if (audioShotManager == null || audioSource == null) return;
        _shotIndex = index;
        var audioShot = audioShotManager.audioShots[index];
        if (audioShot.clip == null || audioShot.played) return;
        ConfigureAudioSource(audioShot.priority, audioShot.volume, audioShot.pitch);
        if (audioShot.delay > 0)
        {
            _delay = new WaitForSeconds(audioShot.delay);
            _waitForStartCoroutine ??= StartCoroutine(WaitForStartDelay(_delay, index));
        }
        else
        {
            audioShotManager.PlayAudio(audioSource, index);
        }
        if (audioShot.onComplete != null)
        {
            _onComplete = audioShot.onComplete;
            _clipLength = new WaitForSeconds(audioShot.clip.length + audioShot.delay);
            _waitForEndCoroutine ??= StartCoroutine(WaitForClipEnd(_clipLength));
        }
    }

    private IEnumerator WaitForStartDelay(WaitForSeconds delay, int index)
    {
        yield return delay;
        _waitForStartCoroutine = null;
        audioShotManager.PlayAudio(audioSource, index);
    }

    private IEnumerator WaitForClipEnd(WaitForSeconds clipLength)
    {
        yield return clipLength;
        _onComplete?.Invoke();
        _waitForEndCoroutine = null;
    }
    
    public void StopAudio()
    {
        if (audioSource != null) audioSource.Stop();
        if (_waitForStartCoroutine != null)
        {
            StopCoroutine(_waitForStartCoroutine);
            _waitForStartCoroutine = null;
            audioShotManager.audioShots[_shotIndex].played = true;
        }

        if (_waitForEndCoroutine == null) return;
        StopCoroutine(_waitForEndCoroutine);
        _onComplete?.Invoke();
        _waitForEndCoroutine = null;
    }
    
    public void PauseAudio()
    {
        if (audioSource != null) audioSource.Pause();
    }

    public void ResetAllAudioShots()
    {
        if (audioShotManager != null) audioShotManager.ResetAllAudioShots();
    }

    public void ResetAudioShot(string id)
    {
        if (audioShotManager != null) audioShotManager.ResetAudioShot(id);
    }

    public void ResetAudioShot(int index)
    {
        if (audioShotManager != null) audioShotManager.ResetAudioShot(index);
    }

    public void OnDestroy()
    {
        StopAudio();
        ResetAllAudioShots();
    }
}