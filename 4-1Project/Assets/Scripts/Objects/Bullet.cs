using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//탄막 타입들 몬스터와 캐릭터의 탄막 스크립트를 분리하길 요망 
public enum BulletType
{
    EVEN_CIRCLE_NORMAL,
    ODD_NUMBER_CIRCLE_NORMAL,
    EVEN_CIRCLE_CURVE,
    ODD_NUMBER_CIRCLE_CURVE,
}

public class Bullet : MonoBehaviour
{
    BulletType BT;

    Rigidbody2D _rigidbody2D;
    private float _time;

    public float lifetime = 2.0f;
    public int power;

    public int STR;

    Vector2 dir;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (Boss.instance != null)
            transform.position = Boss.instance.transform.position;
        _rigidbody2D.velocity = Vector2.zero;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        if (_time > lifetime)
            ObjectPoolingManager.instance.InsertQueue(this, ObjectPoolingManager.instance.queue_energyball);

        //받은 총알의 타입별로 계산 방식이 다르다.
        switch (BT)
        {
            case BulletType.EVEN_CIRCLE_NORMAL:
            case BulletType.ODD_NUMBER_CIRCLE_NORMAL:
                NormaCirclelBullet();
                break;
            case BulletType.EVEN_CIRCLE_CURVE:
            case BulletType.ODD_NUMBER_CIRCLE_CURVE:
                CircleCurveBullet();
                break;
            default:
                break;
        }
    }
    private void OnDisable()
    {
        _time = 0;
    }

    //탄막의 타입과 탄막 방향을 지정해준다
    public void InduceBullet(Vector2 _dir, BulletType _bt)
    {
        dir = _dir;
        BT = _bt;
    }

    //일반적인 원형 탄막
    public void NormaCirclelBullet()
    {
        transform.Translate(dir * 7.0f * Time.deltaTime);
    }

    //원형 커브 탄막 
    public void CircleCurveBullet()
    {
        transform.Translate(dir * 7.0f * Time.deltaTime);
        transform.Rotate(0, 0, Time.smoothDeltaTime * 45.0f);
    }

    //현재 3페이즈 때문에 있는 함수임 추후에 삭제 요망함
    public void InduceBullet(Vector2 _dir)
    {
        _rigidbody2D.AddForce(dir * power);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.Attacked(STR);
                ObjectPoolingManager.instance.InsertQueue(this, ObjectPoolingManager.instance.queue_energyball);
                return;
            }
            Player_Server player_Server = collision.GetComponent<Player_Server>();
            if (player_Server != null)
            {
                ObjectPoolingManager.instance.InsertQueue(this, ObjectPoolingManager.instance.queue_energyball);
                return;
            }
        }
    }
}


