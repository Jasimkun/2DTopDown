using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.Experimental.RestService;
using UnityEngine;

[SerializeField]
public class PlayerData
{
    public List<string> collectedItems = new List<string>();
    public int stage = 1;
}

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    public static PlayerData playerData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); //�ߺ� ����
        }
    }
    public void SaveData(PlayerData playerData)
    {
        string filePath = Application.persistentDataPath + "/player_data.json";
        string json = JsonUtility.ToJson(playerData, true);
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("���� ������ ����� : " + json);
    }
    public PlayerData LoadData()
    {
        string filePath = Application.persistentDataPath + "/player_data.json";
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("���� ������ �ε�� : " + json);
            return playerData;
        }
        else
        {
            Debug.LogWarning("����� ���� �����Ͱ� �����ϴ�.");
            return new PlayerData();
        }
    }
}
