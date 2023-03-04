using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wargame.WeaponSystem;

public class DamageNearby : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(DamageSomeone), 3f, 3f);
    }

    public void DamageSomeone()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 10f);

        foreach (var col in cols)
        {
            if (col.gameObject == gameObject)
                continue;
            
            if (col.TryGetComponent(out Entity e))
            {
                e.ApplyDamage(20);
            }
        }
    }
}
