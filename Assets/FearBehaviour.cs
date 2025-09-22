using UnityEngine;

public class FearBehaviour : StateMachineBehaviour
{
    private Transform playerPos;
    private Transform enemyPos;

    [Header("Fear Settings")]
    public float fearRadius = 6f;       // Radio en el que el enemigo empieza a tener miedo
    public float minSafeDistance = 3.3f; // Distancia crítica donde siempre huye
    public float speed = 3.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        enemyPos = animator.transform;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerPos == null) return;

        float distanceToPlayer = Vector2.Distance(enemyPos.position, playerPos.position);

        if (distanceToPlayer <= fearRadius)
        {
            if (distanceToPlayer > minSafeDistance)
            {
                // Está en rango de miedo pero aún a salvo → puede atacar
                animator.SetTrigger("Charge");
            }
            else
            {
                // Está demasiado cerca → huir
                Vector2 direction = (enemyPos.position - playerPos.position).normalized;
                enemyPos.position = Vector2.MoveTowards(
                    enemyPos.position,
                    enemyPos.position + (Vector3)direction,
                    speed * Time.deltaTime
                );
                Debug.Log("¡Huida!");
            }
        }
        else
        {
            // Está fuera del radio de miedo → Idle o Patrol
            animator.SetTrigger("toIdle");
        }
    }
}
