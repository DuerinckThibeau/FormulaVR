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
    
    bool isPaused = false;
    bool startGame = false;

    void Start()
    {
        
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
        startGame = true;
        Time.timeScale = 1;
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

    }

}
