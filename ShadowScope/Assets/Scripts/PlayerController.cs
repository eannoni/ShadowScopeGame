using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    CircleCollider2D circleCollider;
    PhotonView pv;
    PlayerManager playerManager;
    ScoreManager scoreManager;
    public GameObject sightLight;
    public GameObject moveLight; // Light for when you're moving

    [Header("Particle Effects")]
    public ParticleSystem shootEffect;
    public ParticleSystem hitEffect;

    [Header("HUD Components")]
    public HealthBar healthBar;
    public TMP_Text userName;
    public GameObject ammoDisplay;

    [Header("Shooting")]
    public Transform firePoint;
    public Transform laserFirePoint;
    public LineRenderer laserLinePrefabRed;
    public LineRenderer laserLinePrefabBlue;

    float horizontal, vertical;
    float moveLimiter = 0.7f; // limit diagonal speed
    float rotationSpeed = 100f;

    [Header("Sprites")]
    public Sprite redTeam;
    public Sprite blueTeam;

    [Header("Movement")]
    [SerializeField] float walkSpeed;
    [SerializeField] float crouchSpeed;
    public bool crouching = false;

    [Header("Health")]
    [SerializeField] const int maxHealth = 4;
    [SerializeField] int currHealth = maxHealth;
    bool isDead = false;

    [Header("Ammo")]
    [SerializeField] const int maxAmmo = 25;
    [SerializeField] int currAmmo = 10;

    [Header("Shooting")]
    public int damage = 1;
    public float fireRate = 0.3f;
    public float weaponRange = 1000f;
    public GameObject muzzleFlash; // Light for muzzle flash
    float shotDuration = 0.05f; // how long the bullet trail is enabled
    float nextFire; // amount of time before next fire is allowed
    Vector2 mousePos;

    [Header("Sounds")]
    public AudioClip[] ammoSounds;
    public AudioClip[] deathSounds;
    public AudioClip[] healthSounds;
    public AudioClip[] hurtSounds;
    public AudioClip[] shootSounds;
    public AudioClip healthPickup;
    public AudioClip ammoPickup;
    AudioSource source;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        pv = GetComponent<PhotonView>();

        // gets player manager
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();

        scoreManager = GameObject.FindGameObjectWithTag("ScoreManager").GetComponent<ScoreManager>();
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (pv.IsMine)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.Show();
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(body); // this prevents glitchy movement by destroying RigidBody calculations for all other players.
            healthBar.gameObject.SetActive(false);
            ammoDisplay.gameObject.SetActive(false);
        }

        SetSprite();
        SetUserName();
        UpdateAmmoDisplay();

        walkSpeed = 10f;
        crouchSpeed = 3.5f;
    }

    void Update()
    {
        if (!pv.IsMine) // only let the player control this one?
            return;

        if (scoreManager.IsWinner() == -1 && !isDead)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            crouching = Input.GetKey(KeyCode.LeftShift);

            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetButtonDown("Fire1") && Time.time > nextFire && currAmmo > 0) // Left click fires
            {
                //shoot sound
                source.PlayOneShot(shootSounds[0]);
                nextFire = Time.time + fireRate; //updating time when player can shoot next
                currAmmo--;
                UpdateAmmoDisplay();
                Shoot();
            }
        }
    }

    void FixedUpdate()
    {
        if (!pv.IsMine) // only let the player control this one
            return;

        if (!isDead)
        {
            Move();
            Rotate();
        }
    }

    public void CollectedHealthPickup(int id)
    {
        FullHeal();
        source.PlayOneShot(healthPickup);
        source.PlayOneShot(healthSounds[Random.Range(0, healthSounds.Length)]);
        pv.RPC("RPC_DeactivatePickup", RpcTarget.All, id);
    }

    public void CollectedAmmoPickup(int id, int ammoPickupAmount)
    {
        GetAmmo(ammoPickupAmount);
        source.PlayOneShot(ammoPickup);
        source.PlayOneShot(ammoSounds[Random.Range(0, ammoSounds.Length)]);
        pv.RPC("RPC_DeactivatePickup", RpcTarget.All, id);
    }

    public void FullHeal()
    {
        currHealth = maxHealth;
        healthBar.SetHealth(currHealth);
    }

    public void GetAmmo(int ammo)
    {
        currAmmo += ammo;
        if (currAmmo > maxAmmo)
            currAmmo = maxAmmo;
        UpdateAmmoDisplay();
    }

    [PunRPC]
    void RPC_DeactivatePickup(int id)
    {
        PickupManager.Instance.DeactivatePickup(id);
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

        moveLight.SetActive(true);
        if (horizontal == 0 && vertical == 0)
        {
            moveLight.SetActive(false);
        }

        if (crouching)
            body.velocity = new Vector2(horizontal * crouchSpeed, vertical * crouchSpeed);
        else
            body.velocity = new Vector2(horizontal * walkSpeed, vertical * walkSpeed);
    }

    void MuzzleStop()
    {
        muzzleFlash.SetActive(false);
    }

    void Shoot()
    {
        Vector3 startPoint; // holds start point info that will be sent to all other clients
        Vector3 endPoint; // holds end point info that will be sent to all other clients
        
        // create raycast
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, mousePos);

        // assign startPoint
        startPoint = laserFirePoint.position;

        //muzzleFlash.SetActive(true); // Turns on muzzle flash // Moved to "ShootLine"
        //Invoke("MuzzleStop", 0.05f); // Flash duration

        if (hit) // if hit something
        {
            // hit a player?
            if (hit.collider.tag == "Player")
            {
                PlayerController hitPC = hit.collider.gameObject.GetComponent<PlayerController>();

                // hit someone on other team?
                if (hitPC.playerManager.team != playerManager.team)
                {
                    // deal damage
                    hitPC.TakeDamage(userName.text);
                }
            }

            // assign endPoint
            endPoint = hit.point;
        }
        else // if didn't hit anything
        {
            // assign endPoint to weapon's range
            endPoint = (firePoint.position + (laserFirePoint.transform.right * weaponRange));
            endPoint.z = 0;
        }

        // animate line
        ShootLine(startPoint, endPoint);
    }


    // tells all clients to draw a line from startPos to endPos
    public void ShootLine(Vector3 startPos, Vector3 endPos)
    {
        pv.RPC("RPC_ShootLine", RpcTarget.All, startPos, endPos);
    }

    [PunRPC]
    void RPC_ShootLine(Vector3 startPos, Vector3 endPos)
    {
        ShotEffect(startPos, endPos);
        muzzleFlash.SetActive(true);
        Invoke("MuzzleStop", 0.05f);
    }

    // draws line from startPos to endPos for a given amount of time
    private void ShotEffect(Vector3 startPos, Vector3 endPos)
    {
        // play shoot particle effect
        shootEffect.Play();
        // instantiate and initialize LineRenderer prefab
        if (playerManager.team == 0)
        {
            LineRenderer laserGO = Instantiate(laserLinePrefabRed);
            laserGO.SetPosition(0, startPos);
            laserGO.SetPosition(1, endPos);
            Destroy(laserGO.gameObject, shotDuration);
        }
        else
        {
            LineRenderer laserGO = Instantiate(laserLinePrefabBlue);
            laserGO.SetPosition(0, startPos);
            laserGO.SetPosition(1, endPos);

            // destroy instantiated LineRenderer
            Destroy(laserGO.gameObject, shotDuration);
        }
    }



    public void TakeDamage(string killerName)
    {
        pv.RPC("RPC_TakeDamage", RpcTarget.All);
    }

    [PunRPC]
    void RPC_TakeDamage()
    {
        hitEffect.Play();
        if (!pv.IsMine)
            return;
        currHealth -= damage;
        healthBar.SetHealth(currHealth);
        if (currHealth > 0)
            source.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);

        if (currHealth <= 0)
        {
            DeactivateBeforeDeath();
            pv.RPC("RPC_DieInstantlyOnOthers", RpcTarget.Others);
            Die();
        }
    }

    [PunRPC]
    void RPC_DieInstantlyOnOthers()
    {
        gameObject.SetActive(false);
    }

    void SetSprite()
    {
        if (playerManager.team == 0)
            GetComponent<SpriteRenderer>().sprite = redTeam;
        else if (playerManager.team == 1)
            GetComponent<SpriteRenderer>().sprite = blueTeam;
        else
            Debug.LogError("ERROR: unknown team number, cannot assign player sprite");
    }

    void SetUserName()
    {
        userName.text = pv.Owner.NickName;
    }

    void UpdateAmmoDisplay()
    {
        ammoDisplay.GetComponentInChildren<TMP_Text>().text = "x" + currAmmo;
    }

    void Die()
    {
        source.PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length)]); //play a random death sound from the array
        playerManager.Die();
    }

    void DeactivateBeforeDeath()
    {
        isDead = true;
        healthBar.gameObject.SetActive(false);
        userName.gameObject.SetActive(false);
        ammoDisplay.gameObject.SetActive(false);
        moveLight.SetActive(false);
        sightLight.SetActive(false);
        body.Sleep();
        circleCollider.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
