using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    [Header("플레이어 컴포넌트")]
    public Rigidbody2D rb2D;

    [Header("플레이어 설정")]
    public float moveSpeed;
    public bool canMove;

    private float _horizontal;
    private float _vertical;
    private Animator _ani;
    private PlayerCommands _playerCommands;
    private PlayerBehave _playerBehave;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canMove = true;
        _ani = PlayerAnimator.Instance.ani;
        _playerBehave = PlayerAnimator.Instance.playerBehave;
        _playerCommands = PlayerAnimator.Instance.playerCommands;
    }

    public void Flip(float dir)
    {
        gameObject.transform.localScale = new Vector2(dir, transform.localScale.y);
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            rb2D.linearVelocity = new Vector2(_horizontal, _vertical).normalized*moveSpeed;
        }
        else
        {
            rb2D.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        _horizontal = 0;
        _vertical = 0;
        _playerCommands = PlayerCommands.None;
        _playerBehave = PlayerBehave.Idel;
        if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Up"]))
        {
            _vertical = 1;
            _playerCommands |= PlayerCommands.Up;
            _playerBehave = PlayerBehave.Walk;
        }
        if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Down"])) 
        {
            _vertical = -1;
            _playerCommands |= PlayerCommands.Down;
            _playerBehave = PlayerBehave.Walk;
        }
        if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Right"]))
        {
            _horizontal = -1;
            _playerCommands |= PlayerCommands.Right;
            _playerBehave = PlayerBehave.Walk;
            Flip(1);
        }
        if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Left"]))
        {
            _horizontal = 1;
            _playerCommands |= PlayerCommands.Left;
            _playerBehave = PlayerBehave.Walk;
            Flip(-1);
        }
        if (_playerCommands.HasFlag(PlayerCommands.Left) && _playerCommands.HasFlag(PlayerCommands.Right))
        {
            _horizontal = 0;
        }
        if (_playerCommands.HasFlag(PlayerCommands.Down) && _playerCommands.HasFlag(PlayerCommands.Up))
        {
            _vertical = 0;
        }

        switch (_playerBehave)
        {
            case PlayerBehave.Idel:
                _ani.SetInteger("Behave",0);
                break;
            case PlayerBehave.Walk:
                _ani.SetInteger("Behave", 1);
                break;
        }
    }
}
