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
    private const float raycastDistance = 5f;
    private int maxStepCount = 500; // Maximum steps per episode
    private int currentStepCount;
    private bool shouldReset = false;

    private void Awake()
    {
        wheels = GetComponent<WheelController>();
    }

    private void Start()
    {
        trackCheckpoints.OnPlayerCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnPlayerWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
        trackCheckpoints.OnPlayerWinRace += TrackCheckpoints_OnWin;
    }

    private void TrackCheckpoints_OnWin(object sender, EventArgs e)
    {
        Debug.Log("Episode won");
        EndEpisode();
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, EventArgs e)
    {
        Debug.Log("Wrong checkpoint");
        AddReward(1f);
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, EventArgs e)
    {
        Debug.Log("Correct checkpoint");
        AddReward(1f);
    }

    public override void OnEpisodeBegin()
    {
        if (shouldReset)
        {
            transform.position = spawnPosition.position;
            transform.forward = spawnPosition.forward;
            trackCheckpoints.ResetCheckpoint(transform);
            wheels.StopCompletely();
            currentStepCount = 0;
            shouldReset = false;

            Debug.Log("Episode began");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(transform).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);

        // Add distance to walls as observations
        sensor.AddObservation(GetDistanceToWall(Vector3.forward));
        sensor.AddObservation(GetDistanceToWall(Vector3.left));
        sensor.AddObservation(GetDistanceToWall(Vector3.right));
    }

    private float GetDistanceToWall(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, raycastDistance))
        {
            return hit.distance / raycastDistance;
        }
        else
        {
            return 1f; // No wall within raycast distance
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float forwardAmount = 0f;
        float turnAmount = 0f;

        switch (actions.DiscreteActions[0])
        {
            case 0: forwardAmount = 0f; break;
            case 1: forwardAmount = +4f; break;
        }
        switch (actions.DiscreteActions[1])
        {
            case 0: turnAmount = 0f; break;
            case 1: turnAmount = +10f; break;
            case 2: turnAmount = -10f; break;
        }

        wheels.SetInputs(forwardAmount, turnAmount);

        currentStepCount++;
        if (currentStepCount >= maxStepCount)
        {
            Debug.Log("Max steps reached, considering reset.");
            shouldReset = false;
            EndEpisode(); // This won't reset the car immediately, it sets up for a manual reset on the next OnEpisodeBegin
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        if (Input.GetKey(KeyCode.UpArrow)) forwardAction = 1;

        int turnAction = 0;
        if (Input.GetKey(KeyCode.RightArrow)) turnAction = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) turnAction = 2;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = forwardAction;
        discreteActions[1] = turnAction;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit Wall");
            AddReward(-0.5f);
            CorrectCourse();
        }
        else if (collision.gameObject.CompareTag("Checkpoint"))
        {
            Debug.Log("Hit Checkpoint");
            AddReward(1f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Staying on Wall");
            AddReward(-0.1f);
            CorrectCourse();
        }
    }

    private void CorrectCourse()
    {
        float distanceLeft = GetDistanceToWall(Vector3.left);
        float distanceRight = GetDistanceToWall(Vector3.right);

        if (distanceLeft < 0.2f)
        {
            wheels.SetInputs(0f, 10f); // Turn right
        }
        else if (distanceRight < 0.2f)
        {
            wheels.SetInputs(0f, -10f); // Turn left
        }
    }

    public void RequestManualReset()
    {
        shouldReset = true;
        EndEpisode();
    }
}
