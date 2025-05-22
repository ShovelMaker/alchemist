// Scripts/Managers/SceneTransitionManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수!
using System.Collections; // 코루틴을 사용하기 위해 필요 (페이드 효과 등)
using UnityEngine.UI; // 페이드 효과용 UI Image를 다루기 위해 필요 (선택 사항)

public class SceneTransitionManager : MonoBehaviour
{
    // --- 싱글톤 구현 ---
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
    // --- 싱글톤 구현 끝 ---

    public Image fadeImage; // 화면 전체를 덮는 검은색 UI Image (Inspector에서 할당)
    public float fadeDuration = 1f; // 페이드 효과 지속 시간 (초)
    private bool isTransitioning = false; // 현재 씬 전환 중인지 여부

    void Awake()
    {
        // 싱글톤 인스턴스 관리
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        // 페이드 이미지가 할당되지 않았다면, 동적으로 생성하거나 경고 (여기서는 경고만)
        if (fadeImage == null)
        {
            Debug.LogWarning("SceneTransitionManager: fadeImage가 할당되지 않았습니다. 페이드 효과가 작동하지 않을 수 있습니다.");
        }
        else
        {
            fadeImage.gameObject.SetActive(false); // 처음엔 페이드 이미지 비활성화
            fadeImage.rectTransform.anchorMin = Vector2.zero; // 화면 전체를 덮도록 설정
            fadeImage.rectTransform.anchorMax = Vector2.one;
            fadeImage.rectTransform.sizeDelta = Vector2.zero;
            fadeImage.color = new Color(0, 0, 0, 0); // 처음엔 투명하게
        }
    }

    // 특정 씬으로 전환하는 메인 함수 (페이드 효과 포함)
    public void LoadSceneWithFade(string sceneName)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionRoutine(sceneName));
        }
        else
        {
            Debug.LogWarning("SceneTransitionManager: 이미 다른 씬으로 전환 중입니다.");
        }
    }

    // 씬 전환 코루틴
    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        // 1. 페이드 아웃
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeEffect(1f)); // 알파값을 1로 (불투명하게)
        }
        else
        {
            Debug.Log("SceneTransitionManager: 페이드 아웃 건너뜀 (fadeImage 없음)");
        }


        // (선택 사항) 여기에 로딩 화면 표시 로직 추가 가능
        // UIManager.Instance.ShowLoadingScreen();

        // 2. 씬 비동기 로드 (실제 씬 로딩)
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        // allowSceneActivation = false로 설정하면 로드가 완료되어도 바로 활성화되지 않음
        // asyncLoad.allowSceneActivation = false; 

        while (!asyncLoad.isDone)
        {
            // 여기서 로딩 진행률을 UIManager를 통해 표시할 수 있음
            // float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            // UIManager.Instance.UpdateLoadingProgress(progress);
            // Debug.Log("Loading progress: " + (progress * 100) + "%");

            // if (asyncLoad.progress >= 0.9f && !asyncLoad.allowSceneActivation)
            // {
            //    // 로딩이 거의 완료되었고, 이제 활성화만 남았을 때
            //    // (예: "계속하려면 아무 키나 누르세요" 같은 메시지 후)
            //    // asyncLoad.allowSceneActivation = true;
            // }
            yield return null; // 다음 프레임까지 대기
        }

        // (선택 사항) 로딩 화면 숨기기 로직
        // UIManager.Instance.HideLoadingScreen();

        // 3. 페이드 인
        if (fadeImage != null)
        {
            // 새 씬이 로드된 후 잠시 대기 (선택 사항, 화면이 너무 빨리 바뀌는 것을 방지)
            // yield return new WaitForSeconds(0.1f); 
            yield return StartCoroutine(FadeEffect(0f)); // 알파값을 0으로 (투명하게)
            fadeImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("SceneTransitionManager: 페이드 인 건너뜀 (fadeImage 없음)");
        }


        isTransitioning = false;
    }

    // 페이드 효과를 주는 코루틴
    private IEnumerator FadeEffect(float targetAlpha)
    {
        if (fadeImage == null) yield break; // fadeImage 없으면 중단

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
        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha); // 정확히 목표 알파값으로 설정
    }
}