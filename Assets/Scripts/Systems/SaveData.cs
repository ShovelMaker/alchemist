using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이 어트리뷰트가 있어야 JsonUtility로 직렬화 가능
[System.Serializable]
public class SaveData
{
    public int saveVersion; // 저장파일 버전

    // --- 기본 플레이어 정보 ---
    // 플레이어 위치 [Vector3는 기본적으로 직렬화 가능]
    public Vector3 playerPosition;
    // 플레이어 레벨
    public int playerLevel;
    // 플레이어가 마지막으로 있었던 씬 이름
    public string currentScene;

    // --- 퀘스트 진행 상황 ---
    public int currentMainQuestID;

    // --- 재화 ---
    public int gold;
    // 이곳에 데이터 구조 추가될 예정


    // 생성자 : 새 게임시 초기값 설정
    public SaveData(int currentAppSaveVersion)
    {
        saveVersion = currentAppSaveVersion;
        this.playerLevel = 1;
        currentMainQuestID = 1001;
        gold = 100;
        currentScene = "Tutorial_Forest";
        playerPosition = Vector3.zero;
    }

    public SaveData() { }
}

