using UnityEngine;
using System.Collections;
using LitJson;

public class PatternManager : MonoBehaviour
{
    public static PatternManager instance;

    PatternCommand pat_induceBullet, pat_wheelLaser;
    Vector2 playerPos;
    public PhaseEnd data_PhaseEnd;
    public PhaseRestart data_Restart;

    [HideInInspector]
    public bool _isStart;
    private bool _isEnd;
    private int _index;

    //불렛 타입
    [HideInInspector]
    public BulletType BT;
    public int _patternCount;

    private void Awake()
    {
        instance = this;

        data_PhaseEnd.Init();
        data_Restart.Init();
    }

    private void Start()
    {
        pat_induceBullet = new InduceBullet();
        pat_wheelLaser = new WheelLaser();
    }

    private void Update()
    {
        if (_isStart)
        {
            _isStart = false;
            //_idEnd변수를 case문에 넣은 이유는 패이즈 마다 끝나는 패킷을 보내야하는 시점이 다르기 때문이다
            switch (Boss.instance.patternNum)
            {
                case 2:
                    Debug.Log("총알 생성");
                    pat_induceBullet.BulletExecute(Boss.instance._circleBullet, BT);

                    //만약 패턴이 시작된 횟수가 3일때 랜덤 탄환을
                    //다시 계산하도록 한다
                    if (_patternCount == 3)
                    {
                        _patternCount = 0;
                        pat_wheelLaser.Execute(_index);

                    }
                    else
                    {
                        //그게 아니면 그냥 보냄
                        _patternCount++;
                        Invoke("Restart", 0.2f);
                        //_isEnd = true;
                    }
                    break;
                //case 3: // 유도 탄환
                //    pat_induceBullet.Execute(playerPos);
                //    Boss.instance.DelaySendPhaseData(0.5f);
                //    _isEnd = true;
                //    break;
                case 4: // 랜덤 레이저
                    pat_wheelLaser.Execute(_index);
                    Boss.instance.DelaySendPhaseData(0.5f);
                    //_isEnd = true;
                    break;
                default:
                    pat_induceBullet.BulletExecute(Boss.instance._circleBullet, BT);

                    //만약 패턴이 시작된 횟수가 3일때 랜덤 탄환을
                    //다시 계산하도록 한다
                    if (_patternCount == 3)
                    {
                        _patternCount = 0;
                        pat_wheelLaser.Execute(_index);

                    }
                    else
                    {
                        //그게 아니면 그냥 보냄
                        _patternCount++;
                        Invoke("Restart", 0.2f);
                    }
                    break;
            }

        }

        if (_isEnd)
        {
            _isEnd = false;
            Invoke("SendPhaseEnd", 0.2f);
        }
    }
    
    public void TimeDelaySendDelayPhaseEnd(float _time)
    {
        Invoke("SendDelayPhaseEnd", _time);
    }

    //페이즈가 끝났다는 시점을 지정해주기 위한 함수이다
    public void SendDelayPhaseEnd()
    {
        CancelInvoke("SendDelayPhaseEnd");
        _isEnd = true;
    }

    public void LoadRandomLaser(JsonData _data) // 랜덤 레이저를 날릴 인덱스를 Resolve.
    {
        _isStart = true;
        _index = int.Parse(_data["laserDir"].ToString());
    }

    public void LoadInduceBullet(JsonData _data) // 유도 탄환을 날릴 플레이어의 위치를 Resolve.
    {
        _isStart = true;
        playerPos.x = float.Parse(_data["x"].ToString());
        playerPos.y = float.Parse(_data["y"].ToString());
    }

    //원형 탄환의 타입을 셋팅한다
    public void LoadInduceCircleBullet(JsonData _data)
    {
        Debug.Log("총알 결과는? " + int.Parse(_data["bulletType"].ToString()));
        BT = (BulletType)int.Parse(_data["bulletType"].ToString());
    }

    public void PatternRestart()
    {
        _isStart = true;
    }

    public void PatternStart(JsonData _data)
    {
        Boss.instance.patternNum = int.Parse(_data["Phase"].ToString());
        _isStart = true;
    }

    private void SendPhaseEnd()
    {
        CancelInvoke("SendPhaseEnd");
        Debug.Log("보스 패턴 재시작");
        JsonData SendData = JsonMapper.ToJson(data_PhaseEnd);
        ServerClient.instance.Send(SendData.ToString());
    }

    //해당 페이즈를 재시작 해야하는지 물어보는 함수이다(연속된 패턴에서 사용함)
    public void Restart()
    {
        CancelInvoke("Restart");
        JsonData SendData = JsonMapper.ToJson(data_Restart);
        ServerClient.instance.Send(SendData.ToString());
    }

}
