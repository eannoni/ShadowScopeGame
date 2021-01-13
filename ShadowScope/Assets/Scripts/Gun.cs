using UnityEngine;

public class Gun : MonoBehaviour
{

    public float damage = 10f;
    public float range = 10000f;

    public Camera fpsCam;
    Vector2 mousePos;
    float rotationSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // Left click fires
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit)) // position, direction, output to variable hit, and range
        {
            // Section does not work as far as I've tested.
            Debug.Log(hit.transform.name); // Testing if a raycast has hit another object with a collider.
        }

        Debug.Log(fpsCam.transform.forward); // Prints direction fired

        Debug.Log("Fired a shot"); // This works if I left-click

        // Copied below from PlayerController for rotation

        Vector2 lookDirection = mousePos;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);  
        Debug.Log(angle); // To see if the shooting is changing direction
        //Debug.Log(transform.rotation);
        //Debug.Log(rotation);
    }
}
