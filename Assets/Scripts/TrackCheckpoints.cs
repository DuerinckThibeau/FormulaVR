using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;




public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler OnPlayerCorrectCheckpoint;
    public event EventHandler OnPlayerWrongCheckpoint;
    public event EventHandler OnPlayerWinRace;
    public event EventHandler<PlayerUpdateLapEventArgs> OnPlayerUpdateLap;

    private List<CheckPointSingle> checkPointSingleList;
    private int netCheckpointSingelIndex;
    private int lapCount = 1;
    private void Awake()
    {
        Transform[] checkpoints = GetComponentsInChildren<Transform>();



        checkPointSingleList = new List<CheckPointSingle>();
        for (int i = 1; i < checkpoints.Length; i++)
        {
            Renderer wallRenderer =  checkpoints[i].GetComponent<Renderer>();
            wallRenderer.enabled = false;
            CheckPointSingle checkPointSingle = checkpoints[i].GetComponent<CheckPointSingle>();
            checkPointSingle.SetTrackCheckpoints(this);

            checkPointSingleList.Add(checkPointSingle);
        }

        netCheckpointSingelIndex = 0;



    }
    public void PlayerThroughCheckpoint(CheckPointSingle checkpoint)
    {
        if (checkPointSingleList.IndexOf(checkpoint) == netCheckpointSingelIndex)
        {
            netCheckpointSingelIndex++;
            if (netCheckpointSingelIndex == checkPointSingleList.Count)
            {
                lapCount++;
                OnPlayerUpdateLap?.Invoke(this, new PlayerUpdateLapEventArgs(lapCount));
                netCheckpointSingelIndex = 0;
                if (lapCount >= 4)
                {
                    OnPlayerWinRace?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                OnPlayerCorrectCheckpoint?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            OnPlayerWrongCheckpoint?.Invoke(this, EventArgs.Empty);

        }
    }
    public CheckPointSingle GetNextCheckpoint(Transform transform)
    {
        return checkPointSingleList[netCheckpointSingelIndex];
    }


    internal void ResetCheckpoint(Transform transform)
    {
        Awake();
    }

    public Transform GetCurrentCheckpointTransform()
    {
        if (netCheckpointSingelIndex == 0)
        {
            return checkPointSingleList[0].transform;
        }
        return checkPointSingleList[netCheckpointSingelIndex - 1].transform;
    }


}

public class PlayerUpdateLapEventArgs : EventArgs
{
    public int LapNumber { get; }

    public PlayerUpdateLapEventArgs(int lapNumber)
    {
        LapNumber = lapNumber;
    }
}





