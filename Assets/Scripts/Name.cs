using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Name : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private InputField nameText;
    public string playerName;
    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        startButton.onClick.AddListener(OnClickButton);
    }
    
    private void OnClickButton()
    {
        if (nameText.text.Contains(" ") || nameText.text.Length <= 0)
            return;
        playerName = nameText.text;
        SceneManager.LoadScene("main");
    }
}
