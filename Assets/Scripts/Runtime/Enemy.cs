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

    private bool Deaded = false;
    
    private int CurHealth
    {
        get => curHealth;
        set
        {
            curHealth = value;
            
            if (curHealth < 1 && !Deaded)
            {
                Deaded = true;

                collider.enabled = false;
                
                enemyMat.DOColor(new Color(1, 0, 0 ,0), .3f)
                    .OnComplete(() => Destroy(gameObject));

                GameManager.Instance.AddScore(dropScore);
                return;
            }

            hitSequence.Restart();

            // itSequence.Play();
            //
            // transform.DOScale(Vector3.one, .1f)
            //     .OnComplete(() =>
            //         transform.DOScale(Vector3.one * 2, .1f)).Restart();
            //
            // enemyMat.DOColor(Color.white, .1f)
            //     .OnComplete(() =>
            //         enemyMat.DOColor(Color.red, .1f)).Restart();
        }
    }
    
    
    [SerializeField] private float routineDelay = 1;
    [Header("Component")] [SerializeField] private Collider collider;
    [SerializeField] private NavMeshAgent agent;
    private Transform target;

    private Sequence hitSequence;

    private void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
        
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
        Deaded = false;
        StartCoroutine(EnemyRoutine());
    }

    private void OnDisable()
    {
        Deaded = true;
    }

    public void TakeDamage(int damage)
    {
        CurHealth -= damage;
    }

    private void Attack(IAttackAble attackAble)
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

    private IEnumerator EnemyRoutine()
    {
        while (!Deaded)
        {
            agent.SetDestination(target.position);
            yield return new WaitForSeconds(routineDelay);
        }
    }
}
