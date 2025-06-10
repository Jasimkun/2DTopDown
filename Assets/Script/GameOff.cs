using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOff : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void OnExitButtonClicked()
    {
        Application.Quit();
        Debug.Log("게임 종료");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}