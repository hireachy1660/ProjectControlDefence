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
    private HPBarManager hpBarMng = null;

    private int curGold = 0;

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
}
