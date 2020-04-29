using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFloor : MonoBehaviour
{
    public bool _enterPlayer;   //플레이어가 들어왔는지 확인하는 함수
    
    public string _target;
    

    private void OnEnable()
    {
        if(PatternManager.instance!=null)
        {
            _target = PatternManager.instance._circleFloorTargetName;
            transform.parent = null;
        
            if (_target == GameManager.instance.PlayerName)
            {
                transform.parent = GameManager.instance._player.transform;
                transform.localPosition = Vector2.zero;
            }
            else
            {
                transform.parent = OtherPlayerManager.instance.PlayerList[_target].transform;
                transform.localPosition = Vector2.zero;
            }

            PatternManager.instance.DelayPhaseTimeEnd(3.5f);
        }
    }


    private void OnDisable()
    {
        transform.position = Vector2.zero;
        _target = "";
        PatternManager.instance.TimeDelaySendDelayPhaseEnd(0.0f);
    }
   

    public void OtherPlayerSetOn(string _name)
    {
        _target = _name;
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

    //범위에 있는지 아니면 밖에 있는지 확인한다 만약에 밖이면 피통0
    public void InstanceDeathCheck()
    {
        if (_target != GameManager.instance.PlayerName)
        {
            if (!_enterPlayer)
            {
                GameManager.instance._player.Attacked(9999);
            }
        }
        ObjectPoolingManager.instance.InsertQueue(this, ObjectPoolingManager.instance.queue_circleFloor);
    }
}
