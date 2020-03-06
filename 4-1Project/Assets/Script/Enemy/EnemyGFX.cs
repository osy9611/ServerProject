using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{
    public AIPath aiPath;

    // Start is called before the first frame update
    void Start()
    {
        aiPath = GetComponentInParent<AIPath>();
    }

    // Update is called once per frame
    void Update()
    {
        if(aiPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
        }
        else if(aiPath.desiredVelocity.x<=-0.01f)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1f);
        }
    }
}
