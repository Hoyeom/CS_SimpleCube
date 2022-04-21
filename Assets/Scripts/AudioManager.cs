using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    public static AudioManager Instance
    {
        get => instance;
    }
    
    
    [SerializeField] private AudioSource fxSource;
    [SerializeField] private AudioSource bgSource;
    
    
    [Header("Player")] [SerializeField] private AudioClip playerTakeDamage;
    [SerializeField] private AudioClip playerFire;
    [SerializeField] private AudioClip playerDead;
    [Header("Enemy")] [SerializeField] private AudioClip enemyTakeDamage;
    [SerializeField] private AudioClip enemyDead;


    
    private PlayerController player;


    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();

        player.OnTakeDamaged += PlayerTakeDamageSound;
        player.OnFireBullet += PlayerFireSound;
        player.OnDeaded += PlayerDeadSound;
    }

    private void PlayerTakeDamageSound()
    {
        fxSource.PlayOneShot(playerTakeDamage);
    }

    private void PlayerDeadSound()
    {
        bgSource.Stop();
        fxSource.PlayOneShot(playerDead);

        
        player.OnTakeDamaged -= PlayerTakeDamageSound;
        player.OnFireBullet -= PlayerFireSound;
        player.OnDeaded -= PlayerDeadSound;
    }

    private void PlayerFireSound()
    {
        fxSource.PlayOneShot(playerFire);
    }

    public void EnemyTakeDamageSound()
    {
        fxSource.PlayOneShot(enemyTakeDamage);
    }

    public void EnemyDeadSound()
    {
        fxSource.PlayOneShot(enemyDead);
    }
}