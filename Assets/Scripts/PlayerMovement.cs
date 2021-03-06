using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpPower;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        //Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
         horizontalInput = Input.GetAxis("Horizontal");
        

        //Flip Player based on their movement
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput > -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        
        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        //Wall jump logic
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 7;
                
            }
            
            if (Input.GetKey(KeyCode.Space))
                Jump();

        }
        else
            wallJumpCooldown += Time.deltaTime;
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            if (horizontalInput == 0)
            {
                var localScale = transform.localScale;
                body.velocity = new Vector2(-Mathf.Sign(localScale.x) * 10, 0);
                localScale = new Vector3(-Mathf.Sign(localScale.x), localScale.y,
                    localScale.z);
                transform.localScale = localScale;
            }else
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            
            wallJumpCooldown = 0;
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground") ;
    }

    private bool isGrounded()
    {
        var bounds = boxCollider.bounds;
        RaycastHit2D raycastHit =
            Physics2D.BoxCast(bounds.center, bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        var bounds = boxCollider.bounds;
        RaycastHit2D raycastHit =
            Physics2D.BoxCast(bounds.center, bounds.size, 0, new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}