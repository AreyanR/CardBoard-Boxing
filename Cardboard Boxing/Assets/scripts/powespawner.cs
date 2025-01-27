using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powespawner : MonoBehaviour
{
    public GameObject[] myobject; // An array of powerup objects to spawn
    float timePassed = 0f; // Tracks the time elapsed

    public GameObject player; // Reference to the player GameObject
    public GameObject enemy; // Reference to the enemy GameObject

    private bool shouldSpawnPowerups = true; // Flag to control powerup spawning

    void Update()
    {
        if (shouldSpawnPowerups)
        {
            timePassed += Time.deltaTime; // Increment the timePassed based on real-time

            // Check if enough time has passed to spawn a powerup
            if (timePassed > 1.5f)
            {
                // Generate a random index to select a powerup from the array
                int randomIndex = Random.Range(0, myobject.Length);

                // Generate a random spawn position within a specified range
                Vector3 randomspawnposition = new Vector3(Random.Range(-9, 9), 1, Random.Range(-9, 9));

                // Instantiate a random powerup at the generated spawn position with no rotation
                Instantiate(myobject[randomIndex], randomspawnposition, Quaternion.identity);

                timePassed = 0f; // Reset the timer
            }
        }

        // Check if either player or enemy is not active
        if (!player.activeSelf || !enemy.activeSelf)
        {
            // Stop spawning powerups if either the player or enemy is not active
            shouldSpawnPowerups = false;
        }
    }
}
