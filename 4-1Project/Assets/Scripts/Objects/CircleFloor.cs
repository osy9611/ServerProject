using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFloor : MonoBehaviour
{
    public bool _enterPlayer;   //플레이어가 들어왔는지 확인하는 함수

    Player player;
    Player_Server player_Server;

    private void OnEnable()
    {
        if(player !=null)
        {
            this.gameObject.transform.parent = player.transform;
        }
        else
        {
            this.gameObject.transform.parent = player_Server.transform;
        }
    }

    //플레이어 컴포넌트를 받아온다
    public void PlayerSetOn()
    {
        if (FindObjectOfType<Player>() != null && player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }

    public void OtherPlayerSetOn(Player_Server _player)
    {
        player_Server = _player;
    }

    public bool SetCheck()
    {
        if(player == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.name == GameManager.instance.PlayerName)
        {
            _enterPlayer = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == GameManager.instance.PlayerName)
        {
            _enterPlayer = true;
        }
    }
}
