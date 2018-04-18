using UnityEngine;
using System.Collections;

public class LaserScript : MonoBehaviour {
    public Sprite laserOnSprite;
    public Sprite laserOffSprite;
    public float interval = 0.5f; // apeed for on/off laser
    public float rotationSpeed = 0.0f; // speed for laser rotation
    private bool isLaserOn = true;
    private float timeUntilNextToggle;

    void Start () {
        timeUntilNextToggle = interval;
    }

    void FixedUpdate() {
        //decrease time until next toggle
        timeUntilNextToggle -= Time.fixedDeltaTime;

        //change laser on and off, and enable collider if laser is on.
        if (timeUntilNextToggle <= 0) {
            isLaserOn = !isLaserOn;
            GetComponent<Collider2D>().enabled = isLaserOn;

            //Changelasersprite to on and off sprite, and reset timeuntilnexttoggle
            SpriteRenderer spriteRenderer = ((SpriteRenderer)this.GetComponent<Renderer>());
            if (isLaserOn)
                spriteRenderer.sprite = laserOnSprite;
            else
                spriteRenderer.sprite = laserOffSprite;

            timeUntilNextToggle = interval;
        }

        //rotate laser
        transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.fixedDeltaTime);
    }
}
