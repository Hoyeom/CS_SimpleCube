using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public static GameManager Instance
    {
        get => instance;
    }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private GameObject gameOverObj;

    private Camera mainCam;

    private int score = 0;

    private float edgeOffset = 5;

    private float spawnInterval = 2;

    private bool isStart = false;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCam = Camera.main;


    }

    public void StartGame()
    {
        isStart = true;
        StartCoroutine(SpawnEnemy());
    }
    
    public void StopGame()
    {
        isStart = false;
        gameOverObj.SetActive(true);
    }

    public void SceneReLoad()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private IEnumerator SpawnEnemy()
    {
        while (isStart)
        {
            yield return new WaitForSeconds(spawnInterval);
            Instantiate(enemyPrefab, GetScreenRandEdgePoint(), Quaternion.identity);
        }
    }

    private Vector3 GetScreenRandEdgePoint()
    {
        Vector2 screenEdge = Vector2.zero;

        Vector3 pos = Vector3.zero;

        Vector3 offset = Vector3.zero;

        float width = Screen.width;
        float height = Screen.height;

        switch (Random.Range(0, 4))
        {
            case 0:
                screenEdge.x = Random.value * width;
                screenEdge.y = height;

                offset = new Vector3(0, 0, edgeOffset);

                break;
            case 1:
                screenEdge.x = Random.value * width;

                offset = new Vector3(0, 0, -edgeOffset);

                break;
            case 2:
                screenEdge.y = Random.value * height;
                screenEdge.x = width + edgeOffset;

                offset = new Vector3(edgeOffset, 0);
                break;
            case 3:
                screenEdge.x = Random.value * width;

                offset = new Vector3(-edgeOffset, 0);
                break;
        }

        Ray ray = mainCam.ScreenPointToRay(screenEdge);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            pos = hit.point + offset;
        }

        return pos;
    }

    public void AddScore(int score)
    {
        scoreText.text = $"{this.score += score}";
    }
}