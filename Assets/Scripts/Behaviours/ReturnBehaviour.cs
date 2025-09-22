using UnityEngine;
using UnityEngine.AI;

public class ReturnBehaviour : StateMachineBehaviour
{
    [SerializeField] private float returnSpeed;
    private Vector3 starterPoint;
    private Rigidbody2D rigidbody;
    private EnemyMovement movement;
    private float offset = 0.2f;
    private float difference;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        movement = animator.gameObject.GetComponent<EnemyMovement>();
        starterPoint = movement.startPoint;

        NavMeshAgent agent = movement.GetComponent<NavMeshAgent>();
        if (agent)
        {
            agent.isStopped = false;
            agent.SetDestination(starterPoint);
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        NavMeshAgent agent = movement.GetComponent<NavMeshAgent>();
        if (GamePauseManager.Instance.IsPaused) return;
        // animator.transform.position = Vector2.MoveTowards(animator.transform.position, starterPoint, returnSpeed * Time.deltaTime);
        //movement.Flip(starterPoint);

        float distance = Vector2.Distance(animator.transform.position, starterPoint);
        if (distance <= offset) 
        {
            if (agent) agent.isStopped = true;
            animator.ResetTrigger("isReturned");
            animator.SetTrigger("isReturned");
        }
    }
}
