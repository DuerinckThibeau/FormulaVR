using System.Collections;
using System.Collections.Generic;
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

	bool isPaused = false;
	bool startGame = false;
	bool gameStarted = false;

	void Start()
	{
		idleAudioSource.loop = true;
		drivingAudioSource.loop = true;

		idleAudioSource.volume = 0;
		drivingAudioSource.volume = 0;
	}

	void Update()
	{
		startMenu.SetActive(!startGame);
		if (startGame)
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
			startGame = true;
			Time.timeScale = 1;
			gameStarted = true;
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
}
