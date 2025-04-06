using System;
using UnityEngine;

[Flags]
public enum PlayerCommands
{
    None = 0,
    Up = 1<<0,
    Down = 1<<1,
    Left = 1<<2,
    Right = 1<<3
    
}

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    [Header("플레이어 컴포넌트")]
    public Rigidbody2D rb2D;

    [Header("플레이어 설정")]
    public float moveSpeed;
    public bool canMove;
    public PlayerCommands playerCommands;

    private float horizontal;
    private float vertical;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canMove = true;
    }

    public void Flip(float dir)
    {
        gameObject.transform.localScale = new Vector2(dir, transform.localScale.y);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rb2D.linearVelocity = new Vector2(horizontal, vertical).normalized*moveSpeed;
        }
        else
        {
            rb2D.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        horizontal = 0;
        vertical = 0;
        playerCommands = PlayerCommands.None;

        if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Up"]))
        {
            vertical = 1;
            playerCommands |= PlayerCommands.Up;
        }
        if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Down"])) 
        {
            vertical = -1;
            playerCommands |= PlayerCommands.Down;
        }
        if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Right"]))
        {
            horizontal = -1;
            playerCommands |= PlayerCommands.Right;
            Flip(1);
        }
        if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Left"]))
        {
            horizontal = 1;
            playerCommands |= PlayerCommands.Left;
            Flip(-1);
        }
        if (playerCommands.HasFlag(PlayerCommands.Left) && playerCommands.HasFlag(PlayerCommands.Right))
        {
            horizontal = 0;
        }
        if (playerCommands.HasFlag(PlayerCommands.Down) && playerCommands.HasFlag(PlayerCommands.Up))
        {
            vertical = 0;
        }
    }

}
