using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeScript : MonoBehaviour
{

    float lifespan = 3f;
    void Start()
    {
        Destroy(gameObject, lifespan);
        
    }

    void Update()
    {
        
    }
}
