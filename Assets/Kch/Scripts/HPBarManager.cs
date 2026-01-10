using UnityEngine;
using System.Collections.Generic;

public class HPBarManager : MonoBehaviour
{
    private Queue<HPBar> hpPull = new Queue<HPBar>();

    [SerializeField]
    private GameObject hpBarPref = null;

    public void SetHPBar(IDamageable _target)
    {
        HPBar hPBar = null;
        if(hpPull.TryDequeue(out hPBar))
        {
            hPBar.gameObject.SetActive(true);
            hPBar.OffHpBarCallback = OffHpBar;
            hPBar.SetNewHPBar(_target);
        }
        else
        {
            hPBar = Instantiate(hpBarPref).GetComponent<HPBar>();
            hPBar.SetNewHPBar(_target);
            hPBar.OffHpBarCallback = OffHpBar;
            hpPull.Enqueue(hPBar);
        }
    }

    private void OffHpBar(HPBar _target)
    {
        hpPull.Enqueue(_target);
        _target.gameObject.SetActive(false);
    }
}
