using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChaseBehaviour : StateMachineBehaviour
{
    private Transform playerPos;
    private Transform enemyPos;
    public float speed;
    public float chargeDistance = 3f;
    [SerializeField] EnemyMovement enemyMovement;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        enemyPos = animator.transform;
        enemyMovement = animator.GetComponent<EnemyMovement>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerPos == null) return;
        if (GamePauseManager.Instance.IsPaused) return;
        if (enemyMovement.stunned) return;
        enemyPos.position = Vector2.MoveTowards(enemyPos.position, playerPos.position, speed * Time.deltaTime);

        float distanceToPlayer = Vector2.Distance(enemyPos.position, playerPos.position);
        if (distanceToPlayer <= chargeDistance)
        {
            Debug.Log($"Charge!! {distanceToPlayer}");
            animator.SetTrigger("Charge");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
