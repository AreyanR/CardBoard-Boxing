using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class enemyscript : MonoBehaviour
{
    private Rigidbody rb;
    public float health;
    private int damage;
    private int count;
    private Vector3 scaleChange, positionChange;

    public playercontroller playercount;

    public NavMeshAgent enemy;
    public Transform player;
    public Transform powerup;
    public GameObject ridgebox;

    private Rigidbody Rigidbody;

    public TextMeshProUGUI enemyhealth;
    public TextMeshProUGUI enemypower;

    public Text gameover;

    public GameObject playagain;

    public AudioSource hit;

    public AudioSource powerding;

    public float enemySpeed = 3.0f;

    public GameObject quitgame;

    string difficulty;

    [SerializeField] private Image healthbar;

    List<GameObject> powerups = new List<GameObject>();

    bool fuse = false;
    public bool shouldNavigate = true;
    private bool isOnGround = true;

    public Material enemyMaterial;

    public Material enemyfaceMaterial;
    private Color originalColor, orignalFaceColor;

    public Rigidbody playerRidgebody;

    public GameObject exp;
    public float expforce;

    float originalspeed;

    public bool activated;
    public float downForce = 1.0f;

    float knockbackDistance = 3.0f;

    public bool downForceActive = false;
    private bool isRising = false;

    public AudioSource explosion;

    public bool ifPlayerHit = false;

    public AudioSource fusesound;

    public AudioSource shorterexplosionsound;

    public goodminionscript goodminion;

    public Transform enemytransform;

    public badminionscript yourminion;
    public Rigidbody yourminionRigidbody;
    
    public GameObject minionPrefab;
    public AudioSource spawnPop;

    Transform minionspawnpoint;
    Vector3[] spawnOffset = new Vector3[]
    {
        new Vector3(2f, 2f, 0f),
        new Vector3(0f, 2f, 2f),
        new Vector3(-2f, 2f, 0f)
    };

    public GameObject landmine;
    private Vector3 target;

    private Vector3 iceSize;

    public GameObject iceCube;

    public AudioSource healthpickup;

    public AudioSource freezing;
    

    void Start()
    {
        // Initialize original colors and speed
        originalColor = enemyMaterial.color;
        orignalFaceColor = enemyfaceMaterial.color;
        originalspeed = enemySpeed;
        
        // Set initial game state
        quitgame.SetActive(false);
        playagain.SetActive(false);
        gameover.enabled = false;

        // Get Rigidbody and initial knockback distance
        rb = GetComponent<Rigidbody>();
        knockbackDistance = 5.0f;
        Rigidbody = ridgebox.GetComponent<Rigidbody>();

        // Determine difficulty and set initial values accordingly
        difficulty = PlayerPrefs.GetString("Difficulty");

        if (difficulty == "Easy")
        {
            health = 200;
            damage = 20;
            count = 1;
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }
        else if (difficulty == "Normal")
        {
            health = 250;
            damage = 35;
            count = 3;
            transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        }
        else if (difficulty == "Hard")
        {
            health = 400;
            damage = 50;
            count = 3;
            transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        }
        enemy.speed = enemySpeed;
        scaleChange = new Vector3(0.2f, 0.2f, 0.2f);
        setenemyhealth();
        updatehealthbar();
        setenemypower();
    }

    void Update()
    {
        // Handle enemy navigation
        if (isRising)
        {
            enemy.isStopped = true;
        }
        else
        {
            if (isOnGround && shouldNavigate)
            {
                if (fuse)
                {
                    enemy.SetDestination(player.position);
                }
                else
                {
                    if ((count <= playercount.count && health <= playercount.health))
                    {
                        enemy.SetDestination(player.position);
                    }
                    else
                    {
                        initializePowerups();
                        Transform closestPowerup = FindClosestPowerup();
                        if (closestPowerup != null)
                        {
                            enemy.SetDestination(closestPowerup.position);
                        }
                        else
                        {
                            enemy.SetDestination(player.position);
                        }
                    }
                }
            }
        }

        if (health <= 0)
        {
            health = 0;
            gameObject.SetActive(false);
            gameover.enabled = true;
            playagain.SetActive(true);
            quitgame.SetActive(true);
            enemyMaterial.color = originalColor;
            enemyfaceMaterial.color = orignalFaceColor;
            enemySpeed = originalspeed;
        }
        if (playercount.health <= 0)
        {
            enemyMaterial.color = originalColor;
            enemyfaceMaterial.color = orignalFaceColor;
        }

        setenemyhealth();
        updatehealthbar();
        setenemypower();
    }

    // Handle collisions
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            // Handle power-up pickup
            powerding.Play();
            other.gameObject.SetActive(false);
            transform.localScale += scaleChange;
            health += 15;
            damage += 10;
            count += 1;
            knockbackDistance += 1;
            expforce += 10;
            Rigidbody.mass += 1;
            Rigidbody.drag += 1;
            Rigidbody.angularDrag += 1;
            setenemyhealth();
            updatehealthbar();
            setenemypower();
        }

        if (other.gameObject.CompareTag("bombshell pickup"))
        {
            // Handle bombshell pickup
            health += 10;
            other.gameObject.SetActive(false);
            fuse = true;
            StartCoroutine(bombfuse());
        }

        if (other.gameObject.CompareTag("nuke pickup"))
        {
            // Handle nuke pickup
            health += 5;
            downForceActive = true;
            activated = true;
            other.gameObject.SetActive(false);
            StartCoroutine(SmoothRise());
        }

        else if (other.gameObject.CompareTag("spawner pickup"))
        {
            // Handle spawner pickup
            other.gameObject.SetActive(false);
            spawnMinions();
        }

        else if (other.gameObject.CompareTag("landmine pickup"))
        {
            // Handle landmine pickup
            other.gameObject.SetActive(false);
            Vector3 offset = new Vector3(0f, 3f, 0f);
            target = enemytransform.position + offset;
            spawnPop.Play();
            Instantiate(landmine, target, enemytransform.rotation);
        }
        else if (other.gameObject.CompareTag("heal pickup"))
        {
            // Handle heal pickup
            healthpickup.Play();
            other.gameObject.SetActive(false);
            health += 50;
        }
        else if (other.gameObject.CompareTag("landmine"))
        {
            // Handle landmine collision
            other.gameObject.SetActive(false);
            shorterexplosionsound.Play();
            Instantiate(exp, transform.position, transform.rotation);
            knockBack();
            Vector3 knockbackDirection = (target - transform.position).normalized;
            rb.AddForce(knockbackDirection * expforce, ForceMode.Impulse);
            health -= damage + 5;
            GameObject[] minions = GameObject.FindGameObjectsWithTag("good minion");

            foreach (GameObject minionObject in minions)
            {
                goodminionscript minionScript = minionObject.GetComponent<goodminionscript>();
                Vector3 minionPosition = minionObject.transform.position;
                float distanceToMinion = Vector3.Distance(transform.position, minionPosition);

                if (distanceToMinion <= knockbackDistance)
                {
                    Vector3 knockbackDirection1 = (minionPosition - transform.position).normalized;
                    float knockbackForce = expforce;
                    minionScript.ApplyKnockback(knockbackDirection1, knockbackForce);
                    minionScript.health -= damage + 5;
                }
            }
        }
        else if (other.gameObject.CompareTag("ice pickup"))
        {
            // Handle ice pickup
            playercontroller navigate = playercount.GetComponent<playercontroller>();
            StartCoroutine(freezeOther());
            other.gameObject.SetActive(false);
            GameObject iceCubeInstance = Instantiate(iceCube, player.position, Quaternion.identity);
            playercount.health -= 10;
            iceCubeInstance.transform.SetParent(player);
            iceCubeInstance.transform.localScale = player.localScale + new Vector3(1f, 1f, 1f);
            Destroy(iceCubeInstance, 3f);
        }
        if (other.CompareTag("box") && fuse)
        {
            // Handle box explosion when fused
            fusesound.Stop();
            shorterexplosionsound.Play();
            Instantiate(exp, transform.position, transform.rotation);
            Vector3 knockbackDirection = (player.position - transform.position).normalized;
            playerRidgebody = player.GetComponent<Rigidbody>();
            playerRidgebody.AddForce(knockbackDirection * expforce, ForceMode.Impulse);
            playercount.health -= damage +5;
            StartCoroutine(isOtherHit());
            enemyMaterial.color = originalColor;
            enemyfaceMaterial.color = orignalFaceColor;
            enemy.speed = originalspeed;
            fuse = false;
        }
        else if (other.CompareTag("box"))
        {
            // Handle box collision
            hit.Play();
            playercount.health -= damage;
        }
        else if (other.CompareTag("good minion"))
        {
            // Handle collision with good minion
            hit.Play();
            goodminionscript minionScript = other.gameObject.GetComponent<goodminionscript>();
            minionScript.health -= damage;
            if (fuse)
            {   
                fusesound.Stop();
                shorterexplosionsound.Play();
                Instantiate(exp, transform.position, transform.rotation);
                Vector3 minionPosition = minionScript.transform.position;
                Rigidbody minionRigidbody = minionScript.GetComponent<Rigidbody>();
                Vector3 knockbackDirection = (minionPosition - transform.position).normalized;
                minionRigidbody.AddForce(knockbackDirection * expforce, ForceMode.Impulse);
                minionScript.health -= damage + 5;
                enemyMaterial.color = originalColor;
                enemyfaceMaterial.color = orignalFaceColor;
                enemy.speed = originalspeed;
                fuse = false;
            }
        }
    }

    IEnumerator bombfuse()
    {
        // Coroutine for bombshell fuse
        fusesound.Play();
        enemyMaterial.color = Color.red;
        enemyfaceMaterial.color = Color.red;
        enemy.speed *= 2.5f;
        yield return new WaitForSeconds(3.0f);
        enemyMaterial.color = originalColor;
        enemyfaceMaterial.color = orignalFaceColor;
        enemy.speed = originalspeed;
        fuse = false;
    }

    private IEnumerator SmoothRise()
    {
        // Coroutine for smooth rise effect
        float riseHeight = 15.0f;
        float originalHeight = transform.position.y;
        float riseDuration = .5f;
        float fallDuration = .3f;

        isRising = true;
        isOnGround = false;
        explosion.Play();

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, originalHeight + riseHeight, transform.position.z);

        float startTime = Time.time;

        while (isRising)
        {
            float elapsedTime = Time.time - startTime;

            if (elapsedTime < riseDuration)
            {
                float t = elapsedTime / riseDuration;
                t = Mathf.Pow(t, 2);
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            }
            else if (elapsedTime < riseDuration + fallDuration)
            {
                float t = (elapsedTime - riseDuration) / fallDuration;
                t = 1 - Mathf.Pow(1 - t, 2);
                transform.position = Vector3.Lerp(targetPosition, startPosition, t);
            }
            else
            {
                isRising = false;
                transform.position = startPosition;
            }

            yield return null;
        }
        isOnGround = true;

        GameObject[] minions = GameObject.FindGameObjectsWithTag("good minion");

        foreach (GameObject minionObject in minions)
        {
            goodminionscript minionScript = minionObject.GetComponent<goodminionscript>();
            Vector3 minionPosition = minionObject.transform.position;
            float distanceToMinion = Vector3.Distance(transform.position, minionPosition);

            if (distanceToMinion <= knockbackDistance)
            {
                Vector3 knockbackDirection = (minionPosition - transform.position).normalized;
                float knockbackForce = expforce;
                minionScript.ApplyKnockback(knockbackDirection, knockbackForce);
                minionScript.health -= damage + 5;
            }
        }

        knockBack();
        Instantiate(exp, transform.position, transform.rotation);
        enemy.isStopped = false;
    }

    void knockBack()
    {
        // Function to handle knockback effect on collision with player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= knockbackDistance)
        {
            Vector3 knockbackDirection = (player.position - transform.position).normalized;
             
            if (playerRidgebody != null)
            {
                StartCoroutine(isOtherHit());
                playerRidgebody.AddForce(knockbackDirection * expforce, ForceMode.Impulse);
            }

            playercount.health -= damage + 5;
        }
    }

    Transform FindClosestPowerup()
    {
        // Find the closest powerup to the enemy
        Transform closestPowerup = null;
        float closestDistance = float.MaxValue;

        foreach (GameObject powerupObject in powerups)
        {
            Transform powerupTransform = powerupObject.transform;
            float distance = Vector3.Distance(transform.position, powerupTransform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPowerup = powerupTransform;
            }
        }

        return closestPowerup;
    }

    void setenemyhealth()
    {
        // Update enemy health UI text
        enemyhealth.text = "Health: " + health.ToString();
    }

    void setenemypower()
    {
        // Update enemy power UI text
        enemypower.text = "Power: " + count.ToString();
    }

    void initializePowerups()
    {
        // Initialize the list of powerup objects in the scene
        powerups.Clear();
        string[] powerupTags = new string[] { "pickup", "bombshell pickup", "nuke pickup", "spawner pickup", "heal pickup", "ice pickup", "landmine pickup" };

        foreach (string tag in powerupTags)
        {
            // Find all game objects with the specified tags and add them to the powerups list
            GameObject[] powerupObjects = GameObject.FindGameObjectsWithTag(tag);
            powerups.AddRange(powerupObjects);
        }
    }

    private void updatehealthbar()
    {
        // Update the health bar UI based on the current difficulty and enemy health
        float normalizedHealth = 0;
        if (difficulty == "Easy")
        {
            normalizedHealth = health / 150;
        }
        else if (difficulty == "Normal")
        {
            normalizedHealth = health / 200;
        }
        else if (difficulty == "Hard")
        {
            normalizedHealth = health / 350;
        }
        healthbar.fillAmount = normalizedHealth;
    }

    private IEnumerator isOtherHit()
    {
        // Coroutine to track if the enemy hit another object
        ifPlayerHit = true;
        yield return new WaitForSeconds(0.5f);
        ifPlayerHit = false;
    }

    void spawnMinions()
    {
        // Spawn minions around the enemy
        foreach (Vector3 offset in spawnOffset)
        {
            Vector3 spawnPosition = enemytransform.position + offset;
            GameObject spawnedMinion = Instantiate(minionPrefab, spawnPosition, enemytransform.rotation);
            badminionscript minionScript = spawnedMinion.GetComponent<badminionscript>();
            if (minionScript != null)
            {
                minionScript.playertransform = player; 
                minionScript.player = playercount; 
                minionScript.hit = hit; 
                minionScript.shorterexplosionsound = shorterexplosionsound;
                minionScript.spawnpop = spawnPop; 
            }
        }
    }

    IEnumerator freezeOther()
    {
        // Freeze the player for a duration when hit by an ice pickup
        playercontroller navigate = playercount.GetComponent<playercontroller>();
        freezing.Play();

        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        yield return new WaitForSeconds(3.0f);

        freezing.Stop();

        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = RigidbodyConstraints.None;
            playerRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
    }
}