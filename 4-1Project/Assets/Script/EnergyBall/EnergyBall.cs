using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    public float Speed;
    public string Name;
    public float Damage;
    
    public Player_Wizard player;
    public OhterPlayer_Actor otherPlayer;

    public Vector2 Pos;

    string PlayerName;

    private void Start()
    {
        if(player==null && otherPlayer ==null)
        {
            player = gameObject.transform.root.GetComponent<Player_Wizard>();
            otherPlayer = gameObject.transform.root.GetComponent<OhterPlayer_Actor>();

            this.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        transform.position = transform.root.position;
        if (player != null)
        {
            Pos = CalcDir(player.actor.MousePos);
        }
        else if (otherPlayer != null)
        {
            Pos = CalcDir(otherPlayer.AttackDir);
        }

        //2초 후에 오브젝트를 끈다
        Invoke("OffBall", 2.0f);
    }

    private void OffBall()
    {
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.poolingManager.ObjSet(Name, this.gameObject);
        }
        else if (otherPlayer != null)
        {
           otherPlayer.poolingManager.ObjSet(Name, this.gameObject);
        }
    }

    private Vector2 CalcDir(Vector2 mousePos)
    {
        Vector2 vRot;
        if (player != null)
        {
            vRot = mousePos - (Vector2)player.transform.root.position;
        }
        else
        {
            vRot = mousePos - (Vector2)otherPlayer.transform.position;
        }
        return vRot;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Pos * Speed * Time.deltaTime);
    }
}
