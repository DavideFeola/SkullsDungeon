using UnityEngine;

public class MeleeEnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector2 lastMoveDirection;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        lastMoveDirection = Vector2.down;
    }
    
    public void UpdateAnimation(Vector2 moveDirection)
    {
        if (moveDirection.magnitude > 0.1f)
        {
            lastMoveDirection = moveDirection.normalized;
        }
        
        if (Mathf.Abs(lastMoveDirection.x) > Mathf.Abs(lastMoveDirection.y))
        {
            if (lastMoveDirection.x > 0)
            {
                animator.Play("Enemy_Walk_Right");
            }
            else
            {
                animator.Play("Enemy_Walk_Left");
            }
        }
        else
        {
            if (lastMoveDirection.y > 0)
            {
                animator.Play("Enemy_Walk_Up");
            }
            else
            {
                animator.Play("Enemy_Walk_Down");
            }
        }
    }
}