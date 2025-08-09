using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private AudioSource _audioSource;
    private float _songsPlayed;
    private bool[] _beenPlayed;
    public AudioClip[] songs;
    private bool stopped;

    private void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _beenPlayed = new bool[songs.Length];

        if (!_audioSource.isPlaying)
        {
            changeSong(Random.Range(0, songs.Length));
        }

        stopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_audioSource.isPlaying && !stopped)
        {
            changeSong(Random.Range(0, songs.Length));
        }
        
        ResetShuffle();
    }

    public void changeSong(int songPicked)
    {
        if (!_beenPlayed[songPicked])
        {
            _songsPlayed++;
            _beenPlayed[songPicked] = true;
            _audioSource.clip = songs[songPicked];
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop();
        }
    }

    public void StopMusic()
    {
        _audioSource.Stop();
        stopped = true;
    }

    private void ResetShuffle()
    {
        if (_songsPlayed == songs.Length)
        {
            _songsPlayed = 0;
            for (int i = 0; i < songs.Length; i++)
            {
                if (i == songs.Length)
                {
                    break;
                }
                else
                {
                    _beenPlayed[i] = false;
                }
            }
        }
    }
}
