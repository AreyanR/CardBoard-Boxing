using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class goodminionscript : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent minionNavMesh; // Reference to the NavMeshAgent component
    public Transform enemytransform; // Transform of the enemy to move towards
    private Rigidbody rb; // Reference to the Rigidbody component
    public float health; // Current health of the minion
    private float damage; // Amount of damage this minion deals
    public enemyscript enemy; // Reference to the enemy script

    private float lifespan = 8f; // How long the minion will live
    public AudioSource hit; // Audio source for taking damage
    public AudioSource spawnpop; // Audio source for spawning
    public float explosionForce = 10f; // Force of the explosion when the minion explodes
    public float explosionRadius = 5f; // Radius of the explosion

    public AudioSource shorterexplosionsound; // Audio source for shorter explosions

    public GameObject exp; // Explosion particle effect

    void Start()
    {
        health = 20;
        damage = 2.5f;
        Destroy(gameObject, lifespan); // Destroy the minion after its lifespan
        spawnpop.Play(); // Play the spawning audio

        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
    }

    void Update()
    {
        if (enemytransform != null)
        {
            MoveTowardsEnemy(); // Move the minion towards the enemy
        }

        if (IsHealthDepleted())
        {
            DeactivateMinion(); // Deactivate the minion if its health is depleted
        }
    }

    private void MoveTowardsEnemy()
    {
        minionNavMesh.SetDestination(enemytransform.position); // Set the destination for NavMeshAgent
    }

    private bool IsHealthDepleted()
    {
        return health <= 0; // Check if the minion's health is depleted
    }

    private void DeactivateMinion()
    {
        gameObject.SetActive(false); // Deactivate the minion
        health = 0; // Set health to 0
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse); // Apply knockback force to the minion
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            hit.Play(); // Play the hit audio
            if (enemy != null)
            {
                enemy.health -= damage; // Deal damage to the enemy
            }
        }
        else if (other.CompareTag("bad minion"))
        {
            hit.Play(); // Play the hit audio
            badminionscript otherMinion = other.GetComponent<badminionscript>();
            if (otherMinion != null)
            {
                otherMinion.health -= damage; // Deal damage to another bad minion
            }
        }
        else if (other.CompareTag("landmine"))
        {
            other.gameObject.SetActive(false); // Deactivate the landmine
            Explode(); // Trigger the minion's explosion
        }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            Rigidbody targetRigidbody = collider.GetComponent<Rigidbody>();
            if (targetRigidbody != null)
            {
                Vector3 explosionDirection = collider.transform.position - transform.position;
                float distance = explosionDirection.magnitude;

                float force = 1 - (distance / explosionRadius);
                force = Mathf.Clamp01(force);
                force *= explosionForce;

                targetRigidbody.AddForce(explosionDirection.normalized * force, ForceMode.Impulse);
            }

            health -= 5; // Reduce the minion's health due to the explosion
        }

        shorterexplosionsound.Play(); // Play the shorter explosion audio
        Instantiate(exp, transform.position, transform.rotation); // Spawn the explosion particle effect
    }
}
