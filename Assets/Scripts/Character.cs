using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class Character : MonoBehaviour
{

    [SerializeField]
    protected Stat health;

    public abstract bool IsDead { get; }

    [SerializeField]
    public float movementSpeed;

    protected bool faceRight;

    public bool Attack { get; set; }

    public Animator MyAnimator { get; private set; }

    public bool TakingDamage { get; set; }

    public Vector2 starPos;

    public EdgeCollider2D SwordCollider
    {
        get
        {
            return swordCollider;
        }     
    }

    [SerializeField]
    private EdgeCollider2D swordCollider;

    [SerializeField]
    private List<string> damageSources;

    // Use this for initialization
    public virtual void Start()
    {
        health.Initialize();
        Attack = false;
        faceRight = true;
        this.MyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public abstract IEnumerator TakeDamage();

    public abstract void Death();

    public void ChangeDirection()
    {
        faceRight = !faceRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void MeleeAttack()
    {
        SwordCollider.enabled = true;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage());
        }
    }

}
