using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TrackSelect : MonoBehaviour
{

    [SerializeField] private Material gold;
    [SerializeField] private Material black;

    public GameObject[] stars;

    public int[] starAmount = { 2, 3, 1 };

    public GameObject[] tracks;

    public int selectedTrack = 0;

    public TMP_Text trackName;

    public string[] names = { "Test Track", "Silver City Concept", "A Basic Circle" };

    void Start()
    {
        tracks[selectedTrack].SetActive(true);
        trackName.text = names[selectedTrack];
    }

    public void nextTrack()
    {
        tracks[selectedTrack].SetActive(false);
        selectedTrack = (selectedTrack + 1) % tracks.Length;
        tracks[selectedTrack].SetActive(true);
        trackName.text = names[selectedTrack];
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < starAmount[selectedTrack])
            {
                stars[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                stars[i].GetComponent<MeshRenderer>().material = black;
            }
        }
    }

    public void previousTrack()
    {
        tracks[selectedTrack].SetActive(false);
        selectedTrack--;
        if (selectedTrack < 0)
        {
            selectedTrack += tracks.Length;
        }
        tracks[selectedTrack].SetActive(true);
        trackName.text = names[selectedTrack];
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < starAmount[selectedTrack])
            {
                stars[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                stars[i].GetComponent<MeshRenderer>().material = black;
            }
        }
    }

    public void select()
    {
        PlayerPrefs.SetInt("selectedTrack", selectedTrack);
    }
}
