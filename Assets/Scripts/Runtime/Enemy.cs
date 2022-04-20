using System;
using DG.Tweening;
using Runtime;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IAttackAble
{
    private Material enemyMat;

    [Header("Component")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform testTarget;
    
    private void Awake()
    {
        enemyMat = GetComponentInChildren<Renderer>().material;
    }

    private void Update()
    {
        agent.SetDestination(testTarget.position);
    }

    public void TakeDamage(int damage)
    {
        transform.DOScale(Vector3.one, .1f)
            .OnComplete(() =>
                transform.DOScale(Vector3.one * 2, .1f)).Restart();

        enemyMat.DOColor(Color.white, .1f)
            .OnComplete(() =>
                enemyMat.DOColor(Color.red, .1f)).Restart();
    }

    public void Attack(IAttackAble attackAble)
    {
        attackAble.TakeDamage(5);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.TryGetComponent<IAttackAble>(out IAttackAble attackAble))
        {
            Attack(attackAble);
        }
    }
}
