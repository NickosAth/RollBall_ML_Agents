using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class agent : Agent
{
    private Rigidbody rigidBody;


    public float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public Transform Target;

    public override void OnEpisodeBegin()
    {
        if(this.transform.localPosition.y < 0)
        {
            //if agent falls -> zero momentum
            this.rigidBody.angularVelocity = Vector3.zero;
            this.rigidBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        //Move target to new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Target and Agent Positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(this.transform.localPosition);

        //Agent Velocity
        sensor.AddObservation(rigidBody.velocity.x);
        sensor.AddObservation(rigidBody.velocity.z);

    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Get the continuous action inputs
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];

        // Apply the action inputs to the Rigidbody
        rigidBody.AddForce(controlSignal * speed);

        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached Target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell Off Platform
        if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

}
