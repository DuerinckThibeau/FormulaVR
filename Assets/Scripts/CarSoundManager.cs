using UnityEngine;

public class CarSoundManager : MonoBehaviour
{
	public AudioSource idleAudioSource;
	public AudioSource drivingAudioSource;
	public Rigidbody carRigidbody;
	public float speedThreshold = 1.0f;
	public float transitionSpeed = 0.1f;
	private bool gameStarted = false;

	void Start()
	{
		idleAudioSource.loop = true;
		drivingAudioSource.loop = true;

		idleAudioSource.volume = 0;
		drivingAudioSource.volume = 0;
	}

	void Update()
	{
		if (!gameStarted) return;

		float speed = carRigidbody.velocity.magnitude;

		if (speed < speedThreshold)
		{
			idleAudioSource.volume = Mathf.Lerp(idleAudioSource.volume, 1, Time.deltaTime * transitionSpeed);
			drivingAudioSource.volume = Mathf.Lerp(drivingAudioSource.volume, 0, Time.deltaTime * transitionSpeed);
		}
		else
		{
			idleAudioSource.volume = Mathf.Lerp(idleAudioSource.volume, 0, Time.deltaTime * transitionSpeed);
			drivingAudioSource.volume = Mathf.Lerp(drivingAudioSource.volume, 1, Time.deltaTime * transitionSpeed);
		}
	}

	public void StartGame()
	{
		idleAudioSource.Play();
		drivingAudioSource.Play();
		gameStarted = true;
	}

	public void MuteSounds()
	{
		idleAudioSource.volume = 0;
		drivingAudioSource.volume = 0;
	}
}
