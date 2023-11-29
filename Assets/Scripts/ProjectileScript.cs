using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ProjectileScript : MonoBehaviour
{
    PlayerStateManager stateManager;
    public Transform firingPoint; // The 3D firing point transform
    public GameObject projectilePrefab; // The projectile prefab to instantiate
    public float projectileSpeed = 10f; // Speed of the projectile

    public float steadyFireRate = 0.5f;
    public float individualFireRate = 0.1f;
    private float nextFireTime = 0f;

    Plane plane;

    PlayerMovement movementScript;

    private void Start()
    {
        //create a plane, pass in down for the in-normal, which is the opposite way the plane faces for some reason. apply the players y position as an offset
        plane = new Plane(Vector3.down, transform.position.y);
        stateManager = PlayerStateManager.instance;
        movementScript = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            if (Time.time >= nextFireTime)
            {
                if (movementScript.usingController)
                {
                    FireWithGamepad();
                }
                else
                {
                    FireWithMouse();
                }
                nextFireTime = Time.time + steadyFireRate; // Use steady fire rate when holding
            }
        }
        else
        {
            // Check if the "Fire1" button was clicked (not held)
            if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
            {
                if(movementScript.usingController)
                {
                    FireWithGamepad();
                }
                else
                {
                    FireWithMouse();
                }
                nextFireTime = Time.time + individualFireRate; // Use individual fire rate for clicks
            }
        }
    }

    void FireWithMouse()
    {
        
        // Cast a ray from the mouse position into the game world
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            // Get the point where the ray hits the plane
            Vector3 targetPosition = ray.GetPoint(distance);

            // Preserve the y-coordinate of the firing point
            //targetPosition.y = firingPoint.position.y;

            // Calculate the direction only in the x and z plane, keeping y fixed
            Vector3 direction = (targetPosition - transform.position);
            direction.y = 0; // Keep y-coordinate fixed (flat plane)

            // Normalize the direction
            direction.Normalize();

            // Create a new instance of the projectile
            GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, Quaternion.identity);

            // Get the rigidbody of the projectile (assuming it has one)
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // Set the velocity of the projectile based on the calculated direction and speed
                rb.velocity = direction * projectileSpeed;
            }
            else
            {
                Debug.LogError("Projectile does not have a Rigidbody component!");
            }
        }
    }

    void FireWithGamepad()
    {
        Vector3 direction = movementScript.GetJoystickAimDirection();

        // Create a new instance of the projectile
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(direction, Vector3.up));

        // Get the rigidbody of the projectile (assuming it has one)
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Set the velocity of the projectile based on the calculated direction and speed
            rb.velocity = direction * projectileSpeed;
        }
        else
        {
            Debug.LogError("Projectile does not have a Rigidbody component!");
        }
    }
}
