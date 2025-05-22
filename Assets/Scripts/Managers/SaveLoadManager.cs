// Scripts/Managers/SaveLoadManager.cs
using UnityEngine;
using System.IO; // 파일을 다루기 위한 네임스페이스
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가 (예시용)

public class SaveLoadManager : MonoBehaviour
{
    // --- 싱글톤 구현 ---
    private static SaveLoadManager _instance;
    public static SaveLoadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveLoadManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SaveLoadManager_RuntimeInstance");
                    _instance = singletonObject.AddComponent<SaveLoadManager>();
                    // Debug.Log("SaveLoadManager_RuntimeInstance가 생성되었습니다."); // 필요시 주석 해제
                }
            }
            return _instance;
        }
    }
    // --- 싱글톤 구현 끝 ---

    // 중요: 현재 게임 클라이언트가 사용하는 최신 SaveData 버전
    public const int LATEST_SAVE_VERSION = 1; // SaveData 구조 변경 시 이 버전을 올림

    private string saveFilePrefix = "AlchemistJourney_Slot_";
    private string saveFileExtension = ".json";

    public SaveData currentSaveData; // 현재 게임의 세이브 데이터 객체

    void Awake()
    {
        // 싱글톤 인스턴스 관리
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("SaveLoadManager 인스턴스가 이미 존재하여 새로 생성된 인스턴스를 파괴합니다.");
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // 슬롯별 저장 경로 생성 함수
    private string GetSavePath(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, saveFilePrefix + slotNumber + saveFileExtension);
    }

    // 게임 저장 함수
    public void SaveGame(int slotNumber)
    {
        if (currentSaveData == null)
        {
            Debug.LogError("currentSaveData가 null입니다. 저장할 데이터가 없습니다! 새 게임을 시작하거나 유효한 데이터를 로드한 후 시도하세요.");
            return;
        }

        // 저장 전, SaveData의 버전을 최신으로 설정
        currentSaveData.saveVersion = LATEST_SAVE_VERSION;

        // 중요: 이 시점에 게임 내 다른 매니저들로부터 최신 데이터를 currentSaveData에 반영해야 함.
        // 예시:
        // if (GameManager.Instance != null && GameManager.Instance.Player != null)
        // {
        //    currentSaveData.playerPosition = GameManager.Instance.Player.transform.position;
        //    currentSaveData.currentScene = SceneManager.GetActiveScene().name;
        // }
        // if (InventoryManager.Instance != null)
        // {
        //    currentSaveData.gold = InventoryManager.Instance.Gold;
        //    // currentSaveData.items = InventoryManager.Instance.GetAllItems(); // 등등
        // }


        string savePath = GetSavePath(slotNumber);
        Debug.Log($"게임을 슬롯 {slotNumber}에 저장합니다 (버전: {currentSaveData.saveVersion}). 경로: {savePath}");

        try
        {
            string json = JsonUtility.ToJson(currentSaveData, true); // true: pretty print
            File.WriteAllText(savePath, json);
            Debug.Log($"슬롯 {slotNumber}에 저장 완료!");
            // (선택) 저장 완료 후 UI 피드백
            // if(UIManager.Instance != null) UIManager.Instance.ShowNotification("저장 완료!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"슬롯 {slotNumber} 저장 실패: {e.Message}\n{e.StackTrace}");
            // (선택) 저장 실패 UI 피드백
            // if(UIManager.Instance != null) UIManager.Instance.ShowNotification("저장 실패!");
        }
    }

    // 게임 로드 함수
    public bool LoadGame(int slotNumber)
    {
        string loadPath = GetSavePath(slotNumber);
        Debug.Log($"슬롯 {slotNumber}에서 게임을 로드합니다. 경로: {loadPath}");

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                SaveData loadedData = JsonUtility.FromJson<SaveData>(json);

                // --- 버전 체크 및 마이그레이션 ---
                if (loadedData.saveVersion < LATEST_SAVE_VERSION)
                {
                    Debug.LogWarning($"이전 버전(v{loadedData.saveVersion}) 세이브 파일 발견. 최신 버전(v{LATEST_SAVE_VERSION})으로 마이그레이션 시도.");
                    loadedData = MigrateData(loadedData);
                    if (loadedData.saveVersion != LATEST_SAVE_VERSION)
                    {
                        Debug.LogError($"마이그레이션 후에도 세이브 버전(v{loadedData.saveVersion})이 최신 버전(v{LATEST_SAVE_VERSION})과 불일치! 로드 중단.");
                        return false; // 마이그레이션 실패 간주
                    }
                    Debug.Log("데이터 마이그레이션 성공. 계속 진행합니다.");
                }
                else if (loadedData.saveVersion > LATEST_SAVE_VERSION)
                {
                    Debug.LogError($"세이브 파일(v{loadedData.saveVersion})이 현재 게임(v{LATEST_SAVE_VERSION})보다 최신 버전입니다. 로드 불가!");
                    return false;
                }
                // --- 마이그레이션 끝 ---

                currentSaveData = loadedData;
                Debug.Log($"슬롯 {slotNumber} 로드 완료! (적용된 데이터 버전: v{currentSaveData.saveVersion})");

                // 로드된 데이터를 게임 시스템 및 UI에 적용
                ApplyLoadedDataToGameSystems();
                UpdateUIAfterLoad();

                // (선택) 로드 후 특정 씬으로 이동 등
                // if (!string.IsNullOrEmpty(currentSaveData.currentScene))
                // {
                //    SceneManager.LoadScene(currentSaveData.currentScene);
                // } else {
                //    SceneManager.LoadScene("DefaultGameScene"); // 기본 게임 씬
                // }

                return true; // 로드 성공
            }
            catch (System.Exception e)
            {
                Debug.LogError($"슬롯 {slotNumber} 로드 실패: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning($"슬롯 {slotNumber}에 저장된 파일이 없습니다.");
            return false;
        }
    }

    // 데이터 마이그레이션 함수
    private SaveData MigrateData(SaveData oldData)
    {
        SaveData migratedData = oldData; // 시작은 이전 데이터 복사본 (실제로는 깊은 복사가 필요할 수 있음)

        // 예시: v1 -> v2 마이그레이션
        if (migratedData.saveVersion == 1 && LATEST_SAVE_VERSION >= 2)
        {
            // 여기에 v1 SaveData를 v2 SaveData 구조로 변환하는 로직 작성
            // 예: SaveData_V2 tempV2Data = new SaveData_V2();
            // tempV2Data.saveVersion = 2;
            // tempV2Data.newField = oldData.someOldFieldConverted; // 필드 값 이전 및 변환
            // ...
            // migratedData = tempV2Data; // 실제로는 타입이 다를 수 있으므로 주의
            migratedData.saveVersion = 2; // 변환된 데이터의 버전을 명시적으로 업데이트
            Debug.Log("세이브 데이터: v1 -> v2 마이그레이션 완료.");
        }

        // 예시: v2 -> v3 마이그레이션
        if (migratedData.saveVersion == 2 && LATEST_SAVE_VERSION >= 3)
        {
            // v2 SaveData를 v3 SaveData 구조로 변환
            migratedData.saveVersion = 3;
            Debug.Log("세이브 데이터: v2 -> v3 마이그레이션 완료.");
        }
        // ... 추가적인 버전 마이그레이션 로직 (단계별로)

        // 최종적으로 마이그레이션된 데이터의 버전이 LATEST_SAVE_VERSION과 일치해야 함
        if (migratedData.saveVersion != LATEST_SAVE_VERSION)
        {
            Debug.LogError($"마이그레이션 로직 오류: 최종 버전(v{migratedData.saveVersion})이 LATEST_SAVE_VERSION(v{LATEST_SAVE_VERSION})과 다릅니다!");
            // 여기서 오류를 발생시키거나, 로드를 중단하는 등의 처리가 필요할 수 있음
        }
        return migratedData;
    }

    // 새 게임 시작 함수
    public void StartNewGame(int slotNumber)
    {
        currentSaveData = new SaveData(LATEST_SAVE_VERSION); // 최신 버전으로 새 SaveData 객체 생성
        Debug.Log($"슬롯 {slotNumber}에 새 게임 시작. 기본 데이터로 초기화 (버전: {currentSaveData.saveVersion})");

        // 새 게임 데이터를 게임 시스템 및 UI에 적용
        ApplyLoadedDataToGameSystems(); // 새 게임 데이터도 '로드된' 데이터처럼 시스템에 적용
        UpdateUIAfterLoad(); // UI도 새 게임 상태로 업데이트

        // (선택) 새 게임 데이터를 바로 해당 슬롯에 저장 (플레이어가 명시적으로 저장하기 전까지는 저장 안 할 수도 있음)
        // SaveGame(slotNumber);

        // (선택) 초기 씬으로 이동
        // if (!string.IsNullOrEmpty(currentSaveData.currentScene))
        // {
        //    SceneManager.LoadScene(currentSaveData.currentScene);
        // } else {
        //    SceneManager.LoadScene("DefaultGameScene");
        // }
    }

    // 로드된 (또는 새 게임) 데이터를 실제 게임 시스템에 적용하는 함수
    private void ApplyLoadedDataToGameSystems()
    {
        if (currentSaveData == null) return;

        // 예시:
        // if (GameManager.Instance != null && GameManager.Instance.Player != null)
        // {
        //    GameManager.Instance.Player.transform.position = currentSaveData.playerPosition;
        //    // GameManager.Instance.Player.Level = currentSaveData.playerLevel;
        // }
        // if (InventoryManager.Instance != null)
        // {
        //    InventoryManager.Instance.Gold = currentSaveData.gold;
        //    // InventoryManager.Instance.LoadItems(currentSaveData.items);
        // }
        // QuestManager.Instance.LoadQuests(currentSaveData.questProgresses);
        // ... 기타 필요한 모든 시스템에 데이터 적용
        Debug.Log("세이브 데이터가 게임 시스템에 적용되었습니다.");
    }

    // 데이터 로드 또는 새 게임 후 UI 업데이트를 담당하는 함수
    private void UpdateUIAfterLoad()
    {
        if (currentSaveData == null) return;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateGoldDisplay(currentSaveData.gold);
            // UIManager.Instance.UpdatePlayerLevel(currentSaveData.playerLevel);
            // UIManager.Instance.UpdateActiveQuests(currentSaveData.questProgresses);
            // ... 기타 UI 업데이트
            Debug.Log("UI가 로드된 데이터로 업데이트되었습니다.");
        }
    }


    // 특정 슬롯의 저장 파일 존재 여부 확인
    public bool DoesSaveExist(int slotNumber)
    {
        return File.Exists(GetSavePath(slotNumber));
    }

    // (참고) SaveData 클래스 정의는 별도의 'SaveData.cs' 파일에 있어야 함.
    // [System.Serializable]
    // public class SaveData
    // {
    //     public int saveVersion;
    //     public Vector3 playerPosition;
    //     public int playerLevel;
    //     public string currentScene;
    //     public int currentMainQuestID;
    //     public int gold;
    //
    //     public SaveData(int currentAppSaveVersion)
    //     {
    //         saveVersion = currentAppSaveVersion;
    //         // ... 초기값 설정 ...
    //     }
    //     public SaveData() { } // JsonUtility용 기본 생성자
    // }
}