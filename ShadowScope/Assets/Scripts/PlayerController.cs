using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    PhotonView pv;
    public Transform firePoint;
    public Transform laserFirePoint;

    float horizontal, vertical;
    float moveLimiter = 0.7f; // limit diagonal speed
    float rotationSpeed = 100f;

    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    public bool sprinting = false;
    const float maxHealth = 100f;
    float currHealth = maxHealth;

    PlayerManager playerManager;

    Vector2 mousePos;

    //Laserstuff:
    public float fireRate = 0.25f;
    public float weaponRange = 10000f;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private LineRenderer laserLine;
    private float nextFire;


    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        pv = GetComponent<PhotonView>();

        // gets player manager
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        if (!pv.IsMine) // destroy cameras for all other players?
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(body); // this prevents glitchy movement by destroying RigidBody calculations for all other players.
        }
        walkSpeed = 7.0f;
        sprintSpeed = 15.0f;

        laserLine = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!pv.IsMine) // only let the player control this one?
            return;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        sprinting = Input.GetKey(KeyCode.LeftShift);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetButtonDown("Fire1") && Time.time > nextFire) // Left click fires
        {
            nextFire = Time.time + fireRate; //updating time when player can shoot next

            Shoot();
        }
    }

    void FixedUpdate()
    {
        if (!pv.IsMine) // only let the player control this one?
            return;
        Move();
        Rotate();
    }

    void Rotate()
    {
        Vector2 lookDirection = mousePos;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }


    // Movement mechanics can be reworked, this is just for temporary testing purposes
    void Move()
    {
        if (horizontal != 0 && vertical != 0) // if moving diagonally
        {
            // limit diagonal movement to move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        if (sprinting)
            body.velocity = new Vector2(horizontal * sprintSpeed, vertical * sprintSpeed);
        else
            body.velocity = new Vector2(horizontal * walkSpeed, vertical * walkSpeed);
    }

    void Shoot()
    {
        StartCoroutine(ShotEffect()); //turns laser line on and off
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, mousePos);
        laserLine.SetPosition(0, laserFirePoint.position);//set the start position of the visual effect


        if (hit) // position, direction, output to variable hit, and range
        {
            Debug.Log("Hit something");
            laserLine.SetPosition(1, hit.point);//end position of the laser

            if(hit.collider.tag == "Player")
            {
                Debug.Log("Hit player");
                hit.collider.gameObject.GetComponent<PlayerController>().TakeDamage(10.0f);
            }
        }
        else
        {
            Vector3 lineVector = (firePoint.position + (laserFirePoint.transform.right * weaponRange));
            lineVector.z = 0;
            laserLine.SetPosition(1, lineVector);
        }

    }
    private IEnumerator ShotEffect()
    {
        laserLine.enabled = true;//turn on line renderer

        yield return shotDuration;//wait for shot duration time

        laserLine.enabled = false;//deactivate line renderer once waiting
    }

    public void TakeDamage(float damage)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {

        if (!pv.IsMine)
            return;

        currHealth -= damage;
        Debug.Log("Took damage.");
        Debug.Log("Current HP: " + currHealth);

        if(currHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("You died");
        playerManager.Die();
    }
}
