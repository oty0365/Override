using System;
using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    [Header("플레이어 컴포넌트")]
    public Rigidbody2D rb2D;

    [Header("플레이어 설정")]
    public float moveSpeed;
    private float _originMoveSpeed;
    public bool canInput;
    public float dashTime;

    [Header("대쉬 잔상")]
    public APoolingObject afterImage;

    private float _horizontal;
    private float _vertical;
    private Animator _ani;
    private SpriteRenderer _sr;
    private bool _isKnockBacking;
    private bool _isDashing;

    private PlayerCommands _playerCommands;
    public PlayerCommands PlayerCommands
    {
        get => _playerCommands;
        set
        {
            _playerCommands = value;
            PlayerAnimator.Instance.playerCommands = value;
        }
    }

    private PlayerBehave _playerBehave;
    public PlayerBehave PlayerBehave
    {
        get => _playerBehave;
        set
        {
            _playerBehave = value;
            PlayerAnimator.Instance.playerBehave = value;
        }
    }

    private Vector2 _dir;
    private float _force;
    private float _knockBackTime;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        canInput = true;
        _isKnockBacking = false;
        _ani = PlayerAnimator.Instance.ani;
        _originMoveSpeed = moveSpeed;
        _sr = GetComponent<SpriteRenderer>();
    }

    public void Flip()
    {
        var dir = 0;
        if (gameObject.transform.position.x - Camera.main.ScreenToWorldPoint(Input.mousePosition).x > 0)
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }
        gameObject.transform.localScale = new Vector2(dir, transform.localScale.y);
    }

    private void FixedUpdate()
    {
        switch (PlayerBehave)
        {
            case PlayerBehave.Idel:
            case PlayerBehave.Walk:
                if (canInput)
                {
                    rb2D.linearVelocity = new Vector2(_horizontal, _vertical).normalized * moveSpeed;
                }
                else
                {
                    rb2D.linearVelocity = Vector2.zero;
                }
                    break;

            case PlayerBehave.KnockBack:
                if (!_isKnockBacking)
                {
                    rb2D.linearVelocity = Vector2.zero;
                    rb2D.linearDamping = 4f;
                    rb2D.AddForce(_dir * _force, ForceMode2D.Impulse);
                    _isKnockBacking = true;
                    StartCoroutine(KnockBackFlow());
                }
                break;

            case PlayerBehave.Dash:
                if (!_isDashing)
                {
                    rb2D.linearVelocity = new Vector2(_horizontal, _vertical).normalized * 20f;
                    _isDashing = true;
                    StartCoroutine(DashFlow());
                }
                break;
        }
    }

    public void KnockBack(Vector2 dir, float force, float time)
    {
        _dir = dir;
        _force = force;
        _knockBackTime = time;
        PlayerBehave = PlayerBehave.KnockBack;
    }

    private IEnumerator KnockBackFlow()
    {
        canInput = false;
        yield return new WaitForSeconds(_knockBackTime);
        PlayerBehave = PlayerBehave.Idel;
        rb2D.linearDamping = 0;
        _isKnockBacking = false;
        canInput = true;
    }

    public void Dash()
    {
        PlayerBehave = PlayerBehave.Dash;
        AddCommand(PlayerCommands.Dash);
    }

    private IEnumerator DashFlow()
    {
        canInput = false;
        float step = dashTime / 8f;
        float nextStep = step;

        float alpha = 1;
        float size = 1;

        for (float t = 0f; t <= dashTime; t += Time.deltaTime)
        {
            if (t >= nextStep)
            {
                var o = ObjectPooler.Instance.Get(afterImage, transform.position, transform.rotation.eulerAngles);
                o.GetComponent<PlayerAfterImage>().SetIamge(_sr.sprite, alpha, size,gameObject.transform.localScale.x);

                nextStep += step;
            }
            yield return null;
        }

        PlayerBehave = PlayerBehave.Idel;
        rb2D.linearVelocity = Vector2.zero;
        _isDashing = false;
        canInput = true;
    }

    void Update()
    {
        if (PlayerBehave != PlayerBehave.KnockBack && PlayerBehave != PlayerBehave.Dash && PlayerBehave != PlayerBehave.SpearRun && PlayerCommands == PlayerCommands.None)
        {
            PlayerBehave = PlayerBehave.Idel;
        }

        Flip();
        HandleMoveSpeed();
        InputMove();

        switch (PlayerBehave)
        {
            case PlayerBehave.Idel:
                _ani.SetInteger("Behave", 0);
                break;
            case PlayerBehave.Walk:
                _ani.SetInteger("Behave", 1);
                break;
        }
    }

    private void HandleMoveSpeed()
    {
        if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Dash"]) && PlayerBehave != PlayerBehave.Dash)
        {
            AddCommand(PlayerCommands.Run);
            moveSpeed = 5f;
        }

        if (Input.GetKeyUp(KeyBindingManager.Instance.keyBindings["Dash"]) && PlayerBehave != PlayerBehave.Dash)
        {
            RemoveCommand(PlayerCommands.Run);
            moveSpeed = _originMoveSpeed;
        }
    }

    public void AddCommand(PlayerCommands command)
    {
        if (!_playerCommands.HasFlag(command))
        {
            _playerCommands |= command;
            PlayerAnimator.Instance.playerCommands = _playerCommands;
        }
    }

    public void RemoveCommand(PlayerCommands command)
    {
        if (_playerCommands.HasFlag(command))
        {
            _playerCommands &= ~command;
            PlayerAnimator.Instance.playerCommands = _playerCommands;
        }
    }

    private void InputMove()
    {
        _horizontal = 0;
        _vertical = 0;
        _playerCommands = PlayerCommands.None;

        if (canInput)
        {
            if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Up"]))
            {
                _vertical = 1;
                AddCommand(PlayerCommands.Up);
                if (PlayerBehave == PlayerBehave.Walk || PlayerBehave == PlayerBehave.Idel)
                    PlayerBehave = PlayerBehave.Walk;
            }
            if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Down"]))
            {
                _vertical = -1;
                AddCommand(PlayerCommands.Down);
                if (PlayerBehave == PlayerBehave.Walk || PlayerBehave == PlayerBehave.Idel)
                    PlayerBehave = PlayerBehave.Walk;
            }
            if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Right"]))
            {
                _horizontal = -1;
                AddCommand(PlayerCommands.Right);
                if (PlayerBehave == PlayerBehave.Walk || PlayerBehave == PlayerBehave.Idel)
                    PlayerBehave = PlayerBehave.Walk;
            }
            if (Input.GetKey(KeyBindingManager.Instance.keyBindings["Left"]))
            {
                _horizontal = 1;
                AddCommand(PlayerCommands.Left);
                if (PlayerBehave == PlayerBehave.Walk || PlayerBehave == PlayerBehave.Idel)
                    PlayerBehave = PlayerBehave.Walk;
            }

            if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Dash"]))
            {
                if(PlayerCommands != PlayerCommands.None)
                {
                    Dash();
                }
            }
            if (_playerCommands.HasFlag(PlayerCommands.Left) && _playerCommands.HasFlag(PlayerCommands.Right))
                _horizontal = 0;
            if (_playerCommands.HasFlag(PlayerCommands.Down) && _playerCommands.HasFlag(PlayerCommands.Up))
                _vertical = 0;
        }

        PlayerAnimator.Instance.playerCommands = _playerCommands;
    }
}