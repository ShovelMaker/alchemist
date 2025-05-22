// Scripts/Managers/UIManager.cs
using UnityEngine;
using UnityEngine.UI; // Unity UI 시스템 (Text, Button 등)을 사용하기 위해 필요

public class UIManager : MonoBehaviour
{
    // --- 싱글톤 구현 ---
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("UIManager_RuntimeInstance");
                    _instance = singletonObject.AddComponent<UIManager>();
                    Debug.Log("UIManager_RuntimeInstance가 생성되었습니다.");
                }
            }
            return _instance;
        }
    }
    // --- 싱글톤 구현 끝 ---

    // --- 공통 UI 요소 참조 (Inspector에서 할당) ---
    [Header("플레이어 정보 HUD")]
    public Text goldText;
    // public Text playerLevelText; // 예시: 플레이어 레벨 표시
    // public Slider playerHealthSlider; // 예시: 플레이어 체력 바

    [Header("UI 패널들")]
    public GameObject saveLoadPanel;   // 저장/불러오기 UI 패널
    public GameObject settingsPanel;   // 설정 UI 패널
    // public GameObject inventoryPanel;  // 인벤토리 UI 패널
    // public GameObject dialoguePanel;   // 대화창 UI 패널
    // ... 기타 필요한 패널들

    void Awake()
    {
        // 싱글톤 인스턴스 관리
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("UIManager 인스턴스가 이미 존재하여 새로 생성된 것을 파괴합니다.");
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject); // UI 매니저는 씬 전환 시에도 유지되는 경우가 많음

        InitializeUI();
    }

    // UI 초기 상태 설정
    void InitializeUI()
    {
        // 게임 시작 시 모든 주요 패널들은 숨겨진 상태로 시작 (필요에 따라)
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        // if (inventoryPanel != null) inventoryPanel.SetActive(false);
        // if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // 초기 골드 표시 (SaveLoadManager에서 로드 후 다시 업데이트될 수 있음)
        // UpdateGoldDisplay(0); // 예시: 초기값 0으로 설정
    }

    // --- 골드 UI 업데이트 함수 ---
    public void UpdateGoldDisplay(int amount)
    {
        if (goldText != null)
        {
            goldText.text = "골드: " + amount.ToString(); // 또는 "Gold: " + amount, "재화: " + amount 등 원하는 텍스트로
        }
        else
        {
            // goldText가 할당되지 않았을 경우, 게임 진행에 치명적이지 않다면 경고만,
            // 필수적이라면 오류를 발생시켜 빠르게 인지하도록 할 수도 있네.
            Debug.LogWarning("goldText 참조가 UIManager에 할당되지 않았습니다. Inspector에서 연결해주세요.");
        }
    }

    // --- 저장/불러오기 패널 토글 함수 ---
    // 이 함수는 게임 내 특정 버튼(예: ESC 메뉴의 '저장/불러오기' 버튼)에 연결될 수 있네.
    public void ToggleSaveLoadPanel()
    {
        if (saveLoadPanel != null)
        {
            bool isActive = !saveLoadPanel.activeSelf;
            saveLoadPanel.SetActive(isActive);
            Debug.Log("Save/Load Panel 활성 상태: " + isActive);

            // 다른 패널이 열려있다면 닫아주는 로직도 추가할 수 있네 (선택 사항)
            // if (isActive && settingsPanel != null && settingsPanel.activeSelf)
            // {
            //     settingsPanel.SetActive(false);
            // }
        }
        else
        {
            Debug.LogWarning("saveLoadPanel 참조가 UIManager에 할당되지 않았습니다. Inspector에서 연결해주세요.");
        }
    }

    // --- 설정 패널 토글 함수 ---
    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            bool isActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(isActive);
            Debug.Log("Settings Panel 활성 상태: " + isActive);

            // if (isActive && saveLoadPanel != null && saveLoadPanel.activeSelf)
            // {
            //     saveLoadPanel.SetActive(false);
            // }
        }
        else
        {
            Debug.LogWarning("settingsPanel 참조가 UIManager에 할당되지 않았습니다. Inspector에서 연결해주세요.");
        }
    }

    // 여기에 더 많은 UI 관리 함수들이 추가될 걸세. 예를 들면:
    // public void ShowInventory() { if(inventoryPanel != null) inventoryPanel.SetActive(true); }
    // public void HideInventory() { if(inventoryPanel != null) inventoryPanel.SetActive(false); }
    // public void ShowNotification(string message, float duration) { /* ... */ }
    // public void UpdatePlayerHealth(int currentHealth, int maxHealth) { /* ... */ }
}