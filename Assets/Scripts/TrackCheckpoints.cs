using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
  public event EventHandler OnPlayerCorrectCheckpoint;
  public event EventHandler OnPlayerWrongCheckpoint;
  public event EventHandler OnPlayerWinRace;

  private List<CheckPointSingle> checkPointSingleList;
  private int netCheckpointSingelIndex;
  private void Awake() {
    Transform[] checkpoints = GetComponentsInChildren<Transform>();

    checkPointSingleList = new List<CheckPointSingle>();
    for (int i = 1; i < checkpoints.Length; i++) {
      CheckPointSingle checkPointSingle = checkpoints[i].GetComponent<CheckPointSingle>();
      checkPointSingle.SetTrackCheckpoints(this);

      checkPointSingleList.Add(checkPointSingle);
    }

    netCheckpointSingelIndex = 0;



  }
  public void PlayerThroughCheckpoint(CheckPointSingle checkpoint) {
    if (checkPointSingleList.IndexOf(checkpoint) == netCheckpointSingelIndex) {
      netCheckpointSingelIndex++;
      if(netCheckpointSingelIndex == checkPointSingleList.Count) {
        OnPlayerWinRace?.Invoke(this, EventArgs.Empty);
      } else {
        OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);
      }
    } else {
      OnPlayerWrongCheckpoint?.Invoke(this, EventArgs.Empty);

    }
  }
}
