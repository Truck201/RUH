using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{

    public Vector3 startPoint;

    public float patrolTimer;
    private Animator animator;

    [SerializeField] private float baseTime = 3f;
    [SerializeField] private float toPatrolTimer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        toPatrolTimer = baseTime;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GamePauseManager.Instance.IsPaused) return;
        toPatrolTimer -= Time.deltaTime;
        if (toPatrolTimer <= 0) 
        {
            animator.SetTrigger("IsPatroling");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        toPatrolTimer = 3f;
    }
}
