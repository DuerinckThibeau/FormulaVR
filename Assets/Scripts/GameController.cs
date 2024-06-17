using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] WheelController car = null;

    [SerializeField] Transform spawn = null;

    [SerializeField] TrackCheckpoints trackCheckpoints = null;

    [SerializeField] GameObject pauseMenu = null;
    [SerializeField] GameObject startMenu = null;

    [SerializeField] TextMeshProUGUI countdownText = null;

    [SerializeField] AudioClip raceStartClip = null;

    private AudioSource audioSource;

    bool isPaused = false;
    bool startGame = false;

    void Start()
    {
        startMenu.SetActive(true);
        countdownText.gameObject.SetActive(false);

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = raceStartClip;
    }

    void Update()
    {
      startMenu.SetActive(!startGame);
      if(startGame) {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          isPaused = !isPaused;
        }
        Time.timeScale = isPaused ? 0 : 1;
        pauseMenu.SetActive(isPaused);
      } else {
        Time.timeScale = 0;
      }
    }


    public void UnpauseGame() {
      if(isPaused) {
        isPaused = false;
      }
    }
    public void StartGame() {
      if(!startGame) {
        StartCoroutine(Countdown());
        }
    }
    public void ExitGame() {
      Application.Quit();
    }

    public void ResetGame() 
    { 
        Transform transform = car.GetComponent<Transform>();
        transform.position = spawn.position;
        transform.forward = spawn.forward;
        trackCheckpoints.ResetCheckpoint(transform);
        car.StopCompletely();
        startGame = false;
        isPaused = false;
        pauseMenu.SetActive(false);

    }

    private IEnumerator Countdown()
    {
        countdownText.gameObject.SetActive(true);
        if (audioSource != null)
        {
            audioSource.Play();
        }

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1);
        }

        countdownText.gameObject.SetActive(false);
        startGame = true;
        Time.timeScale = 1;
    }

}
