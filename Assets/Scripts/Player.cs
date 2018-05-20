using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DeadEventHandler();

public class Player : Character
{
    private static Player instance;
    public static Player Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
    }

    [SerializeField]
    private bool airControl;

    private float direction;

    private bool move;

    private float btnHorizontal;
    [SerializeField]
    private float LevelHeight;
    [SerializeField]
    private Transform[] groundPoints;
    [SerializeField]
    private float groundRadius;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private LayerMask whatIsGround;

    public Rigidbody2D MyRigidbody;

    private bool inmortal = false;
    public bool onAir = false;

    [SerializeField]
    private float inmortalTime;

    private SpriteRenderer spriteRenderer;

    public bool Slide { get; set; }

    public bool Jump { get; set; }

    public bool OnGround { get; set; }

    private bool diying = false;

    public event DeadEventHandler Dead;

    public bool IsFalling
    {
        get
        {
            return MyRigidbody.velocity.y < -1;
        }
    }

    public bool HasKey { get; set; }

    public int MaracasCount;

    public override bool IsDead
    {
        get
        {
            if (health.CurrentVal <= 0)
            {
                OnDead();
            }

            return health.CurrentVal <= 0;
        }
    }

    public int Lives { get; set; }

    public override void Start()
    {
        Lives = 3;
        HasKey = false;
        base.Start();
        starPos = transform.position;
        MyRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }
    void Update()
    {
        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y <= LevelHeight)
            {
                Death();
            }
            handleImput();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!TakingDamage && !IsDead)
        {
            var horizontal = Input.GetAxis("Horizontal");

            OnGround = IsGrounded();
            if (move)
            {
                this.btnHorizontal = Mathf.Lerp(btnHorizontal, direction, Time.deltaTime * 2);
                HandleMovement(btnHorizontal);
                Flip(direction);
            }
            else
            {
                HandleMovement(horizontal);

                Flip(horizontal);
            }
            HandleLayers();
        }

    }
    public void OnDead()
    {
        if (Dead != null)
        {
            Dead();
        }
    }
    private void HandleMovement(float horizontal)
    {
        if (IsFalling)
        {
            gameObject.layer = 10;
            ; MyAnimator.SetBool("land", true);
        }
        if (!Attack && !Slide && (OnGround || airControl))
        {
            MyRigidbody.velocity = new Vector2(horizontal * movementSpeed, MyRigidbody.velocity.y);
        }
        if (Jump && MyRigidbody.velocity.y == 0)
        {
            MyRigidbody.AddForce(new Vector2(0, jumpForce));
        }
        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));
    }

    private void Flip(float horizontal)
    {
        if (!Jump && !this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Slide") && (horizontal > 0 && !faceRight || horizontal < 0 && faceRight))
        {
            ChangeDirection();
        }
    }

    private void handleImput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsGrounded())
            {
                MyAnimator.SetTrigger("attack");
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            MyAnimator.SetTrigger("slide");
        }
        if (Input.GetKeyDown(KeyCode.Space) && !IsFalling)
        {
            onAir = true;
            MyAnimator.SetTrigger("jump");
        }   
    }

    private bool IsGrounded()
    {
        if (MyRigidbody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject != gameObject)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private void HandleLayers()
    {
        if (!OnGround)
        {
            MyAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            MyAnimator.SetLayerWeight(1, 0);
        }
    }
    private IEnumerator IndicateInmortal()
    {
        while (inmortal)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(.1f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
    }
    public override IEnumerator TakeDamage()
    {
        if (!inmortal)
        {
            health.CurrentVal -= 10;
            if (!IsDead)
            {
                SoundManager.Instance.PlaySfx("12");
                MyAnimator.SetTrigger("damage");
                inmortal = true;
                StartCoroutine(IndicateInmortal());
                yield return new WaitForSeconds(inmortalTime);
                inmortal = false;
            }
            else
            {
                SoundManager.Instance.PlaySfx("goblinLaught");

               SoundManager.Instance.PlaySfx("death");
                DeleteLife();
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("die");
                MyAnimator.SetTrigger("tomb");
            }
        }
    }

    public override void Death()
    {
        diying = false;
        MyRigidbody.velocity = Vector2.zero;
        MyAnimator.SetTrigger("idle");
        health.CurrentVal = health.MaxVal;
        transform.position = starPos;
    }

    public void BtnJump()
    {
        onAir = true;
        MyAnimator.SetTrigger("jump");
    }
    public void BtnAttack()
    {
        MyAnimator.SetTrigger("attack");
    }
    public void BtnMove(float direction)
    {
        this.direction = direction;
        this.move = true;

    }
    public void BtnStopMove()
    {
        this.direction = 0;
        move = false;
        this.btnHorizontal = 0;
    }

    public void DeleteLife()
    {
        Lives--;
        GameManager.Instance.DeleteLive(this.Lives);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "coin")
        {
            SoundManager.Instance.PlaySfx("coin");
            GameManager.Instance.collectedCoins++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Cross")
        {
            starPos = transform.position;
            SoundManager.Instance.PlaySfx("live");
            health.CurrentVal += 30;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Maraca")
        {
            SoundManager.Instance.PlaySfx("pick");
            GameManager.Instance.collectedCollectibles++;
            MaracasCount++;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Cave")
        {
            GameManager.Instance.LoadLevel1();
        }
        else if (other.gameObject.tag == "Key")
        {
            HasKey = true;
            GameManager.Instance.ShowKey();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Spikes" && !diying)
        {
            SoundManager.Instance.PlaySfx("spikes");
            SoundManager.Instance.PlaySfx("death");
            diying = true;
            DeleteLife();
            health.CurrentVal -= health.MaxVal;
            MyAnimator.SetLayerWeight(1, 0);
            MyAnimator.SetTrigger("die");
            MyAnimator.SetTrigger("tomb");
        }
        else if (other.gameObject.tag == "Gate" && HasKey)
        {
            GameManager.Instance.ShowWinMenu();
        }
    }
    public void FallingTombSound()
    {
        SoundManager.Instance.PlaySfx("falling");
    }
    public void PlacedStoneSound()
    {
        SoundManager.Instance.PlaySfx("fall");
    }
}
