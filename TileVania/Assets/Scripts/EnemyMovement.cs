using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidBody;
    PolygonCollider2D myPollygon;
    float speed;
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myPollygon = GetComponent<PolygonCollider2D>();
    }
    void Update()
    {
        myRigidBody.velocity = new Vector2(moveSpeed, 0);
        speed = myRigidBody.velocity.magnitude;
        DetectFlip();

        
    }
    void DetectFlip()
    {
        if (!myPollygon.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            moveSpeed = -moveSpeed;
            FlipEnemyFacing();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            Destroy(gameObject);
        }
        moveSpeed = -moveSpeed;
        FlipEnemyFacing();
    }

    void FlipEnemyFacing()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
    }
}
