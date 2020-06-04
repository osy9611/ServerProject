using UnityEngine;
using System.Collections;
using LitJson;

public class PatternManager : MonoBehaviour
{
    public static PatternManager instance;

    PatternCommand pat_induceBullet, pat_wheelLaser, pat_circleFloor, pat_fireBall, pat_restriction;

    Vector2 playerPos;
    public PhaseEnd data_PhaseEnd;
    public PhaseRestart data_Restart;
    public PhaseTimeEnd data_PhaseTimeEnd;

    [HideInInspector]
    public bool _isStart;
    public bool _subIsStart = false;
    private bool _isEnd;
    private int _index;

    //불렛 타입
    [HideInInspector]
    public BulletType BT;
    public BulletType SBT;
    public int _patternCount;
    public int _subPatternCount;

    //원형 장판을 위한 함수
    public string _circleFloorTargetName;
    public bool _limitTimeOn;

    //불구슬 용
    bool _setOn;
    public int _time;

    //타이머
    float _timer;

    // 속박
    public string restricTargetname;

    private void Awake()
    {
        instance = this;

        data_PhaseEnd.Init();
        data_Restart.Init();
        data_PhaseTimeEnd.Init();
    }

    private void Start()
    {
        pat_induceBullet = new InduceBullet();
        pat_wheelLaser = new WheelLaser();
        pat_circleFloor = new InduceCircleFloor();
        pat_fireBall = new InduceFireBall();
        pat_restriction = new RestrictionPattern();
    }

    private void Update()
    {
        if (Boss.instance.HP <= 0)
            gameObject.GetComponent<PatternManager>().enabled = false;
        if (_isStart)
        {
            _isStart = false;
            //_idEnd변수를 case문에 넣은 이유는 패이즈 마다 끝나는 패킷을 보내야하는 시점이 다르기 때문이다
            switch (Boss.instance.patternNum)
            {
                case 2:
                    PatternBallExecute();
                    break;
                case 4: // 랜덤 레이저
                    pat_wheelLaser.Execute(_index);
                    break;
                case 8:
                    PatternFireBallExecute();
                    break;
                case 12:
                    PatternCircleFloorExecute();
                    break;
                case 21:
                    PatternRestriction();
                    break;
                default:
                    PatternBallExecute();
                    break;
            }
        }

        if (_isEnd)
        {
            _isEnd = false;
            Invoke("SendPhaseEnd", 0.2f);
        }

        if (_subIsStart)
        {
            Invoke("SubPatternBallExecute", 0.3f);
        }
    }

    /*패턴 관련된 함수들 정리*/
    #region PatternExecutes

    void PatternBallExecute()
    {
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
    }

    void SubPatternBallExecute()
    {
        CancelInvoke("SubPatternBallExecute");
        if (Boss.instance.patternNum != 2)
        {
            if (SBT == BulletType.SQUARE_CURVE || SBT == BulletType.SQUARE_NORMAL)
            {
                pat_induceBullet.BulletExecute(Boss.instance._circleBullet, SBT);
                _subIsStart = false;
            }
            else
            {
                if (_subPatternCount == 4)
                {
                    _subPatternCount = 0;
                    _subIsStart = false;
                }
                else
                {
                    _subPatternCount++;
                    pat_induceBullet.BulletExecute(Boss.instance._circleBullet, SBT);
                }
            }
        }
    }

    void PatternFireBallExecute()
    {
        if (!_subIsStart)
        {
            _subIsStart = true;
        }

        if (!_limitTimeOn)
        {
            if (!_setOn)
            {
                Boss.instance.DelaySendPhaseData(0.5f);
            }
            else
            {
                pat_fireBall.Execute(_time);
            }
        }
        else
        {
            pat_fireBall.Execute();
        }

    }

