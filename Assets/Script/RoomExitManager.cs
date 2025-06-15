using UnityEngine;
using System.Collections.Generic;

public class RoomExitManager : MonoBehaviour
{
    private DungeonManager dungeonManager; // DungeonManager ����
    private int lastPlayerRoomID = -1; // �÷��̾ ���������� �־��� �� ID

    void Start()
    {
        dungeonManager = DungeonManager.GetInstance();
        if (dungeonManager == null)
        {
            Debug.LogError("DungeonManager�� ã�� �� �����ϴ�. RoomExitManager�� ����� �۵����� �ʽ��ϴ�.");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        // ���� ���� �� ���� �� ID�� �ʱ�ȭ
        lastPlayerRoomID = dungeonManager.playerRoomID;
    }

    void Update()
    {
        // �÷��̾��� ���� �� ID�� ����Ǿ����� Ȯ��
        if (dungeonManager.playerRoomID != lastPlayerRoomID)
        {
            // ���� ����Ǿ��ٸ�, ���� ���� ������ ��޴ϴ�.
            LockPreviousRoomDoors(lastPlayerRoomID);

            // ���� �� ID�� ������Ʈ�մϴ�.
            lastPlayerRoomID = dungeonManager.playerRoomID;
        }
    }

    private void LockPreviousRoomDoors(int roomIDToLock)
    {
        // DungeonManager���� �ش� ���� �� ����� �����ɴϴ�.
        if (dungeonManager.doorDic.ContainsKey(roomIDToLock))
        {
            List<Door> doorsInRoom = dungeonManager.doorDic[roomIDToLock];
            foreach (Door door in doorsInRoom)
            {
                // �� ���� LockDoor() �޼��带 ȣ���Ͽ� ��޴ϴ�.
                // Door ��ũ��Ʈ�� LockDoor() �޼��尡 �̹� �ִٰ� �����մϴ�.
                Debug.Log($"������ �����δ� ���ư� �� ���� �� ����.");
            }
        }
        else
        {
            Debug.LogWarning($"�� ID {roomIDToLock}�� �ش��ϴ� �� ����� ã�� �� �����ϴ�.");
        }
    }
}