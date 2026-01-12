using System.Collections;
using System.Runtime.CompilerServices;
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
    private TextMeshProUGUI WaveEnemyCount = null;
    [SerializeField]
    private HPBarManager hpBarMng = null;

    private int curGold = 0;
    private float timeInTimer = 0f;

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
            timer.text = timeInTimer;
            yield return null;
        }
    }
}
