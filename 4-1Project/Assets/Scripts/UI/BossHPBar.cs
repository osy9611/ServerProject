using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Image[] image_HPgauge; // 0번이 제일 위에 표시되도록
    public Text text_phase;

    private float bossHP;
    private float phaseFullHP;
    private float remainderHP;

    public Color32[] HPGaugeColor;

    public int phase;

    private void Start()
    {
        bossHP = Boss.instance.HP; // 보스의 총 HP를 가져옴
        phaseFullHP = bossHP / phase; // 한 페이즈당 보스의 최대체력

        remainderHP = phaseFullHP;
        image_HPgauge[0].color = HPGaugeColor[9];
        image_HPgauge[1].color = HPGaugeColor[8];
        gameObject.SetActive(false);
    }

    private void Update()
    {
        bossHP = Boss.instance.HP;

        phase = (int)(bossHP / phaseFullHP); // 현재 보스 HP에서 페이즈당 최대HP를 나눠서 몇 페이즈인지 계산
        remainderHP = bossHP % phaseFullHP; //  페이즈에 남은 HP

        if (phase != 0)
            text_phase.text = "X" + phase.ToString();
        else
            text_phase.text = "";

        Debug.Log(remainderHP);
        Debug.Log(phaseFullHP);
        image_HPgauge[0].fillAmount = remainderHP / phaseFullHP;

        // 0번 페이즈는 투명페이즈.
        image_HPgauge[0].color = HPGaugeColor[Mathf.Clamp(phase + 1,0,10)]; // 최대값이 페이즈
        image_HPgauge[1].color = HPGaugeColor[Mathf.Clamp(phase,0,9)]; // 최대값이 페이즈 - 1
        
    }
}
