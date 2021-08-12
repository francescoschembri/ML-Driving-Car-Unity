using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class IACarController : Agent
{
    private Rigidbody carRigidbody;
    private CarController cc;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private float secondsOfStationarity;
    public float secondsOfStationarityBeforeReset = 10f;


    public override void Initialize()
    {
        cc = GetComponent<CarController>();
        carRigidbody = GetComponent<Rigidbody>();
        initialPosition = gameObject.transform.position;
        initialRotation = gameObject.transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;
        cc.ResetCar();
        gameObject.transform.rotation = initialRotation;
        gameObject.transform.position = initialPosition;
        secondsOfStationarity = 0f;
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.InverseTransformDirection(carRigidbody.velocity));
        sensor.AddObservation(transform.InverseTransformDirection(carRigidbody.angularVelocity).y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        CheckIfStationary();
        cc.SetAcceleration(actions.DiscreteActions[0] == 2 ? -1 : actions.DiscreteActions[0]);
        cc.SetSteering(actions.DiscreteActions[1] == 2 ? -1 : actions.DiscreteActions[1]);

        if (cc.hasCrashed)
        {
            AddReward(-5f);
            EndEpisode();
        }
        if (secondsOfStationarity >= secondsOfStationarityBeforeReset)
        {
            AddReward(-10f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        if (discreteActions[0] < 0) discreteActions[0] = 2;
        discreteActions[1] = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        if (discreteActions[1] < 0) discreteActions[1] = 2;
    }

    private void CheckIfStationary()
    {
        if (cc.isMoving())
            secondsOfStationarity = 0f;
        else
            secondsOfStationarity += Time.fixedDeltaTime;
    }
}
