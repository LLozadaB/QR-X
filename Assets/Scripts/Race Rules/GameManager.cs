using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Race Settings")]
    public GameObject[] playerCars;
    public Transform spawnPoint;
    public GameObject[] playerCameras;
    public int selectedCar;
    public int totalLaps = 3;
    public List<PrometeoCarController> carsControllers;
    public TrackWaypoints trackWaypoints;
    public GameObject collectable;

    [Header("UI")]
    public TMP_Text lapText;
    public TMP_Text positionText;
    public TMP_Text timeText;
    public TMP_Text wreckedText;
    public GameObject scoreboardUI;
    public Transform scoreboardContent;
    public GameObject scoreboardEntryPrefab;

    [Header("Countdown")]
    public TMP_Text countdownText;
    private bool raceStarted = false;
    private bool countdownActive = false;
    public AudioSource audioSource;
    public AudioClip countdownBeep;
    public AudioClip countdownGo;

    [Header("Wrecks")]
    public AudioClip AIWreckIndicator;
    public AudioClip PlayerWreckIndicator;
    public AudioClip finishSound;

    [Header("Music")]
    public MusicManager musicManager;

    private Dictionary<PrometeoCarController, int> carLaps = new Dictionary<PrometeoCarController, int>();
    private Dictionary<PrometeoCarController, float> carTimes = new Dictionary<PrometeoCarController, float>();
    private Dictionary<PrometeoCarController, bool> carFinished = new Dictionary<PrometeoCarController, bool>();
    private Dictionary<PrometeoCarController, bool> carWrecked = new Dictionary<PrometeoCarController, bool>();
    private Dictionary<PrometeoCarController, float> lapCooldowns = new Dictionary<PrometeoCarController, float>();
    private float lapCooldownTime = 10.0f; // To prevent lap skipping
    private List<PrometeoCarController> wreckedOrder = new List<PrometeoCarController>();
    private bool raceFinished = false;
    private bool playerWrecked = false;
    private float wreckedTimer = 0.0f;
    private float wreckedDelay = 4.0f;
    private PrometeoCarController player;

    private void Awake()
    {
        selectedCar = PlayerPrefs.GetInt("selectedCar");
        playerCars[selectedCar].tag = "Player";
        playerCars[selectedCar].SetActive(true);
        playerCameras[selectedCar].SetActive(true);
    }

    void Start()
    {
        GameObject car = playerCars[selectedCar];
        car.transform.position = spawnPoint.position;
        car.transform.rotation = spawnPoint.rotation;
        carsControllers[0] = GameObject.FindGameObjectWithTag("Player").GetComponent<PrometeoCarController>();
        player = carsControllers[0];

        foreach (var carController in carsControllers)
        {
            carLaps[carController] = -1;
            carTimes[carController] = 0f;
            carFinished[carController] = false;
            carWrecked[carController] = false;
            lapCooldowns[carController] = 0f;
        }
        scoreboardUI.SetActive(false);
        wreckedText.gameObject.SetActive(false);

        StartCoroutine(RaceCountdown());
    }

    void Update()
    {
        if (raceFinished)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                PlayerPrefs.SetInt("returnFromRace", 1);
                SceneManager.LoadScene(0);
            }
            return;
        }

        // If the player collides with the collectable, disable the collectable object
        if (player != null && collectable != null &&
        player.transform.GetChild(0).GetComponent<Collider>().bounds.Intersects(collectable.GetComponent<Collider>().bounds))
        {
            // It's "collected": It doesn't unlock anything yet, and will probably respawn next race
            collectable.SetActive(false);
        }

        // Check for wrecks and update times/laps
        foreach (var car in carsControllers)
        {
            // Wrecked check
            if (!carWrecked[car] && (car.currentHealth <= 0 || car.DriveController == PrometeoCarController.DriverType.Wrecked))
            {
                car.DriveController = PrometeoCarController.DriverType.Wrecked;
                if (car == player)
                {
                    audioSource.volume = 0.6f;
                    musicManager.StopMusic();
                    audioSource.PlayOneShot(PlayerWreckIndicator);
                    carWrecked[car] = true;
                    wreckedOrder.Insert(0, car);
                    carFinished[car] = true;
                    playerWrecked = true;
                    wreckedTimer = 0f;
                }
                else
                {
                    OnAIWrecked(car);
                    carWrecked[car] = true;
                    wreckedOrder.Insert(0, car);
                    carFinished[car] = true;
                }
            }

            // Only update time/lap if not finished/wrecked
            if (!carFinished[car] && !carWrecked[car])
            {
                if (raceStarted)
                {
                    carTimes[car] += Time.deltaTime;
                }

                // Lap check
                if (car.currentNode == 0 && carLaps[car] < totalLaps && lapCooldowns[car] <= 0f)
                {
                    // Increment lap if crossed the finish line
                    if (carLaps[car] == 0 || car.transform.position.z > trackWaypoints.nodes[0].position.z)
                    {
                        carLaps[car]++;
                        lapCooldowns[car] = lapCooldownTime;
                        if (carLaps[car] >= totalLaps)
                        {
                            carFinished[car] = true;
                            if (car == player)
                            {
                                player.DriveController = PrometeoCarController.DriverType.AI;
                            }
                        }
                    }
                }
            }
        }

        // If player is wrecked, show wrecked message and start timer
        if (playerWrecked && !scoreboardUI.activeSelf)
        {
            lapText.text = "";
            positionText.text = "";
            timeText.text = "";
            wreckedText.text = "You have WRECKED!";
            wreckedText.gameObject.SetActive(true);
            wreckedTimer += Time.deltaTime;
            if (wreckedTimer >= wreckedDelay)
            {
                audioSource.volume = 0.8f;
                ShowScoreboard(GetSortedCars());
                raceFinished = true;
                wreckedText.gameObject.SetActive(false);
            }
            return;
        }

        // Wrecked test input for player
        if (Input.GetKeyDown(KeyCode.K))
        {
            player.DriveController = PrometeoCarController.DriverType.Wrecked;
            playerWrecked = true;
        }

        //Wrecked test input for AI
        if (Input.GetKeyDown(KeyCode.L))
        {
            carsControllers[1].DriveController = PrometeoCarController.DriverType.Wrecked;
        }

        // If player finished, show scoreboard after a delay
        if (carFinished[player] && !playerWrecked && !scoreboardUI.activeSelf)
        {
            StartCoroutine(PlayerFinish(4f));
        }

        // Calculate UI text
        List<PrometeoCarController> sortedCars = GetSortedCars();
        if (carLaps[player] < 0)
        {
            lapText.text = $"1/{totalLaps}";

        }
        else
        {
            lapText.text = $"{Mathf.Min(carLaps[player] + 1, totalLaps)}/{totalLaps}";
        }
        positionText.text = $"{sortedCars.IndexOf(player) + 1}/{carsControllers.Count}";
        timeText.text = $"{FormatTime(carTimes[player])}";

        foreach (var car in carsControllers)
        {
            if (lapCooldowns[car] > 0f)
            {
                lapCooldowns[car] -= Time.deltaTime;
            }
        }
    }

    // If AI wrecked, show wrecked message shortly
    public void OnAIWrecked(PrometeoCarController car)
    {
        if (carWrecked[car]) return; // Car already wrecked
        StartCoroutine(AIWreckedMessage(car.name));
    }

    // Controls how long the AI wreck message lasts
    System.Collections.IEnumerator AIWreckedMessage(string name)
    {
        audioSource.PlayOneShot(AIWreckIndicator);
        wreckedText.text = $"{name} has WRECKED!";
        wreckedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        wreckedText.gameObject.SetActive(false);
    }

    // The delay before showing the scoreboard/allowing an exit
    IEnumerator PlayerFinish(float delay)
    {
        audioSource.PlayOneShot(finishSound);
        wreckedText.text = "FINISHED!";
        wreckedText.gameObject.SetActive(true);
        raceFinished = true;
        yield return new WaitForSeconds(delay);
        ShowScoreboard(GetSortedCars());
        wreckedText.gameObject.SetActive(false);
    }

    IEnumerator RaceCountdown()
    {
        countdownActive = true;
        raceStarted = false;

        // Freeze all cars
        foreach (var car in carsControllers)
            car.canMove = false;

        countdownText.gameObject.SetActive(true);

        // Countdown noises and text
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            if (audioSource && countdownBeep)
            {
                audioSource.PlayOneShot(countdownBeep);
            }
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "GO!!!";
        if (audioSource && countdownGo)
        {
            audioSource.PlayOneShot(countdownGo);
        }

        // Unfreeze all cars
            foreach (var car in carsControllers)
                car.canMove = true;

        raceStarted = true;
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        countdownActive = false;
    }

    // Sorts finished (not wrecked) to racing to wrecked (in wreckedOrder)
    List<PrometeoCarController> GetSortedCars()
    {
        List<PrometeoCarController> finished = new List<PrometeoCarController>();
        List<PrometeoCarController> racing = new List<PrometeoCarController>();
        List<PrometeoCarController> wrecked = new List<PrometeoCarController>(wreckedOrder);

        foreach (var car in carsControllers)
        {
            if (carWrecked[car]) continue;
            if (carFinished[car])
                finished.Insert(0, car);
            else
                racing.Insert(0, car);
        }

        // Sort finished and racing by progress
        finished.Sort((a, b) => CompareCars(a, b));
        racing.Sort((a, b) => CompareCars(a, b));

        // Final order: finished, then racing, then wrecked (in order wrecked)
        List<PrometeoCarController> sorted = new List<PrometeoCarController>();
        sorted.AddRange(finished);
        sorted.AddRange(racing);
        sorted.AddRange(wrecked);
        return sorted;
    }

    int CompareCars(PrometeoCarController a, PrometeoCarController b)
    {
        // Higher lap first
        if (carLaps[a] != carLaps[b])
            return carLaps[b].CompareTo(carLaps[a]);
        // Higher node first
        if (a.currentNode != b.currentNode)
            return b.currentNode.CompareTo(a.currentNode);
        // Closest to next node first
        float aDist = Vector3.Distance(a.transform.position, trackWaypoints.nodes[a.currentNode].position);
        float bDist = Vector3.Distance(b.transform.position, trackWaypoints.nodes[b.currentNode].position);
        return aDist.CompareTo(bDist);
    }

    void ShowScoreboard(List<PrometeoCarController> sortedCars)
    {
        scoreboardUI.SetActive(true);

        // Clear previous entries
        foreach (Transform child in scoreboardContent)
            Destroy(child.gameObject);

        for (int i = 0; i < sortedCars.Count; i++)
        {
            var car = sortedCars[i];
            GameObject entry = Instantiate(scoreboardEntryPrefab, scoreboardContent);
            TMP_Text[] texts = entry.GetComponentsInChildren<TMP_Text>();
            texts[0].text = $"{i + 1}"; // Position

            // Name
            if (car == carsControllers[0])
                texts[1].text = "Zip";
            else
                texts[1].text = car.name;

            // Time slot
            if (carWrecked[car])
            {
                // Wrecked text
                string[] wreckedTexts = { "Wrecked", "Trashed", "Rammed" };
                texts[2].text = wreckedTexts[i % wreckedTexts.Length];
            }
            else if (carFinished[car])
            {
                texts[2].text = FormatTime(carTimes[car]);
            }
            else
            {
                // Still racing
                string[] racingTexts = { "Racing...", "Driving...", "Running..." };
                texts[2].text = racingTexts[i % racingTexts.Length];
            }
        }
    }

    string FormatTime(float t)
    {
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        int ms = Mathf.FloorToInt((t * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}.{ms:00}";
    }
}