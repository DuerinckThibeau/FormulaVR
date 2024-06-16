using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointSingle : MonoBehaviour
{
    private TrackCheckpoints trackCheckpoints;
    private void OnTriggerEnter(Collider other) {
      if(other.tag == "Player") {
        trackCheckpoints.PlayerThroughCheckpoint(this);
      }

    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints) {
      this.trackCheckpoints = trackCheckpoints;
    }
}

