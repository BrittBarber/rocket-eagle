using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class SinglePlayerController : MonoBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSkin;
    [SerializeField] public Sprite[] spriteArray;
    

    Rigidbody2D rigidBody;
    public Vector2 startingVelocity = new Vector2(5, 0);
    public Vector2 penalty = new Vector2(5, -1);
    public Vector2 horizontalAcceleration = new Vector2(1, 0);
    public Vector2 verticalAcceleration = new Vector2(0, 5);
    public float rotationSpeed = .5f;

    public float recoveryDuration = 2.0f;
    public float recoveryTimer = 0.0f;
    public bool recovering = false;
    public Quaternion originalRotation;
    public bool finished = false;
    public float finishDuration = 3.0f;
    public float finishTimer = 0.0f;

    public int lap = 1;
    public int maxLaps = 3;
    public int topBoundary = 5;
    public int bottomBoundary = -5;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = startingVelocity;
        originalRotation = transform.rotation;
        spriteRenderer = GetComponent<SpriteRenderer>();

        //get the sprite from the file
        Skins selectedSkin = SaveGameData.LoadSelectedSkin();
        if (selectedSkin == null || selectedSkin.GetPreviewImage() == null)
        {
            //no skin/file was found, using the default
            Debug.LogError("ERROR, the skin preview image was not found, resorting to default");
            spriteRenderer.sprite = defaultSkin;
        }
        else
        {
            //file was found, so everything is good
            spriteRenderer.sprite = selectedSkin.GetPreviewImage();
        }
    }


    // Update is called once per frame
    void Update()
    {

        if (finished == true)
        {
            rigidBody.velocity = new Vector2(1, 0);
            finishTimer += Time.deltaTime;
            if (finishTimer >= finishDuration)
            {
                SceneManager.LoadScene("FinishSceneSP");
            }
        }

        if (recovering == true)
        {
            recoveryTimer += Time.deltaTime;
            Blink();
            Rotate();

            if (recoveryTimer >= recoveryDuration)
            {//end recovery period
                recovering = false;
                Reset();
                recoveryTimer = 0.0f;
            }
        }
        // implement boundaries
        if (transform.position.y >= topBoundary)
        {
            transform.position = new Vector2(transform.position.x, topBoundary);
            if (rigidBody.velocity.y >= 0)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -rigidBody.velocity.y);

            }
        }

        if (transform.position.y <= bottomBoundary)
        {
            transform.position = new Vector2(transform.position.x, bottomBoundary);

            if (rigidBody.velocity.y <= 0)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -rigidBody.velocity.y);

            }
        }

        


        if (rigidBody.velocity.x < startingVelocity.x)
        {
            rigidBody.velocity = rigidBody.velocity + horizontalAcceleration * Time.deltaTime;

        }

        // increase altitude if screen is tapped
        if (Input.GetMouseButtonDown(0))
        {
            rigidBody.velocity = rigidBody.velocity + verticalAcceleration;
        }

    }

    //
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Pipe")
        {

            //reset position
            rigidBody.position = rigidBody.position - penalty;

            //reset speed
            rigidBody.velocity = new Vector2(0, 0);

            recovering = true;
        }
        if (col.gameObject.tag == "Finish")
        {
            if (lap == maxLaps)
            {
                 finished = true;
            }
            else
            {
                lap++;
                rigidBody.position = new Vector2(-6.8f, -0.65f);
            }
        }
        if (col.gameObject.tag == "SpeedBoost")
        {
            rigidBody.velocity = rigidBody.velocity + horizontalAcceleration;
        }
    }

    private void Blink()
    {
        if (this.gameObject.GetComponent<SpriteRenderer>().enabled == true)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;  //disappear
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = true;   //appear
        }
    }

    private void Rotate()
    {
        transform.Rotate(rotationSpeed, rotationSpeed, rotationSpeed);
    }

    private void Reset()
    {
        // visible
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;

        //correct rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.time * rotationSpeed);

    }
}
