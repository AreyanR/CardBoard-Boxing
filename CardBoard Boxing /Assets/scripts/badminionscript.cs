using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class badminionscript : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent minionNavMesh;
    public Transform playertransform;
    private Rigidbody rb;
    public float health;
    private float damage;
    public playercontroller player;

    private float lifespan = 20f;
    public AudioSource hit;
    public AudioSource spawnpop;

    public GameObject exp;
    public float explosionForce = 10f;
    public float explosionRadius = 5f;

    public AudioSource shorterexplosionsound;

    // Initialize health, damage, and destroy the object after a specified lifespan.
    void Start()
    {
        health = 20;
        damage = 2.5f;
        Destroy(gameObject, lifespan);
        spawnpop.Play();
    }

    // Update the minion's navigation towards the player's position.
    void Update()
    {
        minionNavMesh.SetDestination(playertransform.position);

        if (health <= 0)
        {
            health = 0;
            gameObject.SetActive(false);
        }
    }

    // Apply knockback force to the minion.
    public void ApplyKnockback(Vector3 direction, float force)
    {
        Rigidbody minionRigidbody = GetComponent<Rigidbody>();
        minionRigidbody.AddForce(direction * force, ForceMode.Impulse);
    }

    // Handle collisions with other objects.
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("box"))
        {
            hit.Play();
            player.health -= damage;
        }
        else if (other.CompareTag("good minion"))
        {
            hit.Play();
            other.GetComponent<goodminionscript>().health -= damage;
        }
        else if (other.CompareTag("landmine"))
        {
            other.gameObject.SetActive(false);
            Explode();
        }
    }

    // Explode the minion, applying force to nearby objects and reducing its health.
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

            health -= 5;
        }

        shorterexplosionsound.Play();
        Instantiate(exp, transform.position, transform.rotation);
    }
}
