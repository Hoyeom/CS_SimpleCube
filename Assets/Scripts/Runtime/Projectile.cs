using System;
using System.Collections;
using System.Collections.Generic;
using Runtime;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rigid;

    public Rigidbody GetRigidbody() => rigid;
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public event Action<IAttackAble> OnHitObject;
    public event Action<Projectile> OnDestroyProjectile;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IAttackAble>(out IAttackAble attackAble))
        {
            OnHitObject?.Invoke(attackAble);
        }
        
        OnDestroyProjectile?.Invoke(this);
        Destroy(gameObject);
    }
}
