using UnityEngine;
using System.Collections.Generic;

public class RoomExitManager : MonoBehaviour
{
    private DungeonManager dungeonManager; // DungeonManager 참조
    private int lastPlayerRoomID = -1; // 플레이어가 마지막으로 있었던 방 ID

    void Start()
    {
        dungeonManager = DungeonManager.GetInstance();
        if (dungeonManager == null)
        {
            Debug.LogError("DungeonManager를 찾을 수 없습니다. RoomExitManager가 제대로 작동하지 않습니다.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        // 게임 시작 시 현재 방 ID를 초기화
        lastPlayerRoomID = dungeonManager.playerRoomID;
    }

    void Update()
    {
        // 플레이어의 현재 방 ID가 변경되었는지 확인
        if (dungeonManager.playerRoomID != lastPlayerRoomID)
        {
            // 방이 변경되었다면, 이전 방의 문들을 잠급니다.
            LockPreviousRoomDoors(lastPlayerRoomID);

            // 현재 방 ID를 업데이트합니다.
            lastPlayerRoomID = dungeonManager.playerRoomID;
        }
    }

    private void LockPreviousRoomDoors(int roomIDToLock)
    {
        // DungeonManager에서 해당 방의 문 목록을 가져옵니다.
        if (dungeonManager.doorDic.ContainsKey(roomIDToLock))
        {
            List<Door> doorsInRoom = dungeonManager.doorDic[roomIDToLock];
            foreach (Door door in doorsInRoom)
            {
                // 각 문에 LockDoor() 메서드를 호출하여 잠급니다.
                // Door 스크립트에 LockDoor() 메서드가 이미 있다고 가정합니다.
                Debug.Log($"지나온 방으로는 돌아갈 수 없는 것 같아.");
            }
        }
        else
        {
            Debug.LogWarning($"방 ID {roomIDToLock}에 해당하는 문 목록을 찾을 수 없습니다.");
        }
    }
}