using UnityEngine;
using UnityEngine.AI;

public class CharacterStateBehaviourControler : MonoBehaviour
{
    // WanderAround - walk around between random points, wait a bit in some places, on loop, character can easily enter other behaviours from this one.
    // MoveToObject - Walk towards the set destination. Character interacts with the destination objects
    // MoveToArea - Character wants to walk to a different area. Character enters WanderAround after reaching the area.
    // Interact - Character interacts with point of interest. Characters stays in Interact state for a while. Character can enter all other behaviours from this state.
    // More tbp...

    private NavMeshAgent _characterAgent;
    private Vector3 _position = Vector3.zero;
    [SerializeField]private float _TimeToWaitForNextPoint = 0;

    public BehaviourStates CurrentState = BehaviourStates.WanderAround;
    public float WanderRange;
    public Vector2 WanderWaitBetweenGeneratingNewPoints;
    [Range(1, 10)] public int CenterWeightPower = 2;

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
        WanderAroundBehaviour();
    }

    private void WanderAroundBehaviour()
    {
        if(_TimeToWaitForNextPoint > 0)
        {
            _TimeToWaitForNextPoint -= Time.deltaTime;
        }
        else
        {
            RandomizeWaitTime();
            _characterAgent.SetDestination(GenerateNewRandomDestination());
        }
    }
    
    private void RandomizeWaitTime()
    {
        _TimeToWaitForNextPoint = Random.Range(WanderWaitBetweenGeneratingNewPoints.x, WanderWaitBetweenGeneratingNewPoints.y);
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

}
