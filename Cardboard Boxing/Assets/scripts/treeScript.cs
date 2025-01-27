using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeScript : MonoBehaviour
{
    float lifespan = 3f;

    // Destroy the tree object after a specified lifespan.
    void Start()
    {
        Destroy(gameObject, lifespan);
    }

    // Update function (currently empty).
    void Update()
    {
        // You can add any necessary logic here if needed.
    }
}
