using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{

    private Vector3 posA;
    private Vector3 posB;
    private Vector3 nextPos;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Transform childTransfom;
    [SerializeField]
    private Transform transformB;
    // Use this for initialization
    void Start()
    {
        posA = childTransfom.localPosition;
        posB = transformB.localPosition;
        nextPos = posB;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void Move()
    {
        childTransfom.localPosition = Vector3.MoveTowards(childTransfom.localPosition, nextPos, speed * Time.deltaTime);
        if (Vector3.Distance(childTransfom.localPosition, nextPos) <= 0.1)
        {
            ChangeDestination();
        }
    }
    private void ChangeDestination()
    {
        nextPos = nextPos != posA ? posA : posB;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.layer = 9;
            other.transform.SetParent(childTransfom);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.SetParent(null);
        }
    }
}
