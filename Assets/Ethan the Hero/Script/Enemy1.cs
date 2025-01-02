using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] GameObject[] heart;  // Array to hold heart GameObjects
    public int maxHealth = 3;             // Maximum health

    private int currentHealth;            // Current health of the enemy
    private Animator animator;            // Reference to the Animator component
    private bool isDying = false;         // Prevent overlapping actions during death

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (isDying) return; // Prevent further actions when dying

        Debug.Log($"Enemy takes {damage} damage!");
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

    void TriggerHurtAnimation()
    {
        Debug.Log("Enemy is hurt!");
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
        if (isDying) return;

        Debug.Log("Enemy has died!");
        isDying = true;

        animator.SetBool("isDead", true); // Set the Death bool to true

        // Wait for the death animation to finish before destroying the object
        StartCoroutine(DeathSequence());
    }

    System.Collections.IEnumerator DeathSequence()
    {
        // Wait for the death animation length (adjust duration to match animation)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Optionally, disable the enemy's collider or movement
        // For example:
        // GetComponent<Collider>().enabled = false;  // Disable the collider
        // GetComponent<Rigidbody>().isKinematic = true;  // Disable physics interactions

        // Destroy the enemy object
        Destroy(gameObject);
    }

    // You may also want to disable attack functionality here
    public void StartAttack()
    {
        if (isDying) return; // Prevent attack if the enemy is dying

        // Proceed with attack logic
        // Your attack logic goes here...
    }
}
