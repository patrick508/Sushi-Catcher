using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour {

    public float Force = 40.0f; //Force to fly up
    public float forwardMovementSpeed = 3.0f; //Movement speed to go forward every frame

    public Transform groundCheckTransform; // transform of ground

    private bool OnGround; // Am i on the ground?  

    public LayerMask groundCheckLayerMask; //store a layermask of what is the ground

    private bool dead = false; //is the mouse dead?
    private int coins = 0; // amount of coins
    public static int HighScore; // Highscore of coins

    public AudioClip coinCollectSound; //Coin audio

    public GameObject RestartMenu; // All UI components for RestartMenu.
    public Text HighscoreTextPause; //Text that displays the highscore when pause menu is activated
    public Text HighscoreTextDead; //Text that displays the highscore when dead.
    public Text CollectedCoinText; //shows how many coins you collected atm.

    Animator animator;
    bool DoubleCoins = false; //If this bool is true, player gets double coins for x seconds
    float distance;

    public float speed; //Coin Magnet speed
    bool MagnetMove = false; //Decides if the coins may move when magnet is active.

    bool jetpackActive;
    public static bool PauseMenuActive;

    Vector3 startPoint;

    public Text HighScoreMeterTextPause; //Text that displays the highscoremeters when pause menu is activated
    public Text HighscoreMeterTextDead; //Text that displays the highscoremeters when dead.
    public Text TraveledMetersText; //shows how many meters you traveled atm.
    private int meters = 0; // amount of meters
    public static int MeterHighScore; // Highscore of meters

    public GameObject PauseButton;

    void Start() {
        PauseMenuActive = false;
        animator = GetComponent<Animator>();
        HighscoreTextDead.text = "" + HighScore;
        HighscoreTextPause.text = "" + HighScore;
        HighscoreMeterTextDead.text = "" + MeterHighScore;
        HighScoreMeterTextPause.text = "" + MeterHighScore;
        Invoke("IncreaseSpeed", 20.0f); //start IncreaseSpeed after x seconds.
    }

    void Awake() {
        startPoint = transform.position;
        coins = 0;
        HighScore = PlayerPrefs.GetInt("HighScore");
        MeterHighScore = PlayerPrefs.GetInt("MeterHighScore");
    }
    void Update() {
        //If dead and onground, activate RestartMenu
        if (dead && OnGround) {
            PauseMenuActive = true;
            RestartMenu.SetActive(true);
            PauseButton.SetActive(false);
        }
        //If coins is higher than highscore, set highscore to score in playerprefs.
        if (coins > HighScore) {
            HighScore = coins;
            PlayerPrefs.SetInt("HighScore", HighScore);
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
        float meterdistance = this.transform.position.x - startPoint.x;
        meters = (int)meterdistance;
        TraveledMetersText.text = ("M: " + meterdistance.ToString("f0"));
        //If meters is higher than highscoremeters, set highscoremeters to meters in playerprefs.
        if (meters > MeterHighScore) {
            MeterHighScore = meters;
            PlayerPrefs.SetInt("MeterHighScore", MeterHighScore);
            HighscoreMeterTextDead.text = "M" + MeterHighScore;
            HighScoreMeterTextPause.text = "M" + MeterHighScore;
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
    #endregion
    //Reset HighScore.
    public void Reset() {
        PlayerPrefs.DeleteAll();
        HighScore = 0;
        HighscoreTextDead.text = "" + HighScore;
        HighscoreTextPause.text = "" + HighScore;
        MeterHighScore = 0;
        HighScoreMeterTextPause.text = "" + MeterHighScore;
        HighscoreMeterTextDead.text = "" + MeterHighScore;
    }
    //Put a coin icon in the top left and update it if hit coin
    void DisplayCoinsCount() {
        CollectedCoinText.text = coins.ToString();
    }
    //If i press Fire1 and JetpackActive, add force to player and keep going forward with given speed.
    void FixedUpdate() {
        if (Force <= 25.5f) {
            Force = 40f;
        }
            bool JetpackActive = Input.GetButton("Fire1");
            jetpackActive = jetpackActive && !dead;

        if (Input.GetButton("Fire1")) {
            if (Force > 25.5f) {
                Force -= 0.75f;
            }
        }
            if (!dead) {
            //If JetpackActive = true and you dont click a UI element, add the given force.
            if (JetpackActive) {
                if (!EventSystem.current.currentSelectedGameObject) {
                    GetComponent<Rigidbody2D>().AddForce(new Vector2(0, Force));
                }
            }
                Vector2 newVelocity = GetComponent<Rigidbody2D>().velocity;
                newVelocity.x = forwardMovementSpeed;
                GetComponent<Rigidbody2D>().velocity = newVelocity;
            }
            UpdateGroundStatus();
        }

    void UpdateGroundStatus() {
        //Create a cirkel of 0.1 to check if i overlap with any object that has the layermask specified in groundCheckLayerMask
        OnGround = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);

        //Set grounded parameter of Animator and trigger the animation.
        animator.SetBool("grounded", OnGround);
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