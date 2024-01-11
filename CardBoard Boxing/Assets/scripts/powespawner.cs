using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powespawner : MonoBehaviour
{
    public GameObject[] myobject;
    float timePassed = 0f;
    public GameObject player;
    public GameObject enemy;

    private bool shouldSpawnPowerups = true;

    void Update()
    {
        if (shouldSpawnPowerups)
        {
            timePassed += Time.deltaTime;
            if (timePassed > 1.5f)
            {
                int randomIndex = Random.Range(0, myobject.Length);
                Vector3 randomspawnposition = new Vector3(Random.Range(-9, 9), 1, Random.Range(-9, 9));
                Instantiate(myobject[randomIndex], randomspawnposition, Quaternion.identity);
                timePassed = 0f;
            }
        }

        // Check if either player or enemy is not active
        if (!player.activeSelf || !enemy.activeSelf)
        {
            // Stop spawning powerups
            shouldSpawnPowerups = false;
        }
    }
}
