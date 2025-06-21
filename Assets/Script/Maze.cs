using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Maze : MonoBehaviour
{

    [Header("미로 설정")]
    public int m_iSize = 21;

    public enum e_BlockType
    {
        Wall, Road
    }

    public e_BlockType[,] m_ppMazeMap;

    void Awake()
    {
        if (m_iSize % 2 == 0)
        {
            m_iSize++;
        }

        InitializeMazeMap();
    }

    void InitializeMazeMap()
    {
        m_ppMazeMap = new e_BlockType[m_iSize, m_iSize];

        // 모든 셀을 'Wall'(벽)로 초기화
        for (int y = 0; y < m_iSize; y++)
        {
            for (int x = 0; x < m_iSize; x++)
            {
                m_ppMazeMap[y, x] = e_BlockType.Wall;
            }
        }
    }

    void GenerateMazeBinaryTree()
    {
        for (int y = 0; y < m_iSize; y++)
        {
            for (int x = 0; x < m_iSize; x++)
            {
                if (x % 2 == 0 || y % 2 == 0) //x가 짝수이거나 y가 짝수일 때
                {
                    continue;   //뚫지 않기
                }
                if (y == m_iSize - 2 && x == m_iSize - 2)  //목적지
                {
                    continue;   //뚫지 않기
                }
                if (y == m_iSize - 2) //현재 위치에서 세로축으로 벽을 뚫었을 때 미로 둘레를 뚫을 수 있으면
                {
                    m_ppMazeMap[y , x + 1] = e_BlockType.Road; //가로축으로 뚫기
                    continue;
                }
                if (x == m_iSize - 2) //현재 위치에서 가로축으로 벽을 뚫었을 때 미로 둘레를 뚫을 수 있으면
                {
                    m_ppMazeMap[y + 1 , x] = e_BlockType.Road; //세로축으로 뚫기
                    continue;
                }
                if (UnityEngine.Random.Range(0, 1) == 0)  //난수 생성 후 50% 확률로 오른쪽 또는 아래쪽을 뚫기
                {
                    m_ppMazeMap[y , x + 1] = e_BlockType.Road; //오른쪽 길을 뚫기
                }
                else
                {
                    m_ppMazeMap[y + 1 , x] = e_BlockType.Road; //아래쪽 길을 뚫기
                }
            }
        }
    }
}
