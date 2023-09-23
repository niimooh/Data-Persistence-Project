using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[Serializable]
public struct BestScore
{
    [SerializeField] public string bestScoreName;
    [SerializeField] public int bestScore;
}

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text currentScoreText;
    public Text currentPlayerNameText;
    
    public Text bestScoreText;
    public Text bestScorePlayerNameText;
    
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;
    private string currentPlayerName;

    private string bestScoreName;
    private int bestScoreValue;


    // Start is called before the first frame update
    void Start()
    { 
        string path = Application.persistentDataPath + "/BestScore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            BestScore bs = JsonUtility.FromJson<BestScore>(json);
            bestScoreName = bs.bestScoreName;
            bestScoreValue = bs.bestScore;

            UpdateBestScoreUI();
        }
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        
        GameObject go = GameObject.Find("Name");
        Name nameComp = go.GetComponent<Name>();
        currentPlayerName = nameComp.playerName;
        currentPlayerNameText.text = $"Name : {currentPlayerName}";
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        currentScoreText.text = $"Score : {m_Points}";
        if (m_Points >= bestScoreValue)
        {
            bestScoreValue = m_Points;
            bestScoreName = currentPlayerName;

            UpdateBestScoreUI();
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
        Save();
    }

    private void UpdateBestScoreUI()
    {
        bestScorePlayerNameText.text = bestScoreName;
        bestScoreText.text = bestScoreValue.ToString();
    }

    private void Save()
    {
        BestScore bestScore = new BestScore();
        bestScore.bestScore = bestScoreValue;
        bestScore.bestScoreName = bestScoreName;

        string json = JsonUtility.ToJson(bestScore);
        string path = Application.persistentDataPath + "/BestScore.json";
        File.WriteAllText(path, json);
    }
}
