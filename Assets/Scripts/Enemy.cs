using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{

    private IEnemyState currentState;

    public GameObject Target { get; set; }

    private Canvas canvasHealth;

    private float meleeRange = 4;

    public bool InMeleeRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }

            return false;
        }
    }

    public override bool IsDead
    {
        get
        {
            return health.CurrentVal <= 0;
        }
    }
    [SerializeField]
    private Transform leftEdge;

    [SerializeField]
    private int Lives;

    [SerializeField]
    private Transform rightEdge;

    private bool dropItem = true;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        this.starPos = transform.position;
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);
        this.Attack = false;
        ChangeState(new IdleState());
        canvasHealth = transform.GetComponentInChildren<Canvas>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!IsDead)
        {
            if (!TakingDamage)
            {
                currentState.Excute();
            }
            LookAtTarget();
        }
    }

    public void RemoveTarget()
    {
        Target = null;
        ChangeState(new PatrolState());
    }

    private void LookAtTarget()
    {
        if (Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x;
            if (xDir < 0 && faceRight || xDir > 0 && !faceRight)
            {
                ChangeDirection();
            }
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.Enter(this);
    }

    public void Move()
    {
        if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x))
        {
            MyAnimator.SetFloat("speed", 1);
            transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
        }
        else if (currentState is PatrolState)
        {
            ChangeDirection();
        }
        else if (currentState is RangedState)
        {
            Target = null;
            ChangeState(new IdleState());
        }
    }

    public Vector2 GetDirection()
    {
        return faceRight ? Vector2.right : Vector2.left;
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "coin")
        {
            Physics2D.IgnoreCollision(other.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
    public override IEnumerator TakeDamage()
    {
        if (!canvasHealth.isActiveAndEnabled)
        {
            canvasHealth.enabled = true;
        }
        health.CurrentVal -= 10;
        if (!IsDead)
        {
            MyAnimator.SetTrigger("damage");
            if (currentState.GetStateName() == "Patrol" || !(currentState.GetStateName() == "Ranged"))
            {
                ChangeDirection();
            }
        }
        else
        {
            if (dropItem)
            {               
                GameObject coin = (GameObject)Instantiate(GameManager.Instance.CoinPrefab, new Vector3(transform.position.x, transform.position.y + 2), Quaternion.identity);
                Physics2D.IgnoreCollision(coin.GetComponent<Collider2D>(), GetComponent<Collider2D>());
                dropItem = false;
            }
            MyAnimator.SetTrigger("die");
            canvasHealth.enabled = false;
        }
        yield return null;
    }

    public override void Death()
    {
        dropItem = true;
        MyAnimator.ResetTrigger("die");
        MyAnimator.ResetTrigger("puffSpawn");
        MyAnimator.SetTrigger("idle");
        health.CurrentVal = health.MaxVal;
        transform.position = starPos;
    }

    public void PlayDeathSoundTwo()
    { 
        SoundManager.Instance.PlaySfx("puff");
    }
    public void PlayDeathSoundThree()
    {      
        SoundManager.Instance.PlaySfx("laught");
    }
}
