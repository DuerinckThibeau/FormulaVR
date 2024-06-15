using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Random = UnityEngine.Random;

public class CarAgent : Agent
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private Transform spawnPosition;

    private WheelController wheels;


    private void Awake() {
        wheels = GetComponent<WheelController>();
    }

    private void Start() {
        trackCheckpoints.OnPlayerCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnPlayerWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
        trackCheckpoints.OnPlayerWinRace += TrackCheckpoints_OnWin;
    }

    private void TrackCheckpoints_OnWin(object sender, EventArgs e)
    {
        EndEpisode();
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, EventArgs e)
    {
        AddReward(-1f);
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, EventArgs e)
    {
        AddReward(1f);
    }

    public override void OnEpisodeBegin() {
        transform.position = spawnPosition.position + new Vector3(Random.Range(-5f, +5f), 0, Random.Range(-5f, +5f));
        transform.forward = spawnPosition.forward;
        trackCheckpoints.ResetCheckpoint(transform);
        wheels.StopCompletely();
    }

    public override void CollectObservations(VectorSensor sensor) {
        Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(transform).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
    }

    public override void OnActionReceived(ActionBuffers actions) {
        float forwardAmount = 0f;
        float turnAmount = 0f;

        switch (actions.DiscreteActions[0]) {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = +4f; break;
            case 2: forwardAmount = -4f; break;
        }
        switch (actions.DiscreteActions[1]) {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = +5f; break;
            case 2: turnAmount = -5f; break;
        }

        wheels.SetInputs(forwardAmount, turnAmount);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        int forwardAction = 0;
        if (Input.GetKey(KeyCode.UpArrow)) forwardAction = 1;
        if (Input.GetKey(KeyCode.DownArrow)) forwardAction = 2;

        int turnAction = 0;
        if (Input.GetKey(KeyCode.RightArrow)) turnAction = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) turnAction = 2;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = forwardAction;
        discreteActions[1] = turnAction;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Wall")
        {
            // Muur geraakt
            AddReward(-0.5f);
            //EndEpisode();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall") {
            // Muur geraakt
            AddReward(-0.1f);
        }
    }
}
