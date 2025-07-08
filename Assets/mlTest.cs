using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class mlTest : Agent
{

    
    public Transform target;
    public float move = 2;

    float nes = 20;



    public override void OnEpisodeBegin()
    {

        transform.position = Random.Range(-nes, nes) * Vector3.forward +
            Random.Range(-nes, nes) * Vector3.up;

    }



    public override void CollectObservations(VectorSensor sensor)
    {

        sensor.AddObservation(target.position);
        sensor.AddObservation(transform.position);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        float eks = move * 1f;

        transform.Translate(new Vector3(0f, actions.ContinuousActions[0] * eks, 
            actions.ContinuousActions[1] * eks));

        AddReward(1 / Vector3.Distance(transform.position, target.position) - 0.5f);
    }

    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // za direktno kontroliranje ml agenta
        ActionSegment<float> actionSegment = actionsOut.ContinuousActions;
        actionSegment[0] = Input.GetAxisRaw("Horizontal");
        actionSegment[1] = Input.GetAxisRaw("Vertical");

    }
    

    

    private void OnCollisionEnter(Collision collision)
    {

        if (!collision.transform.GetComponent<mlTest>()) {
            //AddReward(1 / Vector3.Distance(transform.position, target.position));
            AddReward(10);
            EndEpisode();
        }

    }

    





}
