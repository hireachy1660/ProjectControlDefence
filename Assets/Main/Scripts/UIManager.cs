using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Child Managers")]
    [SerializeField]
    private UICardManager cardMng = null;
    // 유아이 미니맵 매니저, 
    [SerializeField]
    private TextMeshProUGUI curGoldTMP = null;
    [SerializeField]
    private TextMeshProUGUI timer = null;
    [SerializeField]
    private TextMeshProUGUI WaveEnemyCountTMP = null;
    [SerializeField]
    private HPBarManager hpBarMng = null;
    
    private int curGold = 0;
    private float timeInTimer = 0f;
    private int allEnemyInWave = 0;
    private int remaining = 0;
    private StringBuilder stageSB = new StringBuilder();
    private int maxWaveNum = 0;
    private int curWaveNum = 0;

    #region 라이프 사이클

    #endregion


    public void UpdateGold(int _curGold)
    {
        curGold = _curGold;
        curGoldTMP.text = curGold.ToString();
    }

    public void SetHPBar(IDamageable _target)
    {
        hpBarMng.SetHPBar(_target);
    }

    public IEnumerator StartTimer(float _time)
    {
        timeInTimer = _time;
        while (timeInTimer > 0f)
        {
            timeInTimer -= Time.deltaTime;
            timer.text = timeInTimer.ToString("N2");
            yield return null;
        }
    }

    public void SetWaveEnemyCount(int  _count, int _maxWaveNum)
    {
        maxWaveNum = _maxWaveNum;
        allEnemyInWave = _count;
        curWaveNum++;

        stageSB.Clear();
        stageSB.AppendFormat("Stage : {0} / {1}\n", curWaveNum, maxWaveNum);
        stageSB.AppendFormat("Left Enemy is : {0} / {1}\n", remaining, allEnemyInWave);
        WaveEnemyCountTMP.text = stageSB.ToString();
        remaining = 0;
    }

    public void UpdataEnemyCount()
    {
        remaining++;
        stageSB.Clear();
        stageSB.AppendFormat("Stage : {0} / {1}\n", curWaveNum, maxWaveNum);
        stageSB.AppendFormat("Left Enemy is : {0} / {1}\n", remaining, allEnemyInWave);
        WaveEnemyCountTMP.text = stageSB.ToString();
    }
}