    void PatternCircleFloorExecute()
    {
        if (!_subIsStart)
        {
            _subIsStart = true;
        }

        if (!_limitTimeOn)
        {
            //타겟 이름이 없을 경우 서버에다 타겟을 누가 정할지 알려준다
            if (_circleFloorTargetName == "")
            {
                Boss.instance.DelaySendPhaseData(0.5f);
            }
            else if (_circleFloorTargetName != "")
            {
                pat_circleFloor.Execute(_circleFloorTargetName);
            }
        }
        else
        {
            //죽이라고 서버에서 지시하면 클라는 바로 캐릭터가 범위에 있는지를
            //확인하고 죽여버린다
            pat_circleFloor.Execute();
        }
    }

    private void PatternRestriction()
    {
        if(restricTargetname != "")
            pat_restriction.Execute(restricTargetname);
        else
            Boss.instance.DelaySendPhaseData(0.5f);
    }
    #endregion
    /***********************/


    //오브젝트 셋팅 관련
    #region LoadInduce
    public void LoadRandomLaser(JsonData _data) // 랜덤 레이저를 날릴 인덱스를 Resolve.
    {
        _isStart = true;
        _index = int.Parse(_data["laserDir"].ToString());
    }

    //원형 탄환의 타입을 셋팅한다
    public void LoadInduceCircleBullet(JsonData _data)
    {
        BT = (BulletType)int.Parse(_data["bulletType"].ToString());
    }

    //원형 장판 셋팅
    public void LoadInduceCircleFloor(JsonData _data)
    {
        _circleFloorTargetName = _data["targetName"].ToString();
        SBT = (BulletType)int.Parse(_data["bulletType"].ToString());
        _isStart = true;
    }

    //불구슬 셋팅
    public void LoadInuceFirBall(JsonData _data)
    {
        _time = int.Parse(_data["millTime"].ToString());
        SBT = (BulletType)int.Parse(_data["bulletType"].ToString());
        _setOn = true;
        _isStart = true;
    }

    // 속박 셋팅
    public void LoadRestriction(JsonData _data)
    {
        restricTargetname = _data["targetName"].ToString();
        Debug.Log(restricTargetname);
        _isStart = true;
    }
    #endregion

    //서버에서 패턴을 재시작하라고 받으면 재시작을 위한 함수
    public void PatternRestart()
    {
        Boss.instance.Attack();
        _isStart = true;
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
        _limitTimeOn = false;
        _circleFloorTargetName = "";
        restricTargetname = "";
    }

    //패턴을 셋팅하고 그 패턴을 실행하는 함수
    public void PatternStart(JsonData _data)
    {
        Boss.instance.patternNum = int.Parse(_data["Phase"].ToString());
        Boss.instance.Attack();
        _isStart = true;
    }

    //시간 제한이 걸려있는 패턴일 경우에는 타이머 체크를 하고 서버로 보내주는 역할을 한다
    public void DelayPhaseTimeEnd(float _time)
    {
        Invoke("SendPhaseTimeEnd", _time);
    }

    private void SendPhaseTimeEnd()
    {
        CancelInvoke("SendPhaseTimeEnd");
        JsonData SendData = JsonMapper.ToJson(data_PhaseTimeEnd);
        ServerClient.instance.Send(SendData.ToString());
    }

    public void LimitTimeOn()
    {
        _limitTimeOn = true;
        _isStart = true;
    }

    private void SendPhaseEnd()
    {
        CancelInvoke("SendPhaseEnd");
        JsonData SendData = JsonMapper.ToJson(data_PhaseEnd);
        ServerClient.instance.Send(SendData.ToString());
    }

    //해당 페이즈를 재시작 해야하는지 물어보는 함수이다(연속된 패턴에서 사용함)
    public void SendServerRestart()
    {
        CancelInvoke("SendServerRestart");
        JsonData SendData = JsonMapper.ToJson(data_Restart);
        ServerClient.instance.Send(SendData.ToString());
    }

    public void Restart()
    {
        CancelInvoke("Restart");
        _isStart = true;
    }
}