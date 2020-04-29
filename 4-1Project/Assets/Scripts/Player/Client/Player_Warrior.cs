using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Warrior : MonoBehaviour
{
    private RaycastHit2D _hit2D;
    private Player _mainPlayer;
    
    private bool _isHit;
    public float time, attacktime;

    private void Awake()
    {
        _mainPlayer = GetComponent<Player>();
    }

    private void Update()
    {
        if(time < attacktime + 0.5f)
            time += 0.016f;

        if(Input.GetMouseButtonDown(0))
        {
            if (time > attacktime)
            {
                _mainPlayer.AttackPlayer();
                _hit2D = Physics2D.Raycast(transform.position, _mainPlayer._mousePos, 2f);
                time = 0;
                if (_hit2D.collider != null)
                {
                    if (_hit2D.collider.name == "Boss")
                    {
                        _mainPlayer.SendDamageInfo(Boss.instance.DEF);
                        Boss.instance.ActiveHPBar();
                    }
                }
            }
        }
    }
}
