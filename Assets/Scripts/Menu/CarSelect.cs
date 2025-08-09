using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CarSelect : MonoBehaviour
{
    public GameObject[] cars;

    public int selectedCar = 0;

    public TMP_Text carName;
    public TMP_Text manufacturerName;
    public TMP_Text carDescription;
    

    [SerializeField] private Material gold;
    [SerializeField] private Material black;

    public GameObject[] speedBlocks;
    public GameObject[] accelerationBlocks;
    public GameObject[] handlingBlocks;
    public TMP_Text healthText;

    public string[] manufacturers = { "DYNAMO INDUSTRIES", "VULKAN MOTORS", "ORASSIA LLC" };

    public string[] names = { "SPEARHEAD", "GAZELLE", "D-58 \"CRUSHER\"" };

    public string[] descriptions = {
        "Often used by newcomers, the 2050 Spearhead is an average G-250 with it's gearing skewed towards top speed. It handles well enough, can take a few hits, and is at home in a straight line, but the low acceleration makes it a bit of a slog off the line and in combat recovery.",
        "The 2055 Gazelle is a lightweight, high-tech G-250 designed for quick acceleration and nimble handling. It's small engine and high acceleration gearing make it's top speed low, and it's thin construction makes it fragile, but it'll annhialate tight corners unlike anything else.",
        "The Crusher is actually 2040 G-200, with upgrades to make it a G-250. Typical with Orassia, this was made to be the last G-250 standing. It's huge engine gives immense speed, and it's primitive frame is very durable, but it's weight gives sub-par acceleration and poor handling." };

    public int[] topSpeed = { 7, 3, 8 };

    public int[] acceleration = { 3, 7, 4 };

    public int[] handling = { 5, 8, 3 };

    public int[] health = { 400, 275, 650 };

    void Start()
    {
        cars[selectedCar].SetActive(true);
        carName.text = names[selectedCar];
        manufacturerName.text = manufacturers[selectedCar];
        carDescription.text = descriptions[selectedCar];

        for (int i = 0; i < speedBlocks.Length; i++)
        {
            if (i < topSpeed[selectedCar])
            {
                speedBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                speedBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        for (int i = 0; i < accelerationBlocks.Length; i++)
        {
            if (i < acceleration[selectedCar])
            {
                accelerationBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                accelerationBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        for (int i = 0; i < handlingBlocks.Length; i++)
        {
            if (i < handling[selectedCar])
            {
                handlingBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                handlingBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        healthText.text = "" + health[selectedCar];
    }

    public void nextCar()
    {
        cars[selectedCar].SetActive(false);
        selectedCar = (selectedCar + 1) % cars.Length;
        cars[selectedCar].SetActive(true);
        carName.text = names[selectedCar];
        manufacturerName.text = manufacturers[selectedCar];
        carDescription.text = descriptions[selectedCar];

        for (int i = 0; i < speedBlocks.Length; i++)
        {
            if (i < topSpeed[selectedCar])
            {
                speedBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                speedBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        for (int i = 0; i < accelerationBlocks.Length; i++)
        {
            if (i < acceleration[selectedCar])
            {
                accelerationBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                accelerationBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        for (int i = 0; i < handlingBlocks.Length; i++)
        {
            if (i < handling[selectedCar])
            {
                handlingBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                handlingBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        healthText.text = "" + health[selectedCar];
    }

    public void previousCar()
    {
        cars[selectedCar].SetActive(false);
        selectedCar--;
        if (selectedCar < 0)
        {
            selectedCar += cars.Length;
        }
        cars[selectedCar].SetActive(true);
        carName.text = names[selectedCar];
        manufacturerName.text = manufacturers[selectedCar];
        carDescription.text = descriptions[selectedCar];

        for (int i = 0; i < speedBlocks.Length; i++)
        {
            if (i < topSpeed[selectedCar])
            {
                speedBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                speedBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        for (int i = 0; i < accelerationBlocks.Length; i++)
        {
            if (i < acceleration[selectedCar])
            {
                accelerationBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                accelerationBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        for (int i = 0; i < handlingBlocks.Length; i++)
        {
            if (i < handling[selectedCar])
            {
                handlingBlocks[i].GetComponent<MeshRenderer>().material = gold;
            }
            else
            {
                handlingBlocks[i].GetComponent<MeshRenderer>().material = black;
            }
        }
        healthText.text = "" + health[selectedCar];
    }

    public void startGame()
    {
        PlayerPrefs.SetInt("selectedCar", selectedCar);
        int selectedTrack = PlayerPrefs.GetInt("selectedTrack");
        SceneManager.LoadScene(selectedTrack + 1, LoadSceneMode.Single);
    }
}
