using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class EnergyBall : MonoBehaviour
{
    private Rigidbody2D _rigidbody2d;
    private Vector2 _mousePos;

    public float speed, time;
    public int STR = 200;
    
    public BossDamage PtoB_damage_data; // 플레이어가 보스에게 데미지를 넣을 때

    bool _isPlayer;

    private void Awake()
    {
        _rigidbody2d = GetComponent<Rigidbody2D>();

        PtoB_damage_data.Init();
    }

    private void OnEnable()
    {
        time = 0;
        _rigidbody2d.velocity = Vector2.zero;
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time >= 2.0f)
            ObjectPoolingManager.instance.InsertQueue(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Boss")
        {
            if (STR - Boss.instance.DEF > 0)
            {
                PtoB_damage_data.damage = STR - Boss.instance.DEF;
                ObjectPoolingManager.instance.InsertQueue(this);

                if(_isPlayer) // 내가 쏜 공이 맞으면 체력계산
                {
                    JsonData SendData = JsonMapper.ToJson(PtoB_damage_data);
                    ServerClient.instance.Send(SendData.ToString());
                }
          
                Boss.instance.ActiveHPBar();
            }
        }

        if (collision.gameObject.tag == "FireBall")
        {
            if(_isPlayer)
            {
                Boss.instance._fireBall.HitFireBall(collision.gameObject.name);
            }            
            ObjectPoolingManager.instance.InsertQueue(this);
        }      
    }

    public void ShootBall(Vector2 _direction, Vector2 _position,string _name)
    {
        if(_name == GameManager.instance.PlayerName)
        {
            _isPlayer = true;
        }
        else
        {
            _isPlayer = false;
        }
        transform.position = _position;
        _rigidbody2d.AddForce(_direction * speed);
    }
}
