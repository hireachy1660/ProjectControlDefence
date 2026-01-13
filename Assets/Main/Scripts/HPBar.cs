using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public delegate void OffHpBardelegate(HPBar _target);

    private OffHpBardelegate offHpBarCallback;

    public OffHpBardelegate OffHpBarCallback
    {
        set { offHpBarCallback = value; }
    }

    private IDamageable myTarget = null;
    [SerializeField]
    private Image hpBar = null;

    public void SetNewHPBar(IDamageable _target)
    {
        myTarget = _target;
    }

    private void Update()   // 액션으로 변경 필요
    {
        SetCurHp();
        transform.position = Camera.main.WorldToScreenPoint(myTarget.transform.position) + (Vector3.up * 10f);
    }

    public void SetCurHp()
    {
        if(myTarget.CurHealth <= 0f|| myTarget == null)
        {
            offHpBarCallback?.Invoke(this);
        }
        else
        {
        hpBar.fillAmount = myTarget.CurHealth / myTarget.MaxHealth;

        }
    }

    
}
