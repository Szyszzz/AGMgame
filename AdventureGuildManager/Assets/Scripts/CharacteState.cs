using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacteState : MonoBehaviour
{
    private NavMeshAgent CharacterNavmeshAgent;
    private bool WasRandomWanderDestinationGenerated = false;
    private bool WasPremadeTargetDestinationPicked = false;
    private Vector3 CharacterNextDestination;
    

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
        if (CurrentCharacterState == CharacterStates.Wander)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(CharacterNextDestination, 1f);
        }

        if (CurrentCharacterState == CharacterStates.WalkTowards)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(CharacterNextDestination, 1f);
        }
    }

    private void Start()
    {
        CharacterNavmeshAgent = GetComponent<NavMeshAgent>();
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
        WasRandomWanderDestinationGenerated = false;
        WasPremadeTargetDestinationPicked = false;
    }

    #endregion

    #region Wander

    public void WanderStateBehaviour()
    {
        if (!WasRandomWanderDestinationGenerated)
        {
            GenerateNewRandomDestination();
            WasRandomWanderDestinationGenerated = true;
        }
        MoveTo();
        if (IsDestinationReached())
        {
            CurrentCharacterState = CharacterStates.Idle;
        }
    }

    private void GenerateNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * MaxWanderDistance;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, MaxWanderDistance, 1);
        CharacterNextDestination = LerpTowardsMiddleOfTheBoard(hit.position);
    }

    private Vector3 LerpTowardsMiddleOfTheBoard(Vector3 TargetPosition)
    {
        float CenterWeight = Mathf.Pow(Random.Range(0.7f, 1f), CenterWeightPower);
        return Vector3.Lerp(Vector3.zero, TargetPosition, CenterWeight);
    }

    #endregion

    #region WalkTowards
    public void WalkTowardsStateBehaviour()
    {
        if (!WasPremadeTargetDestinationPicked)
        {
            PickPremadeTargetPosition();
            WasPremadeTargetDestinationPicked = true;
        }
        MoveTo();
        if (IsDestinationReached())
        {
            CurrentCharacterState = CharacterStates.Idle;
        }
    }

    public void PickPremadeTargetPosition()
    {
        int RandomTargetIndex = Random.Range(0, PremadeTargetPositions.Length);
        CharacterNextDestination = PremadeTargetPositions[RandomTargetIndex].transform.position;
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

    private void MoveTo()
    {
        CharacterNavmeshAgent.destination = CharacterNextDestination;
    }

    #endregion


}
