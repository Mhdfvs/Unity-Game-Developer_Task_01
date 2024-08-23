using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = -9.81f;

    // Coin 
    public int coinsCollected = 0;
    public int totalCoins; // Set this to the number of coins in the scene

    public Text coinCountText;  // Reference to the ItemCountText UI element.
    public Text collectionCompleteText;  // Reference to the CollectionCompleteText UI element.

    // Health
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthBar;
    public Animator playerAnimator; // For death animation
    public GameObject gameOverUI;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        // Initialize the UI with the current count.
        UpdateCoinCountText();
        collectionCompleteText.enabled = false;  // Hide the "Collection Complete" text initially.

        currentHealth = maxHealth;
        healthBar.value = CalculateHealth();
    }

    private void Update()
    {
        MovePlayer();
        HandleGravity();
        Jump();
    }

    private void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed;

        Vector3 moveDir = transform.right * moveX + transform.forward * moveZ;
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void HandleGravity()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // Coin Collection
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Collectible"))
        {
            // Destroy the collectible object.
            Destroy(other.gameObject);

            // Increment the item count.
            coinsCollected++;

            // Update the UI.
            UpdateCoinCountText();

            // Check if all items are collected.
            if (coinsCollected >= totalCoins)
            {
                collectionCompleteText.enabled = true;  // Show "Collection Complete" message.
            }
            if (other.CompareTag("Hazard"))
            {
                TakeDamage(20); // Example damage value
            }
        }
    }

    // Update the item count UI text.
    private void UpdateCoinCountText()
    {
        coinCountText.text = "Coins Collected: " + coinsCollected.ToString() + "/" + totalCoins.ToString();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.value = CalculateHealth();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    float CalculateHealth()
    {
        return (float)currentHealth / maxHealth;
    }

    void Die()
    {
        // Disable player movement
        GetComponent<PlayerController>().enabled = false;

        // Play death animation (if available)
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Optionally, show a Game Over UI (enable a hidden UI element)
        gameOverUI.SetActive(true);
    }
}
