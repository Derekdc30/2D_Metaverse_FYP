using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet;

public class PlayerMovement : NetworkBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 1f;
    //public GameObject playerCamera;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private Vector2 movement;
    private string currentAnimation = "";

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(base.IsOwner)
        {
            //playerCamera.SetActive(true);
            animator = GetComponent<Animator>();
        }
    }

    private void Start(){
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");     
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

   
  
    private void Update(){
        if(!base.IsOwner){return;}
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement = new Vector2(moveHorizontal,moveVertical);
        rb.velocity = movement* moveSpeed;

        //bool moving = (moveHorizontal != 0f || moveVertical != 0f);
        Flip();
        CheckAnimation();
    }

    private void Flip()
    {
        float moveDir = Input.GetAxis("Horizontal");
        rb.velocity = movement* moveSpeed;

        //Determine whether flip based on volocity
        bool faceDir = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if (faceDir)
        {
            if (rb.velocity.x > 0.1f)
            {
                sr.flipX = false;
            }
            if (rb.velocity.x < -0.1f)
            {
                sr.flipX = true;
            }
        }
    }
    private void CheckAnimation()
    {
        if(rb.velocity.x != 0 || rb.velocity.y != 0)
        {
            ChangeAnimation("Moving");
        }
        else
            ChangeAnimation("Idle");
    }
     private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if(currentAnimation != animation)
        {
            currentAnimation = animation;
            animator.CrossFade(animation, crossfade);
        }
    }
}
