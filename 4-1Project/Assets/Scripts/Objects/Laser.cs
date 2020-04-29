using UnityEngine;
using System.Collections;

public enum LaserType
{
    WHEELLASER
}

public class Laser : MonoBehaviour
{
    private LaserType LT;
    public float _speed = 100.0f;
    public bool _on = false;
    private int _setDir = 0;

    void Update()
    {
        if (_on)
        {
            transform.Rotate(0, 0, Time.deltaTime * _speed);
            if (transform.rotation.eulerAngles.z >= 180.0f)
            {
                ObjectPoolingManager.instance.InsertQueue(this, ObjectPoolingManager.instance.queue_laser);
                PatternManager.instance.TimeDelaySendDelayPhaseEnd(0.1f);
                Boss.instance.DelaySendPhaseData(0.5f);
            }
        }
    }

    private void OnEnable()
    {
        //this.transform.rotation = Quaternion.identity;
        if (PatternManager.instance != null)
        {
            this.transform.position = PatternManager.instance.transform.position;
        }
    }

    private void OnDisable()
    {
        _on = false;
        this.transform.rotation = Quaternion.identity;
    }

    public void WheelNow()
    {
        _on = true;
    }
}
