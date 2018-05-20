using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winScript : MonoBehaviour {
    [SerializeField]
    private GameObject needKey;

    private bool playOnce = true;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GetPlayerDistance();

    }


    public void GetPlayerDistance()
    {
        float dist = Mathf.Abs(Player.Instance.transform.position.x - transform.position.x);
        if (dist < 5 && Player.Instance.HasKey)
        {
            needKey.SetActive(false);
            GameManager.Instance.ShowWinMenu();
        }
        if (dist < 3 && !Player.Instance.HasKey && playOnce ==true)
        {
            playOnce = false;
            SoundManager.Instance.PlaySfx("closedDoor");
        }
        else if (dist <= 15 && !Player.Instance.HasKey)
        {
            needKey.SetActive(true);
        }
        else
        {
            playOnce = true;
            needKey.SetActive(false);
        }
    }
}
