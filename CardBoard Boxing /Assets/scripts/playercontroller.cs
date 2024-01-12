using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class playercontroller : MonoBehaviour
{
    private Rigidbody rb;
    public int count;
    public float speed;
    public float health;
    private int damage;

    public TextMeshProUGUI PlayerHealth;
    public TextMeshProUGUI playerpower;

    private float jumpTimer;
    private float scalingFactor = 1.0f;

    public enemyscript enemy;
    public float rotationSpeed = 1.0f;
    public Text gameover;
    public GameObject playagain;
    public AudioSource hit;

    public AudioSource powerding;
    public AudioSource explosion;
    public AudioSource dash;

    public GameObject quitgame;

    public GameObject exp;
    public float expforce, radius;

    bool activated;
    float downForce = 1.0f;

    float knockbackDistance = 3.0f;

    bool downForceActive = false;

    public Transform enemyTransform;

    private bool isRising = false;

    private float dashForce = 2.0f;
    private bool isDashing = false;
    private float originalSpeed;

    private float currentStamina;
    public float maxStamina = 10f;
    public float staminaRegenerationRate = 2f;
    public float dashStaminaCost = 10.0f;

    public Rigidbody enemyridgebody;

    bool fuse = false;

    public Material playerMaterial; 
    public Material playerfaceMaterial;
    private Color originalColor, orignalFaceColor;

    [SerializeField] private Image healthbar;
    [SerializeField] private Image stamwheel;

    private bool areConstraintsDisabled = false;

    public AudioSource fusesound;
    public AudioSource shorterexplosionsound;
    public AudioSource spawnPop;

    public goodminionscript yourminion;
    public Rigidbody yourminionRigidbody;
    
    public GameObject minionPrefab;

    Transform minionspawnpoint;
    public Transform playertransform;

    public GameObject landmine;
    private Vector3 target;
    private Vector3 iceSize;

    public GameObject iceCube;

    public AudioSource healthpickup;
    public AudioSource freezing;

    badminionscript badminion;

    float maxhealth = 200f;


    public float currentFreezeDuration = 0f;
    public float originalFreezeDuration = 3f; // Set your initial freeze duration here

    Vector3[] spawnOffset = new Vector3[]
    {
        new Vector3(2f, 2f, 0f),
        new Vector3(0f, 2f, 2f),
        new Vector3(-2f, 2f, 0f)
    };

    void Start()
    {
        originalColor = playerMaterial.color;
        orignalFaceColor = playerfaceMaterial.color;
        quitgame.SetActive(false);
        playagain.SetActive(false);
        gameover.enabled = false;
        rb = GetComponent<Rigidbody>();
        count = 0;
        health = maxhealth;
        damage = 10;
        maxStamina = 10;
        knockbackDistance = 10.0f;
        currentStamina = maxStamina;
        originalSpeed = speed;
        sethealthtext();
        setplayerpower();
        updatehealthbar();
        updatestamwheel();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        Vector3 horizontalForce = moveDirection * speed;
        horizontalForce.y = rb.velocity.y;
        rb.velocity = horizontalForce;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private IEnumerator DisableMovementConstraintsForSeconds(float seconds)
    {
        rb.constraints = RigidbodyConstraints.None;

        yield return new WaitForSeconds(seconds);

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        areConstraintsDisabled = false;
    }

    void DisableMovementConstraintsForOneSecond()
    {
        if (!areConstraintsDisabled)
        {
            StartCoroutine(DisableMovementConstraintsForSeconds(1.0f));
            areConstraintsDisabled = true;
        }
    }

    void Update()
    {
        if (enemy.ifPlayerHit == true)
        {
            StartCoroutine(DisableMovementConstraintsForSeconds(0.5f));
        }

        if (health <= 0)
        {
            health = 0;
            gameObject.SetActive(false);
            gameover.enabled = true;
            playagain.SetActive(true);
            quitgame.SetActive(true);
            playerMaterial.color = originalColor;
            playerfaceMaterial.color = orignalFaceColor;
        }

        if (enemy.health <= 0)
        {
            playerMaterial.color = originalColor;
            playerfaceMaterial.color = orignalFaceColor;
        }

        jumpTimer -= Time.deltaTime;

        if (!isDashing)
        {
            if (Input.GetButtonDown("Jump") && CanDash())
            {
                StartCoroutine(Dash());
            }
        }

        if (!isRising && downForceActive)
        {
            rb.AddForce(Vector3.down * (downForce * 2.0f), ForceMode.Impulse);
        }

        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenerationRate * Time.deltaTime * 3;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            updatestamwheel();
        }

        sethealthtext();
        setplayerpower();
        updatehealthbar();
    }

    private IEnumerator Dash()
    {
        if (CanDash())
        {
            isDashing = true;
            currentStamina -= dashStaminaCost;

            speed *= 2f;
            dash.Play();

            Vector3 dashDirection = rb.velocity.normalized;

            rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);

            yield return new WaitForSeconds(0.2f);

            while (speed > originalSpeed)
            {
                speed -= 50.0f * Time.deltaTime;
                yield return null;
            }

            speed = originalSpeed;

            isDashing = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pickup"))
        {
            powerding.Play();
            other.gameObject.SetActive(false);
            count += 1;
            transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            health += 15;
            damage += 5;
            rb.mass += 1;
            rb.drag += 1;
            rb.angularDrag += 1;
            expforce += 10;
            knockbackDistance += 1;
            scalingFactor = 1.0f / transform.localScale.x;
            setplayerpower();
            updatehealthbar();
        }
        else if (other.gameObject.CompareTag("bombshell pickup"))
        {
            health += 10;
            other.gameObject.SetActive(false);
            fuse = true;
            StartCoroutine(bombfuse());
        }
        else if (other.gameObject.CompareTag("spawner pickup"))
        {
            other.gameObject.SetActive(false);
            spawnMinions();
        }
        else if (other.gameObject.CompareTag("nuke pickup"))
        {
            health += 5;
            downForceActive = true;
            activated = true;
            other.gameObject.SetActive(false);
            StartCoroutine(SmoothRise());
            explosion.Play();
        }
        else if (other.gameObject.CompareTag("heal pickup"))
        {
            healthpickup.Play();
            other.gameObject.SetActive(false);
            health += 50;
        }
        else if (other.gameObject.CompareTag("landmine pickup"))
        {
            other.gameObject.SetActive(false);
            Vector3 offset = new Vector3(0f, 3f, 0f);
            target = playertransform.position + offset;
            spawnPop.Play();
            Instantiate(landmine, target, playertransform.rotation);
        }
        else if (other.gameObject.CompareTag("ice pickup"))
        {
            enemyscript navigate = enemy.GetComponent<enemyscript>();
            navigate.shouldNavigate = false;
            StartCoroutine(freezeOther());
            other.gameObject.SetActive(false);
            
            GameObject iceCubeInstance = Instantiate(iceCube, enemyTransform.position, Quaternion.identity);
            enemy.health -= 10;
            
            iceCubeInstance.transform.SetParent(enemyTransform);
            iceCubeInstance.transform.localScale = enemyTransform.localScale + new Vector3(1f, 1f, 1f);
            Destroy(iceCubeInstance, 3f);
        }
        else if (other.gameObject.CompareTag("landmine"))
        {
            other.gameObject.SetActive(false);
            shorterexplosionsound.Play();
            Instantiate(exp, transform.position, transform.rotation);
            knockBack();
            Vector3 knockbackDirection = (target - transform.position).normalized;
            rb.AddForce(knockbackDirection * expforce, ForceMode.Impulse);
            health -= damage + 5;
            GameObject[] minions = GameObject.FindGameObjectsWithTag("bad minion");
            foreach (GameObject minionObject in minions)
            {
                badminionscript minionScript = minionObject.GetComponent<badminionscript>();
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
        else if (other.CompareTag("enemy") && fuse)
        {
            fusesound.Stop();
            shorterexplosionsound.Play();
            Instantiate(exp, transform.position, transform.rotation);
            Vector3 knockbackDirection = (enemyTransform.position - transform.position).normalized;
            enemyridgebody = enemy.GetComponent<Rigidbody>();
            enemyridgebody.AddForce(knockbackDirection * expforce, ForceMode.Impulse);
            enemy.health -= damage + 5;
            playerMaterial.color = originalColor;
            playerfaceMaterial.color = orignalFaceColor;
            speed = originalSpeed;
            fuse = false;
        }
        else if (other.CompareTag("enemy"))
        {
            hit.Play();
            enemy.health -= damage;
        }
        if (other.CompareTag("bad minion"))
        {
            hit.Play();
            badminionscript minionScript = other.gameObject.GetComponent<badminionscript>();
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
                playerMaterial.color = originalColor;
                playerfaceMaterial.color = orignalFaceColor;
                speed = originalSpeed;
                fuse = false;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (activated == true)
        {
            if (exp != null)
            {
                Instantiate(exp, transform.position, transform.rotation);
                knockBack();

                GameObject[] minions = GameObject.FindGameObjectsWithTag("bad minion");

                foreach (GameObject minionObject in minions)
                {
                    badminionscript minionScript = minionObject.GetComponent<badminionscript>();
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
            }
            activated = false;
            downForceActive = false;
        }
    }

    void knockBack()
    {
        float distanceToEnemy = Vector3.Distance(transform.position, enemyTransform.position);

        if (distanceToEnemy <= knockbackDistance)
        {
            Vector3 knockbackDirection = (enemyTransform.position - transform.position).normalized;

            Rigidbody enemyRigidbody = enemyTransform.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                enemyRigidbody.AddForce(knockbackDirection * expforce, ForceMode.Impulse);
            }

            enemy.health -= damage + 5;
        }
    }

    void sethealthtext()
    {
        PlayerHealth.text = "Health: " + health.ToString();
    }

    void setplayerpower()
    {
        playerpower.text = "Power: " + count.ToString();
    }

    private IEnumerator SmoothRise()
    {
        float riseHeight = 15.0f;
        float riseSpeed = 50.0f;

        isRising = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(transform.position.x, riseHeight, transform.position.z);

        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (isRising)
        {
            float distanceCovered = (Time.time - startTime) * riseSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            if (fractionOfJourney >= 1.0f)
            {
                isRising = false;
            }

            yield return null;
        }
    }

    IEnumerator bombfuse()
    {
        fusesound.Play();
        playerMaterial.color = Color.red;
        playerfaceMaterial.color = Color.red;
        speed *= 2f;

        yield return new WaitForSeconds(3.0f);

        playerMaterial.color = originalColor;
        playerfaceMaterial.color = orignalFaceColor;
        speed = originalSpeed;
        fuse = false;
    }

    private void updatehealthbar()
    {
        float normalizedHealth = health / maxhealth;
        healthbar.fillAmount = normalizedHealth;
    }

    private void updatestamwheel()
    {
        float normalizedStamina = currentStamina / maxStamina;
        stamwheel.fillAmount = normalizedStamina;
    }

    private bool CanDash()
    {
        return currentStamina >= dashStaminaCost;
    }

    void spawnMinions()
    {
        foreach (Vector3 offset in spawnOffset)
        {
            Vector3 spawnPosition = playertransform.position + offset;
            GameObject spawnedMinion = Instantiate(minionPrefab, spawnPosition, playertransform.rotation);
            goodminionscript minionScript = spawnedMinion.GetComponent<goodminionscript>();
            if (minionScript != null)
            {
                minionScript.enemytransform = enemyTransform;
                minionScript.enemy = enemy;
                minionScript.hit = hit; 
                minionScript.shorterexplosionsound = shorterexplosionsound;
                minionScript.spawnpop = spawnPop; 
            }
        }
    }

    IEnumerator freezeOther()
    {
        enemyscript navigate = enemy.GetComponent<enemyscript>();
        if (navigate != null)
        {
            freezing.Play();
            
            Rigidbody enemyRigidbody = enemy.GetComponent<Rigidbody>();
            if (enemyRigidbody != null)
            {
                enemyRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            }

            yield return new WaitForSeconds(3.0f);

            freezing.Stop();
            
            if (enemyRigidbody != null)
            {
                enemyRigidbody.constraints = RigidbodyConstraints.None;
            }
            navigate.shouldNavigate = true;
        }
    }
}

