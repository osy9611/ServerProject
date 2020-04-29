using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Magician : MonoBehaviour
{
    public static Player_Magician instance;

    private Player _mainPlayer;

    private Vector2 _mousePos;

    public float time, attacktime;

    private void Awake()
    {
        instance = this;

        _mainPlayer = GetComponent<Player>();
    }

    private void Update()
    {
        if (time < attacktime + 0.5f)
            time += 0.016f;

        if(Input.GetMouseButtonDown(0))
        {
            if(time > attacktime)
            {
                _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _mousePos -= (Vector2)transform.position;
                _mousePos.Normalize();

                _mainPlayer.Data.ax = _mousePos.x;
                _mainPlayer.Data.ay = _mousePos.y;

                time = 0;
                ObjectPoolingManager.instance.GetQueue(_mousePos, transform.position);
                _mainPlayer.AttackPlayer();
            }
        }
    }
}
