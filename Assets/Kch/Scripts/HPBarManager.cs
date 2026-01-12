using UnityEngine;
using System.Collections.Generic;

public class HPBarManager : MonoBehaviour
{
    private Queue<HPBar> hpPull = new Queue<HPBar>();

    [SerializeField]
    private GameObject hpBarPref = null;

    public void SetHPBar(IDamageable _target)
    {
        HPBar hpBar = null;
        if(hpPull.TryDequeue(out hpBar))
        {
            hpBar.gameObject.SetActive(true);
            hpBar.OffHpBarCallback = OffHpBar;
            hpBar.SetNewHPBar(_target);
        }
        else
        {
            hpBar = Instantiate(hpBarPref,transform).GetComponent<HPBar>();
            hpBar.SetNewHPBar(_target);
            hpBar.OffHpBarCallback = OffHpBar;
        }

    }

    private void OffHpBar(HPBar _target)
    {
        hpPull.Enqueue(_target);
        _target.gameObject.SetActive(false);
    }
}
