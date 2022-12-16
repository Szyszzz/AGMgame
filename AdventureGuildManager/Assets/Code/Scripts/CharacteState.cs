using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacteState : MonoBehaviour
{
    private NavMeshAgent CharacterNavmeshAgent;
    private Vector3 CharacterNextRandomDestination;
    private Vector3 CharacterNextPremadeDestination;
    

    public CharacterStates CurrentCharacterState = CharacterStates.Idle;
    public enum CharacterStates
    { 
        Idle,
        Wander,
        WalkTowards,
        Interact
    }
    public float MaxWanderDistance;
    [Range(1,10)]public int CenterWeightPower = 1;
    public GameObject[] PremadeTargetPositions;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(CharacterNextRandomDestination, 1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(CharacterNextPremadeDestination, 1f);
    }

    private void Start()
    {
        CharacterNavmeshAgent = GetComponent<NavMeshAgent>();
        GenerateNewRandomDestination();
        PickPremadeTargetPosition();
    }

    void Update()
    {
        switch(CurrentCharacterState)
        {
            case CharacterStates.Idle:
                IdleStateBehaviour();
                break;

            case CharacterStates.Wander:
                WanderStateBehaviour();
                break;

            case CharacterStates.WalkTowards:
                WalkTowardsStateBehaviour();
                break;

            case CharacterStates.Interact:
                InteractStateBehaviour();
                break;
        }
    }

    #region Idle

    public void IdleStateBehaviour()
    {

    }

    #endregion

    #region Wander

    public void WanderStateBehaviour()
    {
        MoveToRandom();
        if (IsDestinationReached())
        {
            GenerateNewRandomDestination();
        }
    }

    private void GenerateNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * MaxWanderDistance;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, MaxWanderDistance, 1);
        CharacterNextRandomDestination = LerpTowardsMiddleOfTheBoard(hit.position);
    }

    private Vector3 LerpTowardsMiddleOfTheBoard(Vector3 TargetPosition)
    {
        float CenterWeight = Mathf.Pow(Random.Range(0.7f, 1f), CenterWeightPower);
        return Vector3.Lerp(Vector3.zero, TargetPosition, CenterWeight);
    }


    private void MoveToRandom()
    {
        CharacterNavmeshAgent.destination = CharacterNextRandomDestination;
    }

    #endregion

    #region WalkTowards
    public void WalkTowardsStateBehaviour()
    {
        MoveToPremade();
        if (IsDestinationReached())
        {
            PickPremadeTargetPosition();
        }
    }

    public void PickPremadeTargetPosition()
    {
        int RandomTargetIndex = Random.Range(0, PremadeTargetPositions.Length);
        CharacterNextPremadeDestination = PremadeTargetPositions[RandomTargetIndex].transform.position;
    }

    private void MoveToPremade()
    {
        CharacterNavmeshAgent.destination = CharacterNextPremadeDestination;
    }

    #endregion

    #region Interact

    private void InteractStateBehaviour()
    {

    }

    #endregion

    #region SharedFunctions

    public bool IsDestinationReached()
    {

        float dist = CharacterNavmeshAgent.remainingDistance;

        return (dist != Mathf.Infinity && CharacterNavmeshAgent.pathStatus != NavMeshPathStatus.PathInvalid && CharacterNavmeshAgent.remainingDistance == 0);
    }

    #endregion


}
