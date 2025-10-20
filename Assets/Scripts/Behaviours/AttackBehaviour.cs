using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    private Transform enemyPos;
    private Vector3 targetPos;
    private Vector3 direction;
    public float speed = 15f;
    public float deceleration = 0.95f;
    public bool isCarrot = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Guardar posición del enemigo y del jugador al inicio
        enemyPos = animator.transform;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            targetPos = playerObj.transform.position;
            direction = (targetPos - enemyPos.position).normalized;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyPos == null) return;
        if (GamePauseManager.Instance.IsPaused) return;

        if (Vector2.Distance(enemyPos.position, targetPos) > 0.01f)
        {
            // Mover al enemigo hacia la dirección
            enemyPos.position += direction * speed * Time.deltaTime;
            //Debug.Log("Attack");
            // Desacelerar progresivamente
            speed *= deceleration;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isCarrot)
        {
           speed = 25f;
        }
    }
}
