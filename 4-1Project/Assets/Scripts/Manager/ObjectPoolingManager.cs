using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager instance;

    public Queue<Laser> queue_laser = new Queue<Laser>();
    public Queue<Bullet> queue_energyball = new Queue<Bullet>();
    public Queue<EnergyBall> queue_magicBall = new Queue<EnergyBall>();
    public Queue<CircleFloor> queue_circleFloor = new Queue<CircleFloor>();

    public Laser laser;
    public Bullet energyBall;
    public EnergyBall magicBall;
    public CircleFloor circle;
    public int poolingCount = 500;

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < poolingCount; i++)
        {
            Bullet t_object = Instantiate(energyBall, Vector2.zero, Quaternion.identity);
            t_object.transform.parent = gameObject.transform;
            queue_energyball.Enqueue(t_object);
            t_object.gameObject.SetActive(false);
        }

        for (int i = 0; i < 10; i++)
        {
            EnergyBall t_object = Instantiate(magicBall, Vector2.zero, Quaternion.identity);
            t_object.transform.parent = gameObject.transform;
            queue_magicBall.Enqueue(t_object);
            t_object.gameObject.SetActive(false);
        }

        Laser temp = Instantiate(laser, Vector2.zero, Quaternion.identity);
        temp.transform.parent = gameObject.transform;
        queue_laser.Enqueue(temp);
        temp.gameObject.SetActive(false);

        //원형 장판
        CircleFloor floorTemp = Instantiate(circle, Vector2.zero, Quaternion.identity);
        floorTemp.transform.parent = gameObject.transform;
        queue_circleFloor.Enqueue(floorTemp);
        floorTemp.gameObject.SetActive(false);  
    }

    public void InsertQueue(Bullet _object, Queue<Bullet> _queue) // Second Paramerer is put object Queue(poolingmanager queue)
    {
        _queue.Enqueue(_object);
        _object.gameObject.SetActive(false);
    }
    public Bullet GetQueue(Queue<Bullet> _queue)
    {
        if (queue_energyball.Count != 0)
        {
            Bullet t_object = _queue.Dequeue();
            t_object.gameObject.SetActive(true);
            return t_object;
        }
        return null;
    }

    public void InsertQueue(Laser _object, Queue<Laser> _queue) // Second Paramerer is put object Queue(poolingmanager queue)
    {
        _queue.Enqueue(_object);
        _object.gameObject.SetActive(false);
    }
    public Laser GetQueue(Queue<Laser> _queue)
    {
        if (queue_laser.Count != 0)
        {
            Laser t_object = _queue.Dequeue();
            return t_object;
        }
        return null;
    }

    public void InsertQueue(EnergyBall _object) // Second Paramerer is put object Queue(poolingmanager queue)
    {
        queue_magicBall.Enqueue(_object);
        _object.gameObject.SetActive(false);
    }
    public void GetQueue(Vector2 _direction, Vector2 _transform)
    {
        if (queue_magicBall.Count != 0)
        {
            EnergyBall t_object = queue_magicBall.Dequeue();
            t_object.gameObject.SetActive(true);
            t_object.ShootBall(_direction, _transform);
        }
    }

    //원형 장판 풀링
    public void InsertQueue(CircleFloor _object, Queue<CircleFloor> _queue) // Second Paramerer is put object Queue(poolingmanager queue)
    {
        _queue.Enqueue(_object);
        _object.gameObject.SetActive(false);
    }
    public CircleFloor GetQueue(Queue<CircleFloor> _queue)
    {
        if (queue_laser.Count != 0)
        {
            CircleFloor t_object = _queue.Dequeue();
            return t_object;
        }
        return null;
    }
}
