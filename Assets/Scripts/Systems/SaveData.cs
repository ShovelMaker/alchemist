using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� ��Ʈ����Ʈ�� �־�� JsonUtility�� ����ȭ ����
[System.Serializable]
public class SaveData
{
    public int saveVersion; // �������� ����

    // --- �⺻ �÷��̾� ���� ---
    // �÷��̾� ��ġ [Vector3�� �⺻������ ����ȭ ����]
    public Vector3 playerPosition;
    // �÷��̾� ����
    public int playerLevel;
    // �÷��̾ ���������� �־��� �� �̸�
    public string currentScene;

    // --- ����Ʈ ���� ��Ȳ ---
    public int currentMainQuestID;

    // --- ��ȭ ---
    public int gold;
    // �̰��� ������ ���� �߰��� ����


    // ������ : �� ���ӽ� �ʱⰪ ����
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

