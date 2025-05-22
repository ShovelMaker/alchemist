// Scripts/Managers/UIManager.cs
using UnityEngine;
using UnityEngine.UI; // Unity UI �ý��� (Text, Button ��)�� ����ϱ� ���� �ʿ�

public class UIManager : MonoBehaviour
{
    // --- �̱��� ���� ---
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
                    Debug.Log("UIManager_RuntimeInstance�� �����Ǿ����ϴ�.");
                }
            }
            return _instance;
        }
    }
    // --- �̱��� ���� �� ---

    // --- ���� UI ��� ���� (Inspector���� �Ҵ�) ---
    [Header("�÷��̾� ���� HUD")]
    public Text goldText;
    // public Text playerLevelText; // ����: �÷��̾� ���� ǥ��
    // public Slider playerHealthSlider; // ����: �÷��̾� ü�� ��

    [Header("UI �гε�")]
    public GameObject saveLoadPanel;   // ����/�ҷ����� UI �г�
    public GameObject settingsPanel;   // ���� UI �г�
    // public GameObject inventoryPanel;  // �κ��丮 UI �г�
    // public GameObject dialoguePanel;   // ��ȭâ UI �г�
    // ... ��Ÿ �ʿ��� �гε�

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("UIManager �ν��Ͻ��� �̹� �����Ͽ� ���� ������ ���� �ı��մϴ�.");
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject); // UI �Ŵ����� �� ��ȯ �ÿ��� �����Ǵ� ��찡 ����

        InitializeUI();
    }

    // UI �ʱ� ���� ����
    void InitializeUI()
    {
        // ���� ���� �� ��� �ֿ� �гε��� ������ ���·� ���� (�ʿ信 ����)
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        // if (inventoryPanel != null) inventoryPanel.SetActive(false);
        // if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // �ʱ� ��� ǥ�� (SaveLoadManager���� �ε� �� �ٽ� ������Ʈ�� �� ����)
        // UpdateGoldDisplay(0); // ����: �ʱⰪ 0���� ����
    }

    // --- ��� UI ������Ʈ �Լ� ---
    public void UpdateGoldDisplay(int amount)
    {
        if (goldText != null)
        {
            goldText.text = "���: " + amount.ToString(); // �Ǵ� "Gold: " + amount, "��ȭ: " + amount �� ���ϴ� �ؽ�Ʈ��
        }
        else
        {
            // goldText�� �Ҵ���� �ʾ��� ���, ���� ���࿡ ġ�������� �ʴٸ� ���,
            // �ʼ����̶�� ������ �߻����� ������ �����ϵ��� �� ���� �ֳ�.
            Debug.LogWarning("goldText ������ UIManager�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �������ּ���.");
        }
    }

    // --- ����/�ҷ����� �г� ��� �Լ� ---
    // �� �Լ��� ���� �� Ư�� ��ư(��: ESC �޴��� '����/�ҷ�����' ��ư)�� ����� �� �ֳ�.
    public void ToggleSaveLoadPanel()
    {
        if (saveLoadPanel != null)
        {
            bool isActive = !saveLoadPanel.activeSelf;
            saveLoadPanel.SetActive(isActive);
            Debug.Log("Save/Load Panel Ȱ�� ����: " + isActive);

            // �ٸ� �г��� �����ִٸ� �ݾ��ִ� ������ �߰��� �� �ֳ� (���� ����)
            // if (isActive && settingsPanel != null && settingsPanel.activeSelf)
            // {
            //     settingsPanel.SetActive(false);
            // }
        }
        else
        {
            Debug.LogWarning("saveLoadPanel ������ UIManager�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �������ּ���.");
        }
    }

    // --- ���� �г� ��� �Լ� ---
    public void ToggleSettingsPanel()
    {
        if (settingsPanel != null)
        {
            bool isActive = !settingsPanel.activeSelf;
            settingsPanel.SetActive(isActive);
            Debug.Log("Settings Panel Ȱ�� ����: " + isActive);

            // if (isActive && saveLoadPanel != null && saveLoadPanel.activeSelf)
            // {
            //     saveLoadPanel.SetActive(false);
            // }
        }
        else
        {
            Debug.LogWarning("settingsPanel ������ UIManager�� �Ҵ���� �ʾҽ��ϴ�. Inspector���� �������ּ���.");
        }
    }

    // ���⿡ �� ���� UI ���� �Լ����� �߰��� �ɼ�. ���� ���:
    // public void ShowInventory() { if(inventoryPanel != null) inventoryPanel.SetActive(true); }
    // public void HideInventory() { if(inventoryPanel != null) inventoryPanel.SetActive(false); }
    // public void ShowNotification(string message, float duration) { /* ... */ }
    // public void UpdatePlayerHealth(int currentHealth, int maxHealth) { /* ... */ }
}