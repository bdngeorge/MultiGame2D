using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    CapsuleCollider2D cc;

    [SerializeField] 
    private float thrust;

    string jumpDirection = "left";
    int jumpCount = 0;
    float distToWall;
    float gravScale = 1f;
    int lives = 3;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cc = GetComponent<CapsuleCollider2D>();

        distToWall = cc.bounds.extents.x;
        Debug.Log(distToWall);
    }

    // Update is called once per frame
    void Update() {
        Jump();
        Debug.Log(IsOnWall());
        if(IsOnWall()) rb.gravityScale = gravScale;
        else rb.gravityScale = 1f;

    }

    void FixedUpdate() {
    }

    void Jump() {
        if(Input.GetKeyDown(KeyCode.Space) && jumpCount < 2) {
            rb.gravityScale = 1;
            rb.velocity = Vector2.zero;
            if(jumpDirection == "left") {
                rb.AddForce(((transform.up*2) - transform.right) * thrust, ForceMode2D.Impulse);
                jumpDirection = "right";
            } else {
                rb.AddForce(((transform.up*2) + transform.right) * thrust, ForceMode2D.Impulse);
                jumpDirection = "left";
            }
            sr.flipX = !sr.flipX;
            jumpCount++;
        }
    }
    
    private bool IsOnWall() {

        Debug.DrawRay(transform.position, transform.right * (distToWall + .1f), Color.red);
        Debug.DrawRay(transform.position, -transform.right * (distToWall + .1f), Color.red);

        return Physics2D.Raycast(transform.position, transform.right, distToWall + 0.1f).collider?.tag == ("Wall")
               || Physics2D.Raycast(transform.position, transform.right, distToWall + 0.1f).collider?.tag == ("StickyWall")
               || Physics2D.Raycast(transform.position, -transform.right, distToWall + 0.1f).collider?.tag == ("Wall")
               || Physics2D.Raycast(transform.position, -transform.right, distToWall + 0.1f).collider?.tag == ("StickyWall");
    }

    IEnumerator DealWithGrav(string type) {
        switch (type) {
            case "sticky":
                rb.gravityScale = 0f;
                break;
            default:
                rb.gravityScale = .1f;
                break;            
        }
        yield return new WaitForSeconds(1f);
        if(rb.gravityScale != 1) rb.gravityScale = 1;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Wall")
          || other.gameObject.CompareTag("StickyWall")) jumpCount = 0;
        
        if(other.gameObject.CompareTag("Wall")) {
            gravScale = .1f;
            rb.velocity = Vector2.zero;
        }
        if(other.gameObject.CompareTag("StickyWall")) {
            gravScale = 0f;
            rb.velocity = Vector2.zero;
        }
        if(other.gameObject.CompareTag("SpikyWall")) {
            rb.velocity = Vector2.zero;
            // lives--;
            // rb.AddForce(*add pushback to player*)
        }
    }
}
