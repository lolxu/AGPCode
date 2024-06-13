using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePointFacePlayer : MonoBehaviour
{
    public Transform rotatablePart;
    public Transform player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // Calculate the direction vector
            Vector3 direction = player.position - rotatablePart.position;
            direction.y = 0; // This ensures the rotation is only around the Y axis

            // Create a rotation based on the direction pointing towards the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Calculate lerped y rotation
            float yRotation = Mathf.LerpAngle(rotatablePart.rotation.eulerAngles.y, targetRotation.eulerAngles.y, Time.deltaTime * 5);

            // Set the GameObject's rotation to the newly created rotation
            // Only change the y rotation, keep x and z as they are
            rotatablePart.rotation = Quaternion.Euler(rotatablePart.rotation.eulerAngles.x, yRotation,
                rotatablePart.rotation.eulerAngles.z);
        }
    }
}
