using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 1f;
    private Rigidbody2D rb;
    private PlayerAnimating _animating;
    private void Start(){
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");     
        Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
    private void Awake() {
        _animating = GetComponent<PlayerAnimating>();
    }
    private void Update(){
        if(!base.IsOwner){return;}
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal,moveVertical);
        rb.velocity = movement* moveSpeed;

        bool moving = (moveHorizontal != 0f || moveVertical != 0f);
        _animating.SetMoving(moving);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("TP_Home"))
        {
            SendPlayerToOwnIsland();
        }
    }
    void SendPlayerToOwnIsland()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("UserHome");
    }
}
