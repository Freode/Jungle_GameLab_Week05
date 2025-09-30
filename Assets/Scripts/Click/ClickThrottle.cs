using UnityEngine;

/// <summary>
/// 클릭 수락 간 최소 간격(minInterval)로 연타 보호.
/// - 수락된 클릭은 Debug.Log로 출력
/// - 거절된 클릭은 남은 대기시간(ms)와 함께 로그(옵션)
/// - 마우스 좌클릭(임시) 또는 UI Button.onClick에 TryClick() 연결
/// </summary>
public class ClickThrottle : MonoBehaviour
{
    [Header("Anti-Spam")]
    [Tooltip("두 클릭 사이의 최소 시간(초). 예: 0.06s ≈ 최대 16.6 CPS")]
    [Range(0.01f, 0.25f)] public float minInterval = 0.06f;

    [Header("Debug")]
    [Tooltip("거절된 클릭도 로그로 볼지 여부")]
    public bool logRejected = true;

    private float _lastClickTime = -9999f;
    private int _accepted;
    private int _rejected;

    // 선택: 1초 단위 CPS 간이 측정
    private float _cpsWindowStart;
    private int _cpsCount;

    public int tempCount = 0;
    public int mouseCount = 0;

    private void Awake()
    {
        _cpsWindowStart = Time.unscaledTime;
    }

    /// <summary>연타 보호를 적용한 클릭 시도</summary>
    public void TryClick()
    {
        float now = Time.unscaledTime;              // 타임스케일 영향 없음
        float dt = now - _lastClickTime;

        if (dt < minInterval)
        {
            _rejected++;
            if (logRejected)
            {
                float waitMs = (minInterval - dt) * 1000f;
                Debug.Log($"[Click] REJECTED (anti-spam). Wait ~{waitMs:F0} ms | Rejected={_rejected}");
            }
            return;
        }

        _lastClickTime = now;
        _accepted++;
        _cpsCount++;

        // 클릭 수락 로그
        Debug.Log($"[Click] Accepted #{_accepted}");
        tempCount++;

        // 1초 창으로 CPS 출력(선택)
        if (now - _cpsWindowStart >= 1f)
        {
            Debug.Log($"[Click] CPS={_cpsCount}, TotalAccepted={_accepted}, TotalRejected={_rejected}");
            _cpsWindowStart = now;
            _cpsCount = 0;
        }
    }

    // 임시: 마우스 좌클릭으로 테스트 (프로덕션에선 제거 가능)
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            mouseCount++;
    }
}
