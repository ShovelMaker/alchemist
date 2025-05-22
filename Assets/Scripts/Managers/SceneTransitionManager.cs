// Scripts/Managers/SceneTransitionManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // �� ������ ���� �ʼ�!
using System.Collections; // �ڷ�ƾ�� ����ϱ� ���� �ʿ� (���̵� ȿ�� ��)
using UnityEngine.UI; // ���̵� ȿ���� UI Image�� �ٷ�� ���� �ʿ� (���� ����)

public class SceneTransitionManager : MonoBehaviour
{
    // --- �̱��� ���� ---
    private static SceneTransitionManager _instance;
    public static SceneTransitionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SceneTransitionManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SceneTransitionManager_RuntimeInstance");
                    _instance = singletonObject.AddComponent<SceneTransitionManager>();
                }
            }
            return _instance;
        }
    }
    // --- �̱��� ���� �� ---

    public Image fadeImage; // ȭ�� ��ü�� ���� ������ UI Image (Inspector���� �Ҵ�)
    public float fadeDuration = 1f; // ���̵� ȿ�� ���� �ð� (��)
    private bool isTransitioning = false; // ���� �� ��ȯ ������ ����

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        // ���̵� �̹����� �Ҵ���� �ʾҴٸ�, �������� �����ϰų� ��� (���⼭�� ���)
        if (fadeImage == null)
        {
            Debug.LogWarning("SceneTransitionManager: fadeImage�� �Ҵ���� �ʾҽ��ϴ�. ���̵� ȿ���� �۵����� ���� �� �ֽ��ϴ�.");
        }
        else
        {
            fadeImage.gameObject.SetActive(false); // ó���� ���̵� �̹��� ��Ȱ��ȭ
            fadeImage.rectTransform.anchorMin = Vector2.zero; // ȭ�� ��ü�� ������ ����
            fadeImage.rectTransform.anchorMax = Vector2.one;
            fadeImage.rectTransform.sizeDelta = Vector2.zero;
            fadeImage.color = new Color(0, 0, 0, 0); // ó���� �����ϰ�
        }
    }

    // Ư�� ������ ��ȯ�ϴ� ���� �Լ� (���̵� ȿ�� ����)
    public void LoadSceneWithFade(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionRoutine(sceneName));
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager: �̹� �ٸ� ������ ��ȯ ���Դϴ�.");
        }
    }

    // �� ��ȯ �ڷ�ƾ
    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        // 1. ���̵� �ƿ�
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeEffect(1f)); // ���İ��� 1�� (�������ϰ�)
        }
        else
        {
            Debug.Log("SceneTransitionManager: ���̵� �ƿ� �ǳʶ� (fadeImage ����)");
        }


        // (���� ����) ���⿡ �ε� ȭ�� ǥ�� ���� �߰� ����
        // UIManager.Instance.ShowLoadingScreen();

        // 2. �� �񵿱� �ε� (���� �� �ε�)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // allowSceneActivation = false�� �����ϸ� �ε尡 �Ϸ�Ǿ �ٷ� Ȱ��ȭ���� ����
        // asyncLoad.allowSceneActivation = false; 

        while (!asyncLoad.isDone)
        {
            // ���⼭ �ε� ������� UIManager�� ���� ǥ���� �� ����
            // float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            // UIManager.Instance.UpdateLoadingProgress(progress);
            // Debug.Log("Loading progress: " + (progress * 100) + "%");

            // if (asyncLoad.progress >= 0.9f && !asyncLoad.allowSceneActivation)
            // {
            //    // �ε��� ���� �Ϸ�Ǿ���, ���� Ȱ��ȭ�� ������ ��
            //    // (��: "����Ϸ��� �ƹ� Ű�� ��������" ���� �޽��� ��)
            //    // asyncLoad.allowSceneActivation = true;
            // }
            yield return null; // ���� �����ӱ��� ���
        }

        // (���� ����) �ε� ȭ�� ����� ����
        // UIManager.Instance.HideLoadingScreen();

        // 3. ���̵� ��
        if (fadeImage != null)
        {
            // �� ���� �ε�� �� ��� ��� (���� ����, ȭ���� �ʹ� ���� �ٲ�� ���� ����)
            // yield return new WaitForSeconds(0.1f); 
            yield return StartCoroutine(FadeEffect(0f)); // ���İ��� 0���� (�����ϰ�)
            fadeImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("SceneTransitionManager: ���̵� �� �ǳʶ� (fadeImage ����)");
        }


        isTransitioning = false;
    }

    // ���̵� ȿ���� �ִ� �ڷ�ƾ
    private IEnumerator FadeEffect(float targetAlpha)
    {
        if (fadeImage == null) yield break; // fadeImage ������ �ߴ�

        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }
        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha); // ��Ȯ�� ��ǥ ���İ����� ����
    }
}