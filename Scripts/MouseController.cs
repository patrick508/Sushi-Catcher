using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    public float jetpackForce = 40.0f; //Force to fly up
    public float forwardMovementSpeed = 3.0f; //Movement speed to go forward every frame

    public Transform groundCheckTransform; // transform of ground

    private bool OnGround; // Am i on the ground?  

    public LayerMask groundCheckLayerMask; //store a layermask of what is the ground
    public ParticleSystem jetpack; //jetpack flames

    private bool dead = false; //is the mouse dead?
    private int coins = 0; // amount of coins
    private int HighScore; // Highscore

    public Texture2D coinIconTexture; //coin texture

    public AudioClip coinCollectSound; //Coin audio
    public AudioSource jetpackAudio; //Audiosource for jetpack
    public AudioSource footstepsAudio; //Audiosource for footsteps

    public GameObject RestartMenu; // All UI components for RestartMenu.
    public Text HighscoreTextPause; //Text that displays the highscore when pause menu is activated
    public Text HighscoreTextDead; //Text that displays the highscore when dead.
    public Image HighScoreBG; // Background image for Highscore.
    public Text CollectedCoinText;

    Animator animator;
    bool DoubleCoins = false; //If this bool is true, player gets double coins for x seconds
    float distance;

    public float speed; //Coin Magnet speed
    bool MagnetMove = false; //Decides if the coins may move when magnet is active.

    public Text Speedup;
    public Transform player;
    bool jetpackActive;
    public static bool PauseMenuActive;
  // bool test1 = false;
   // bool test2 = false;

    void Start() {
        PauseMenuActive = false;
        animator = GetComponent<Animator>();
        HighscoreTextDead.text = "" + HighScore;
        HighscoreTextPause.text = "" + HighScore;
        Invoke("IncreaseSpeed", 20.0f); //start IncreaseSpeed after x seconds.
    }

    void Awake() {
        coins = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");
    }
    void Update() {
        //If dead and onground, activate RestartMenu
        if (dead && OnGround) {
            PauseMenuActive = true;
            RestartMenu.SetActive(true);
        }
        //If coins is higher than highscore, set highscore to score in playerprefs.
        if (coins > HighScore) {
            HighScore = coins;
            PlayerPrefs.SetInt("", HighScore);
            HighscoreTextDead.text = "" + HighScore;
            HighscoreTextPause.text = "" + HighScore;
        }
        DisplayCoinsCount();
        //Create a points array for all coins and make them move towards player.
        GameObject[] points = GameObject.FindGameObjectsWithTag("Coins");
        if (MagnetMove == true) {
            foreach (GameObject go in points) {
                distance = Vector3.Distance(go.transform.position, this.transform.position);
                float step = speed * Time.deltaTime;
                if (distance < 8f) {
                    go.transform.position = Vector3.MoveTowards(go.transform.position, this.transform.position, step);
                }
            }
        }

    }
    #region Increase MovementSpeed
    //Increase movement speed to 6 every x seconds with x amount
    void IncreaseSpeed() {
        if (forwardMovementSpeed < 6f) {
            forwardMovementSpeed += 0.025f;
            Invoke("IncreaseSpeed", .5f);
        }
    }

 /*   IEnumerator SpeedText() {
        if (test1 == true) {
            Vector3 SpeedText = Camera.main.WorldToScreenPoint(player.position);
            Speedup.text = "Speedup >>";
            Speedup.transform.position = new Vector3(SpeedText.x + 50, SpeedText.y + 180, 0);
        }
        if (test2 == true) {
            Vector3 SpeedText = Camera.main.WorldToScreenPoint(player.position);
            Speedup.text = "Maxspeed!";
            Speedup.transform.position = new Vector3(SpeedText.x + 50, SpeedText.y + 180, 0);
            Speedup.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(4f);
        Speedup.gameObject.SetActive(false);
        test1 = false;
        StopCoroutine(SpeedText());
    } */
    #endregion
    //Reset HighScore.
    public void Reset() {
        PlayerPrefs.DeleteAll();
        HighScore = 0;
        HighscoreTextDead.text = "" + HighScore;
        HighscoreTextPause.text = "" + HighScore;
    }
    //Put a coin icon in the top left and update it if hit coin
    void DisplayCoinsCount() {
        CollectedCoinText.text = coins.ToString();
    }
    //If i press Fire1 and JetpackActive, add force to player and keep going forward with given speed.
    void FixedUpdate() {
        if (jetpackForce <= 25.5f) {
            jetpackForce = 40f;
        }
            bool JetpackActive = Input.GetButton("Fire1");
            jetpackActive = jetpackActive && !dead;

        if (Input.GetButton("Fire1")) {
            if (jetpackForce > 25.5f) {
                jetpackForce -= 0.75f;
            }
        }
            if (!dead) {
            //If JetpackActive = true and you dont click a UI element, add the given force.
            if (JetpackActive) {
                if (!EventSystem.current.currentSelectedGameObject) {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jetpackForce));
                }
            }
                Vector2 newVelocity = GetComponent<Rigidbody2D>().velocity;
                newVelocity.x = forwardMovementSpeed;
                GetComponent<Rigidbody2D>().velocity = newVelocity;
            }
            UpdateGroundStatus();
            AdjustJetpack(jetpackActive);
            AdjustFootstepsAndJetpackSound(jetpackActive);
        }

    void AdjustFootstepsAndJetpackSound(bool jetpackActive) {
        footstepsAudio.enabled = !dead && OnGround;

        jetpackAudio.enabled = !dead && !OnGround;
        jetpackAudio.volume = jetpackActive ? 1.0f : 0.5f;
    }

    void UpdateGroundStatus() {
        //Create a cirkel of 0.1 to check if i overlap with any object that has the layermask specified in groundCheckLayerMask
        OnGround = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);

        //Set grounded parameter of Animator and trigger the animation.
        animator.SetBool("grounded", OnGround);
    }

    //Disable particle system and slowly decrease emission, and vice versa
    void AdjustJetpack(bool jetpackActive) { 
            jetpack.enableEmission = !OnGround;
            jetpack.emissionRate = jetpackActive ? 300.0f : 75.0f;

        }

    //change the collider based on what i hit.
    void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.CompareTag("Coins"))
            CollectCoin(collider);
        if (collider.gameObject.CompareTag("PWDoubleCoin"))
            PowerUpDoubleCoin(collider);
        if (collider.gameObject.CompareTag("PWMagnet"))
            PowerUpMagnet(collider);
        if (collider.gameObject.CompareTag("Lasers"))
            HitByLaser(collider);
        if (collider.gameObject.CompareTag("LasersHigh"))
            HitByLaser(collider);
        if (collider.gameObject.CompareTag("LasersLow"))
            HitByLaser(collider);
    }
    //if i hit laser
    void HitByLaser(Collider2D laserCollider) {
        if (!dead) 
        laserCollider.GetComponent<AudioSource>().Play();
        dead = true;
        animator.SetBool("dead", true);
    }

    //increase coins and destroy hitted coin.
    void CollectCoin(Collider2D coinCollider) {
        if (DoubleCoins == false) {
            coins++;
        }
        if (DoubleCoins == true) {
            coins += 2;
        }

        Destroy(coinCollider.gameObject);
        AudioSource.PlayClipAtPoint(coinCollectSound, transform.position);
    }
    #region Magnet Powerup
    void PowerUpMagnet(Collider2D PowerupMagnet) {
        StartCoroutine(PowerupMagnetRoutine());
        Destroy(PowerupMagnet.gameObject);
    }
    IEnumerator PowerupMagnetRoutine() {
        MagnetMove = true;
        yield return new WaitForSeconds(5f);
        MagnetMove = false;
    }
    #endregion
    #region DoubleCoins Powerup
    //Increase collected coins to x2 (powerup)
    void PowerUpDoubleCoin(Collider2D PowerupCollider) {
        print("Godverdomme mooie powerup!");
        StartCoroutine(PowerupDoublePoints());
        Destroy(PowerupCollider.gameObject);
    }

    //Set double coins to true for x seconds
    IEnumerator PowerupDoublePoints() {
        DoubleCoins = true;
        yield return new WaitForSeconds(8f);
        DoubleCoins = false;
    }
    #endregion
}