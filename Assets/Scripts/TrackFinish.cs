using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackFinish : MonoBehaviour
{
  [SerializeField] private TrackCheckpoints trackCheckpoints;

  private void Start() {
    trackCheckpoints.OnPlayerWinRace += TrackCheckpoints_OnPlayerWinRace;

    Hide();

  }


  private void TrackCheckpoints_OnPlayerWinRace(object sender, System.EventArgs e) {
    Show();
  }

  private void Show() {
    gameObject.SetActive(true);
  }

  private void Hide() {
    gameObject.SetActive(false);
  }
}

