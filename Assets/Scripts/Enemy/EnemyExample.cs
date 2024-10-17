using UnityEngine;
using UnityEngine.AI;

public class EnemyExample : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform target;

    private void Update() {
        agent.SetDestination(target.position);
    }
}
