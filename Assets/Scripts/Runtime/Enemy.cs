using System;
using System.Collections;
using DG.Tweening;
using Runtime;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IAttackAble
{
    private Material enemyMat;

    [SerializeField] private int maxHealth;
    [SerializeField] private int dropScore = 1;
    
    private int curHealth;

    private bool isDead = false;
    
    private int CurHealth
    {
        get => curHealth;
        set
        {
            curHealth = value;
            
            if (curHealth < 1 && !isDead)
            {
                isDead = true;

                collider.enabled = false;
                
                enemyMat.DOColor(new Color(1, 0, 0 ,0), .3f)
                    .OnComplete(() => Destroy(gameObject));

                GameManager.Instance.AddScore(dropScore);
                return;
            }
        }
    }
    
    
    [SerializeField] private float routineDelay = .5f;
    [Header("Component")] [SerializeField] private Collider collider;
    [SerializeField] private NavMeshAgent agent;
    private Transform target;

    private Sequence hitSequence;
    private int attackPower = 1;

    private void Awake()
    {
        GameObject obj = GameObject.FindWithTag("Player");

        if(obj == null) { return; }

        target = obj.transform;
        
        
        enemyMat = GetComponentInChildren<Renderer>().material;
        
        hitSequence = DOTween.Sequence();
        hitSequence.SetAutoKill(false);
        hitSequence.Pause();
        hitSequence.Insert(0, enemyMat.DOColor(Color.white, .1f).SetLoops(2, LoopType.Yoyo));
        hitSequence.Insert(0, transform.DOScale(transform.localScale / 2, 0.1f).SetLoops(2, LoopType.Yoyo));
    }

    private void OnEnable()
    {
        curHealth = maxHealth;
        collider.enabled = true;
        isDead = false;
        StartCoroutine(EnemyRoutine());
    }

    private void OnDisable()
    {
        isDead = true;
    }

    public void TakeDamage(int damage)
    {
        hitSequence.Restart();
        CurHealth -= damage;
    }

    private void Attack(IAttackAble attackAble)
    {
        attackAble.TakeDamage(attackPower);
    }

    private void OnCollisionStay(Collision collision)
    {        
        if (collision.collider.TryGetComponent<IAttackAble>(out IAttackAble attackAble))
        {
            Attack(attackAble);
        }
    }

    private IEnumerator EnemyRoutine()
    {
        while (!isDead)
        {
            if(target != null)
                agent.SetDestination(target.position);
            yield return new WaitForSeconds(routineDelay);
        }
    }
}
