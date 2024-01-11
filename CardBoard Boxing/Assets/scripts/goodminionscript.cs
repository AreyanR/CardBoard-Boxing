using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class goodminionscript : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent minionNavMesh;
    public Transform enemytransform;
    private Rigidbody rb;
    public float health;
    private float damage;
    public enemyscript enemy;

    private float lifespan = 8f;
    public AudioSource hit;
    public AudioSource spawnpop;
    public float explosionForce = 10f;
    public float explosionRadius = 5f;

    public AudioSource shorterexplosionsound;

    public GameObject exp;
    void Start()
    {
        health = 20;
        damage = 2.5f;
        Destroy(gameObject, lifespan);
        spawnpop.Play();

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (enemytransform != null)
        {
            MoveTowardsEnemy();
        }

        if (IsHealthDepleted())
        {
            DeactivateMinion();
        }
    }

    private void MoveTowardsEnemy()
    {
        minionNavMesh.SetDestination(enemytransform.position);
    }

    private bool IsHealthDepleted()
    {
        return health <= 0;
    }

    private void DeactivateMinion()
    {
        gameObject.SetActive(false);
        health = 0;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            hit.Play();
            if (enemy != null)
            {
                enemy.health -= damage;
            }
        }
        else if (other.CompareTag("bad minion"))
        {
            hit.Play();
            badminionscript otherMinion = other.GetComponent<badminionscript>();
            if (otherMinion != null)
            {
                otherMinion.health -= damage;
            }
        }
        else if (other.CompareTag("landmine"))
        {
            other.gameObject.SetActive(false);
            Explode();
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

            health -= 5;
        }

        shorterexplosionsound.Play();
        Instantiate(exp, transform.position, transform.rotation);
    }
}
