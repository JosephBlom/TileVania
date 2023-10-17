using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb2d;
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    BoxCollider2D myFeetCollider;
    SpriteRenderer mySpriteRenderer;
    Scene scene;
    float startGravity;
    bool hasJump = true;
    bool isAlive = true;
    int coinCount = 0;

    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] float jumpSpeed = 5.0f;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float climbSpeed = 3.0f;
    [SerializeField] GameObject prefab;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] public int bulletDamage = 5;
    [SerializeField] float fireTime = 1f;
    [SerializeField] TextMeshProUGUI myText;
    float timer = 0f;


    void Start()
    {
        Debug.Log(SceneManager.sceneCount);
        rb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        startGravity = rb2d.gravityScale;
        myFeetCollider = GetComponent <BoxCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if(isAlive)
        {
            Run();
            flipSprite();
            climbLadder();
            Shoot();
        }
        Die();
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Coin")
        {
            Destroy(collision.gameObject);
            coinCount++;
            myText.text = "Coins: " + coinCount;
        }
        if(collision.gameObject.tag  == "Exit")
        {
            scene = SceneManager.GetActiveScene();
            if (scene.buildIndex == 2)
            {
                Debug.Log("You Win!");
            }
            else
            {
                int nextScene = scene.buildIndex + 1;
                StartCoroutine(DelaySceneLoad(nextScene));
                IEnumerator DelaySceneLoad(int nextScene)
                {
                    yield return new WaitForSeconds(2f);
                    SceneManager.LoadScene(nextScene);
                }
            }
        }
    }
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
 
    void OnJump(InputValue value)
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }
        if (value.isPressed)
        {
            rb2d.velocity += new Vector2(0f, jumpSpeed);
            hasJump = false;
        }
    }
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rb2d.velocity.y);
        rb2d.velocity = playerVelocity;

        bool playerHaseHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        if(playerHaseHorizontalSpeed)
        {
            myAnimator.SetBool("isRunning", true);
        }
        else
        {
            myAnimator.SetBool("isRunning", false);
        }
    }

    void flipSprite()
    {
        bool playerHaseHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;

        if (playerHaseHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb2d.velocity.x), 1f);
        }
    }

    void climbLadder()
    {
        
        if (!myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) { rb2d.gravityScale = startGravity; myAnimator.SetBool("isClimbing", false); return; }

        Vector2 climbVelocity = new Vector2(rb2d.velocity.x, moveInput.y * climbSpeed);
        rb2d.gravityScale = 0f;
        rb2d.velocity = climbVelocity;

        bool playerHasVerticalSpeed = Mathf.Abs(rb2d.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing", playerHasVerticalSpeed);
    }


    void Die()
    {
        if (myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies")) || myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Water")) || myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            isAlive = false;
            mySpriteRenderer.color = Color.red;
            myAnimator.SetTrigger("Dying");
            rb2d.velocity = deathKick;
            scene = SceneManager.GetActiveScene();
            int activeScene = scene.buildIndex;
            StartCoroutine(DelaySceneLoad(activeScene));
            IEnumerator DelaySceneLoad(int activeScene)
            {
                yield return new WaitForSeconds(2f);
                SceneManager.LoadScene(activeScene);
            }

            
        }
        
    }
    void Shoot()
    {
        Debug.Log("here");
        Vector3 shootDirection = Input.mousePosition;
        shootDirection = Camera.main.ScreenToWorldPoint(shootDirection);
        shootDirection.z = 0f;
        shootDirection -= transform.position;

        timer += Time.deltaTime;

        if (Input.GetButton("Fire1") && timer > fireTime)
        {
            Debug.Log("foo");
            shootDirection.Normalize();
            GameObject bullet = Instantiate(prefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = shootDirection * bulletSpeed;
            bullet.transform.right = shootDirection;
            Destroy(bullet, 3);
            timer = 0;
        }
    }
}
