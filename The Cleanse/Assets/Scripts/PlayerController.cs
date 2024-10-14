using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;

    private Animator playerAnimator;
    private SpriteRenderer playerSprite;

    void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();
    }   

    void OnEnable()
    {
        playerControls.Enable();
    }

    void Update()
    {
        PlayerInput();
    }

    void FixedUpdate()
    {
        SetPlayerDirection();
        Move();
    }

    void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        playerAnimator.SetFloat("xDirection", movement.x);
        playerAnimator.SetFloat("yDirection", movement.y);
    }

    void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.deltaTime));
    }

    void SetPlayerDirection()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePosition.x < playerScreenPoint.x)
        {
            playerSprite.flipX = true;
        }
        else
        {
            playerSprite.flipX = false;
        }
    }

}
