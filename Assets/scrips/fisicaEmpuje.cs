using System.Collections;
using UnityEngine;

public class fisicaEmpuje : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float pushMultiplier = 0.8f;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
    }

   
    public void ApplyPush(Vector2 direction, int resultadoCartas)
    {
        if (rb != null)
        {
            float finalForce = resultadoCartas * pushMultiplier;
            rb.AddForce(direction * finalForce, ForceMode2D.Impulse);
        }
    }

   
    public void ExecuteFastAttack(Vector2 direction, float distance = 1.2f, float duration = 0.25f)
    {
        StartCoroutine(RoutineFastAttack(direction, distance, duration));
    }

    private IEnumerator RoutineFastAttack(Vector2 direction, float distance, float duration)
    {
        
        if (animator != null)
        {
            animator.Play("ataque_jugador", -1, 0f);
        }

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (Vector3)(direction.normalized * distance);

        float elapsed = 0f;
        float halfDuration = duration / 2f;

        
        while (elapsed < halfDuration)
        {
            Vector3 nextPos = Vector3.Lerp(startPosition, targetPosition, elapsed / halfDuration);
            if (rb != null) rb.MovePosition(nextPos);
            else transform.position = nextPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

       
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            Vector3 nextPos = Vector3.Lerp(targetPosition, startPosition, elapsed / halfDuration);
            if (rb != null) rb.MovePosition(nextPos);
            else transform.position = nextPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        
        if (rb != null) rb.MovePosition(startPosition);
        else transform.position = startPosition;
    }
}