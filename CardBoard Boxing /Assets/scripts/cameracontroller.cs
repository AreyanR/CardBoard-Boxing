using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracontroller : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset;

    // Initialize the offset between the camera and the player.
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update the camera's position to follow the player with the specified offset.
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}
