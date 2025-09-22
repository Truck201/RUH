using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{
    private EnemyMovement enemy;
    private Transform[] patrolPoints;
    public float speed;
    private int randomSpot;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<EnemyMovement>();
        patrolPoints = enemy.patrolPoints;

        if (patrolPoints != null && patrolPoints.Length > 0)
            randomSpot = Random.Range(0, patrolPoints.Length);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (GamePauseManager.Instance.IsPaused) return;
        if (Vector2.Distance(animator.transform.position, patrolPoints[randomSpot].position) > 0.2f)
        {
            animator.transform.position = Vector2.MoveTowards(animator.transform.position, patrolPoints[randomSpot].position, speed * Time.deltaTime);

        }
        else
        {
            Debug.Log("Returned to idle");
            animator.SetTrigger("toIdle");
        }
    }
}
