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

    private Transform lastCheckpoint;
    private float timeAgainstWall = 0f;
    private float maxTimeAgainstWall = 5f; // Maximum time allowed against the wall in seconds

    private void Awake()
    {
        wheels = GetComponent<WheelController>();
    }

    private void Start()
    {
        trackCheckpoints.OnPlayerCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnPlayerWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
        trackCheckpoints.OnPlayerWinRace += TrackCheckpoints_OnWin;
        lastCheckpoint = spawnPosition;
    }

    private void TrackCheckpoints_OnWin(object sender, EventArgs e)
    {
        Debug.Log("Episode won");
        EndEpisode();
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, EventArgs e)
    {
        AddReward(-1f); // Corrected the reward signal for wrong checkpoint
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, EventArgs e)
    {
        AddReward(1f);
        lastCheckpoint = trackCheckpoints.GetCurrentCheckpointTransform();
    }

    public override void OnEpisodeBegin()
    {
        if (shouldReset)
        {
            ResetToLastCheckpoint();
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
            case 1: forwardAmount = +30f; break;
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
            shouldReset = false;
            EndEpisode();
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
            timeAgainstWall = 0f; // Reset the timer when a collision starts
        }
        else if (collision.gameObject.CompareTag("Checkpoint"))
        {
            Debug.Log("Hit Checkpoint");
            AddReward(1f);
            lastCheckpoint = collision.transform; // Update the last checkpoint
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Staying on Wall");
            AddReward(-0.1f);
            CorrectCourse();

            timeAgainstWall += Time.deltaTime;
            if (timeAgainstWall >= maxTimeAgainstWall)
            {
                Debug.Log("Too long against wall, resetting to last checkpoint.");
                ResetToLastCheckpoint();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            timeAgainstWall = 0f; // Reset the timer when the collision ends
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

    private void ResetToLastCheckpoint()
    {
        transform.position = lastCheckpoint.position;
        transform.forward = lastCheckpoint.forward;
        wheels.StopCompletely();
        Debug.Log("Reset to last checkpoint");
    }

    public void RequestManualReset()
    {
        shouldReset = true;
        EndEpisode();
    }
}
