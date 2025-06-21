using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Maze : MonoBehaviour
{

    [Header("�̷� ����")]
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

        // ��� ���� 'Wall'(��)�� �ʱ�ȭ
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
                if (x % 2 == 0 || y % 2 == 0) //x�� ¦���̰ų� y�� ¦���� ��
                {
                    continue;   //���� �ʱ�
                }
                if (y == m_iSize - 2 && x == m_iSize - 2)  //������
                {
                    continue;   //���� �ʱ�
                }
                if (y == m_iSize - 2) //���� ��ġ���� ���������� ���� �վ��� �� �̷� �ѷ��� ���� �� ������
                {
                    m_ppMazeMap[y , x + 1] = e_BlockType.Road; //���������� �ձ�
                    continue;
                }
                if (x == m_iSize - 2) //���� ��ġ���� ���������� ���� �վ��� �� �̷� �ѷ��� ���� �� ������
                {
                    m_ppMazeMap[y + 1 , x] = e_BlockType.Road; //���������� �ձ�
                    continue;
                }
                if (UnityEngine.Random.Range(0, 1) == 0)  //���� ���� �� 50% Ȯ���� ������ �Ǵ� �Ʒ����� �ձ�
                {
                    m_ppMazeMap[y , x + 1] = e_BlockType.Road; //������ ���� �ձ�
                }
                else
                {
                    m_ppMazeMap[y + 1 , x] = e_BlockType.Road; //�Ʒ��� ���� �ձ�
                }
            }
        }
    }
}
