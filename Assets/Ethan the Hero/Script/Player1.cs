using System.Collections;
using UnityEngine;

public class Player1 : MonoBehaviour
{
    [Header("Movement and Attack Settings")]
    public float moveSpeed = 2f; // Movement speed of the player
    public float attackRange = 1.5f; // Range at which the player will attack the enemy
    public int damage = 1; // Damage dealt to the enemy

    [Header("Health Settings")]
    [SerializeField] GameObject[] heart; // Array to hold heart GameObjects
    public int maxHealth = 3; // Maximum health

    private int currentHealth;            // Current health of the enemy
    private Animator animator;            // Reference to the Animator component
    private bool isDying = false;         // Prevent overlapping actions during death
    private bool isDead = false;          // Flag to check if the player is dead

    private Transform enemy;
    private Vector3 startPosition;
    private bool isAttacking = false; // Prevents overlapping actions

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Find the enemy in the scene (ensure Enemy1 tag is set on the enemy)
        enemy = GameObject.FindGameObjectWithTag("Enemy1")?.transform;
        startPosition = transform.position;

        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead || isAttacking) return; // Prevent movement and attacking if the player is dead or attacking
        if (enemy == null) return; // Ensure the enemy still exists

        if (Input.GetKeyDown(KeyCode.Y)) // Trigger attack
        {
            StartCoroutine(MoveAndAttackSequence());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // Prevent taking damage if the player is dead

        Debug.Log($"Player takes {damage} damage!");

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0

        UpdateHealthUI();

        if (currentHealth > 0)
        {
            TriggerHurtAnimation(); // Trigger the Hurt animation
        }
        else
        {
            TriggerDeathAnimation(); // Trigger the Death sequence
        }
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < heart.Length; i++)
        {
            heart[i].SetActive(i < currentHealth);
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Optional: Trigger death animation here if needed
        Destroy(gameObject);
    }

    void MoveTowardsEnemy()
    {
        Vector3 targetPosition = new Vector3(enemy.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        animator.SetBool("isMoving", true);
    }

    void StartAttack()
    {
        if (isDead) return; // Prevent attack if the player is dead

        animator.SetBool("isMoving", false);
        animator.SetBool("isAttacking", true);
        Debug.Log("Player is attacking!"); // Debug for attack
        StartCoroutine(AttackAndRetreat());
    }

    IEnumerator AttackAndRetreat()
    {
        isAttacking = true;

        // Wait for the attack animation to complete
        yield return new WaitForSeconds(0.5f);

        // Apply damage if the enemy is in range
        if (enemy != null && Vector3.Distance(transform.position, enemy.position) <= attackRange)
        {
            Enemy1 enemyScript = enemy.GetComponent<Enemy1>();
            if (enemyScript != null)
            {
                Debug.Log("Player hits the enemy!"); // Debug for hitting the enemy
                enemyScript.TakeDamage(damage); // Call enemy's TakeDamage method
            }
        }

        // Stop attack animation
        animator.SetBool("isAttacking", false);

        // Move back to starting position
        while (Vector3.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("isMoving", false);
        isAttacking = false;
    }

    IEnumerator MoveAndAttackSequence()
    {
        while (Vector3.Distance(transform.position, enemy.position) > attackRange)
        {
            MoveTowardsEnemy();
            yield return null;
        }

        StartAttack();
    }

    void TriggerHurtAnimation()
    {
        Debug.Log("Player is hurt!");
        animator.SetBool("isHurt", true); // Set the Hurt bool to true

        // Reset the Hurt animation after a short delay
        StartCoroutine(ResetHurtAnimation());
    }

    System.Collections.IEnumerator ResetHurtAnimation()
    {
        yield return new WaitForSeconds(0.10f); // Adjust based on your Hurt animation length
        animator.SetBool("isHurt", false); // Reset the Hurt bool
    }

    void TriggerDeathAnimation()
    {
        if (isDying || isDead) return;

        Debug.Log("Player has died!");
        isDying = true;
        isDead = true; // Mark the player as dead

        animator.SetBool("isDead", true); // Set the Death bool to true

        // Wait for the death animation to finish before destroying the object
        StartCoroutine(DeathSequence());
    }

    System.Collections.IEnumerator DeathSequence()
    {
        // Wait for death animation length (adjust duration to match animation)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Die(); // Call Die method to destroy the player
    }
}
