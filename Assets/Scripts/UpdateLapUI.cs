using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateLapUI : MonoBehaviour
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private TextMeshProUGUI lapText;

    private int maxLap = 3;
    private void Start()
    {
        trackCheckpoints.OnPlayerUpdateLap += TrackCheckpoints_OnPlayerUpdateLap;
    }

    private void TrackCheckpoints_OnPlayerUpdateLap(object sender, PlayerUpdateLapEventArgs e)
    {
        lapText.text = e.LapNumber.ToString() + "/" + maxLap.ToString();
    }

   
}
