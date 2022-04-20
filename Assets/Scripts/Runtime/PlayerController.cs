using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour,IAttackAble
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    [SerializeField] private Rigidbody rigid;
    [SerializeField] private Transform fireRig;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 3;
    
    private Transform firePoint;

    private Camera mainCam;

    private Material playerMat;

    private Vector3 inputVector;

    private Vector3 viewDir;

    private int attackPower = 1;

    private float hitDelay = 0.5f;
    private bool isDelay = false;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private int maxHealth = 5;
    private int curHealth;

    private int CurHealth
    {
        get => curHealth;
        set
        {
            curHealth = value;
        }
    }
    

    [SerializeField] private float speed = 3;

    private void Awake()
    {
        CurHealth = maxHealth;

        firePoint = fireRig.GetChild(0);
        
        mainCam = Camera.main;
        playerMat = GetComponentInChildren<Renderer>().material;
        playerMat.EnableKeyword("_EMISSION");
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + inputVector * speed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        LookAtMouse();
    }

    private void LookAtMouse()
    {
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer)) 
        {
            viewDir = hit.point - transform.position;

            fireRig.rotation = Quaternion.LookRotation(viewDir);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDelay) { return; }
        
        isDelay = true;
        
        CurHealth -= damage;

        playerMat.DOColor(Color.red, hitDelay / 2)
            .OnComplete(() =>
                playerMat.DOColor(Color.white, hitDelay / 2)).Restart();

        transform.DOScale(Vector3.one / 2, hitDelay / 2)
            .OnComplete(() =>
                transform.DOScale(Vector3.one, hitDelay / 2)
                    .OnComplete(() => isDelay = false)).Restart();
    }

    private void Attack(IAttackAble attackAble)
    {
        attackAble.TakeDamage(attackPower);
    }

    private void ProjectileDestroyed(Projectile projectile)
    {
        projectile.OnHitObject -= Attack;
        projectile.OnDestroyProjectile -= ProjectileDestroyed;
    }

    public void FireInput(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:

                firePoint.DOScaleY(1, 0.1f)
                    .OnComplete(() =>   
                        firePoint.DOScaleY(0.5f, 0.1f)).Restart();
                
                GameObject obj = Instantiate(projectilePrefab, firePoint.position, fireRig.rotation);

                Projectile projectile = obj.GetComponent<Projectile>(); 
                
                projectile.GetRigidbody().velocity = fireRig.forward * projectileSpeed;

                projectile.OnHitObject += Attack;
                projectile.OnDestroyProjectile += ProjectileDestroyed;

                Destroy(obj, 5);
                
                break;
        }
    }
    
    public void MoveInput(InputAction.CallbackContext context)
    {
        Vector3 contextVector = context.ReadValue<Vector2>();
        
        contextVector.z = contextVector.y;
        contextVector.y = 0;
        
        inputVector = contextVector;
        
        switch (context.phase)
        {
            case InputActionPhase.Started:

                playerMat.DOColor(Color.white, EmissionColor, 0.1f)
                    .OnComplete(() =>
                        playerMat.DOColor(Color.black, EmissionColor, 0.1f)).Restart();
              
                break;
            case InputActionPhase.Performed:

                contextVector.x = Mathf.Abs(contextVector.x);
                contextVector.z = Mathf.Abs(contextVector.z);

                transform.DOScale(Vector3.one + contextVector / 2, .1f)
                    .OnComplete(() =>
                        transform.DOScale(Vector3.one, .1f)).Restart();
                
                break;
            case InputActionPhase.Canceled:
                
                break;
        }
    }
}
