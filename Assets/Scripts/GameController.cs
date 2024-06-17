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
	[SerializeField] AudioSource idleAudioSource = null;
	[SerializeField] AudioSource drivingAudioSource = null;
	[SerializeField] Rigidbody carRigidbody = null;
	[SerializeField] float speedThreshold = 1.0f;
	[SerializeField] TextMeshProUGUI countdownText = null;
    [SerializeField] AudioClip raceStartClip = null;

	private AudioSource audioSource;		
	bool isPaused = false;
	bool startGame = false;
	bool gameStarted = false;

	bool countdownFinished = false;


    void Start()
	{
		idleAudioSource.loop = true;
		drivingAudioSource.loop = true;
		startMenu.SetActive(true);
        countdownText.gameObject.SetActive(false);

		audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = raceStartClip;
		idleAudioSource.volume = 0;
		drivingAudioSource.volume = 0;
	}

	void Update()
	{
		startMenu.SetActive(!startGame);
		if (startGame && countdownFinished)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				isPaused = !isPaused;
			}
			Time.timeScale = isPaused ? 0 : 1;
			pauseMenu.SetActive(isPaused);

			if (gameStarted)
			{
				UpdateCarSounds();
			}
		}
		else
		{
			Time.timeScale = 0;
		}
	}

	void UpdateCarSounds()
	{
		float speed = carRigidbody.velocity.magnitude;
		if (speed < speedThreshold)
		{
			idleAudioSource.volume = 1;
			drivingAudioSource.volume = 0;
		}
		else
		{
			idleAudioSource.volume = 0;
			drivingAudioSource.volume = 1;
		}
	}

	public void StartGame()
	{
		if (!startGame)
		{
            StartCoroutine(Countdown());
			idleAudioSource.Play();
			drivingAudioSource.Play();
        }
	}

	public void ExitGame()
	{
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
        countdownFinished = false;


    }

	public void MuteAllSounds()
	{
		idleAudioSource.volume = 0;
		drivingAudioSource.volume = 0;
	}

	public void UnpauseGame()
	{
		if (isPaused)
		{
			isPaused = false;
		}
	}

    private IEnumerator Countdown()
    {
		startMenu.SetActive(false);
        startGame = true;
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


		countdownFinished = true;
        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1;
        StartCarSounds();
    }

    private void StartCarSounds()
    {
        idleAudioSource.Play();
        drivingAudioSource.Play();
        gameStarted = true;
    }

}
