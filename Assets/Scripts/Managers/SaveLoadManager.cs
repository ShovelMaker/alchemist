using System;
using UnityEngine;
using System.IO;
[Serializable]
public class SaveData
{
    public Vector2 playerPosition;
    public int playerLevel;
    public string[] completedQuests;
}
public class SaveLoadManager : MonoBehaviour
{
    // 저장 슬롯 개수 지정
    private const int MaxSaveSlots = 2;
    private string GetSavePath(int slot)
    {
        slot = Mathf.Clamp(slot, 1, MaxSaveSlots);
        return Path.Combine(Application.persistentDataPath,
                        $"save_slot{slot}.json");

    }

    public void SaveGame(int slot)
    {
        // 플레이어 위치 가져오기
        Vector3 pos3D;

        // 저장 경로 가져오기
        string path = GetSavePath(slot);


    }
}