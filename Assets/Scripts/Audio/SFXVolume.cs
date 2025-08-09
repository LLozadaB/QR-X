using UnityEngine;
using UnityEngine.Audio;
public class SFXVolume : MonoBehaviour
{

    public AudioMixer sfxMixer;
    
    public void setSFXVolume(float volume)
    {
        sfxMixer.SetFloat("sfxVolume", volume);
    }
}
