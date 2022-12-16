using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class CharacterStateBehaviourControler : MonoBehaviour
{
    // WanderAround - walk around between random points, wait a bit in some places, on loop, character can easily enter other behaviours from this one.
    // MoveToObject - Walk towards the set destination. Character interacts with the destination objects
    // MoveToArea - Character wants to walk to a different area. Character enters WanderAround after reaching the area.
    // Interact - Character interacts with point of interest. Characters stays in Interact state for a while. Character can enter all other behaviours from this state.
    // More TBA...

    private NavMeshAgent _characterAgent;
    [SerializeField]private float _timeToWaitForNextPoint = 0;
    [SerializeField] private Vector3 _moveToObjectTarget;
    private bool WasMoveToObjectTargetPicked = false;

    public BehaviourStates CurrentState = BehaviourStates.WanderAround;
    public float WanderRange;
    public Vector2 WanderWaitBetweenGeneratingNewPoints;
    [Range(1, 10)] public int CenterWeightPower = 2;
    public InteractableRegistryGenerator InteractableRegistryGenerator;

    public enum BehaviourStates
    {
        WanderAround,
        MoveToObject,
        MoveToArea,
        Interact
    }

    private void Start()
    {
        _characterAgent = GetComponent<NavMeshAgent>();
        _characterAgent.SetDestination(GenerateNewRandomDestination());
    }

    private void Update()
    {
        switch(CurrentState)
        {
            case BehaviourStates.WanderAround:
                WanderAroundBehaviour();
                break;
            case BehaviourStates.MoveToObject:
                MoveToObjectBehaviour();
                break;
        }
    }

    public void ChangeState(BehaviourStates newState)
    {
        if (CurrentState == newState)
            return;
        CurrentState = newState;
        ResetTimersAndBools();
    }

    private void ResetTimersAndBools()
    {
        _timeToWaitForNextPoint = 0;
        WasMoveToObjectTargetPicked = false;
    }

    private void WanderAroundBehaviour()
    {
        if(_timeToWaitForNextPoint > 0)
        {
            _timeToWaitForNextPoint -= Time.deltaTime;
        }
        else
        {
            RandomizeWaitTime();
            _characterAgent.SetDestination(GenerateNewRandomDestination());
        }
    }

    private void MoveToObjectBehaviour()
    {
        bool destinationReached = false;
        if(WasMoveToObjectTargetPicked)
        {
            destinationReached = WasDestinationReached();
        }
        else
        {
            WasMoveToObjectTargetPicked = true;
            _moveToObjectTarget = PickRandomPresetTarget();
            _characterAgent.SetDestination(_moveToObjectTarget);   
        }

        if(destinationReached)
        {
            ChangeState(BehaviourStates.WanderAround);
        }
    }
    
    private void RandomizeWaitTime()
    {
        _timeToWaitForNextPoint = Random.Range(WanderWaitBetweenGeneratingNewPoints.x, WanderWaitBetweenGeneratingNewPoints.y);
    }

    private Vector3 GenerateNewRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * WanderRange;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, WanderRange, 1);
        return LerpTowardsMiddleOfTheBoard(hit.position);
    }

    private Vector3 LerpTowardsMiddleOfTheBoard(Vector3 TargetPosition)
    {
        float CenterWeight = Mathf.Pow(Random.Range(0.7f, 1f), CenterWeightPower);
        return Vector3.Lerp(Vector3.zero, TargetPosition, CenterWeight);
    }

    private Vector3 PickRandomPresetTarget()
    {
        int r = Random.Range(0, InteractableRegistryGenerator.InteractablesRegistry.Count);

        return InteractableRegistryGenerator.InteractablesRegistry[r].transform.position;
    }

    private bool WasDestinationReached()
    {
        bool isCompleted = (_characterAgent.pathStatus == NavMeshPathStatus.PathComplete);
        bool isLengthZero = (_characterAgent.remainingDistance == 0);
        return (isCompleted && isLengthZero);
    }
}
