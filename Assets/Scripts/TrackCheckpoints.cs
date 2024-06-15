using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{

    public class CarCheckpointEventArgs : EventArgs
    {
        public Transform carTransform;
    }

    public event EventHandler<CarCheckpointEventArgs> OnPlayerCorrectCheckpoint;
    public event EventHandler<CarCheckpointEventArgs> OnPlayerWrongCheckpoint;

    protected virtual void RaiseCorrectCheckpoint(CarCheckpointEventArgs e)
    {
        OnPlayerCorrectCheckpoint?.Invoke(this, e);
    }

    protected virtual void RaiseWrongCheckpoint(CarCheckpointEventArgs e)
    {
        OnPlayerWrongCheckpoint?.Invoke(this, e);
    }

    // Call this method when the player passes a correct checkpoint
    public void PlayerPassedCorrectCheckpoint(Transform carTransform)
    {
        RaiseCorrectCheckpoint(new CarCheckpointEventArgs { carTransform = carTransform });
    }

    // Call this method when the player passes a wrong checkpoint
    public void PlayerPassedWrongCheckpoint(Transform carTransform)
    {
        RaiseWrongCheckpoint(new CarCheckpointEventArgs { carTransform = carTransform });
    }
  //  public event EventHandler OnPlayerCorrectCheckpoint;
  //public event EventHandler OnPlayerWrongCheckpoint;
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
        OnPlayerCorrectCheckpoint?.Invoke(this, (CarCheckpointEventArgs)EventArgs.Empty);
      }
    } else {
      OnPlayerWrongCheckpoint?.Invoke(this, (CarCheckpointEventArgs)EventArgs.Empty);

    }
  }
    public CheckPointSingle GetNextCheckpoint(Transform transform) {
        return checkPointSingleList[netCheckpointSingelIndex];
    }

    internal void ResetCheckpoint(Transform transform)
    {
        Awake();
    }


}





