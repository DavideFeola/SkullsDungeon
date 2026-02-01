using UnityEngine;

public class SniperAnimator : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        
        if (animator == null)
            Debug.LogError("❌ Animator mancante su " + gameObject.name);
    }

    public void UpdateAnimation(Vector2 direction)
    {
        if (animator == null) return;
        
        // Normalizza la direzione
        if (direction.magnitude > 0.01f)
        {
            direction.Normalize();
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
        }
    }
}
