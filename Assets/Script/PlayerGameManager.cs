using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerGameManager : MonoBehaviour
{

    public TMP_InputField inputField;
    public Button gameStartButton;
    // Start is called before the first frame update
    void Start()
    {
        gameStartButton.onClick.AddListener(OnGameStartButtonClicked);
    }

    private void OnGameStartButtonClicked()
    {
        string playerName = inputField.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("�÷��̾� �̸��� �Է��ϼ���.");
            return;
        }

        PlayerPrefs.SetString("Playername", playerName);
        PlayerPrefs.Save();

        Debug.Log("�÷��̾� �̸��� ����Ǿ����ϴ� : " + playerName);

        SceneManager.LoadScene("Test");
    }
}
