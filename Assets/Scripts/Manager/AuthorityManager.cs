
using UnityEngine;

/// <summary>
/// 파라오의 권위(Authority)를 관리하는 싱글톤 매니저.
/// 권위 게이지는 상한 없이 증가하며, 증가량은 로그 함수처럼 점차 감소합니다.
/// </summary>
public class AuthorityManager : MonoBehaviour
{
    #region Singleton
    public static AuthorityManager instance { get; private set; }
    #endregion

    [Header("Authority Settings")]
    [Tooltip("현재 권위 게이지. 상한 없이 계속 증가할 수 있습니다.")]
    public float authorityGauge = 0f;

    [Tooltip("권위가 감소하기 시작하는 비활성 시간(초)입니다.")]
    public float decayDelay = 3f;

    [Tooltip("초당 감소하는 권위의 양입니다.")]
    public float decayRate = 5f;

    [Header("Balancing Settings")]
    [Tooltip("게이지가 0일 때의 기본 권위 증가량입니다.")]
    public float baseIncreaseAmount = 10f;

    [Tooltip("증가량 감소에 영향을 미치는 스케일링 팩터입니다. 값이 클수록 증가량이 더 빠르게 줄어듭니다.")]
    public float scalingFactor = 0.1f;

    // 마지막으로 권위가 증가한 시간을 추적합니다.
    private float timeSinceLastIncrease = 0f;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    private void Update()
    {
        // 마지막 증가 후 경과 시간을 계산합니다.
        timeSinceLastIncrease += Time.deltaTime;

        // 설정된 지연 시간보다 오래 증가하지 않았고, 게이지가 0보다 크면 감소를 시작합니다.
        if (timeSinceLastIncrease > decayDelay && authorityGauge > 0)
        {
            // 권위가 1보다 클 경우, 감소 속도를 권위 값에 반비례하여 조절합니다.
            // 이로 인해 권위가 높을수록 감소 속도가 느려집니다.
            float currentDecayRate = (authorityGauge > 1f) ? (decayRate / authorityGauge) : decayRate;
            
            authorityGauge -= currentDecayRate * Time.deltaTime;
            authorityGauge = Mathf.Max(authorityGauge, 0f); // 게이지가 0 밑으로 내려가지 않도록 합니다.
        }
    }

    /// <summary>
    /// 권위 게이지를 증가시킵니다. 증가량은 현재 게이지 값에 따라 동적으로 계산됩니다.
    /// </summary>
    public void IncreaseAuthority()
    {
        // 현재 게이지 값에 따라 증가량을 계산합니다. (로그 함수와 유사한 형태)
        // 분모에 +1을 하여 authorityGauge가 0일 때도 정상적으로 작동하도록 합니다.
        float amountToIncrease = baseIncreaseAmount / (authorityGauge * scalingFactor + 1);

        // 권위 게이지를 증가시킵니다.
        authorityGauge += amountToIncrease;

        // 마지막 증가 시간을 초기화하여 감소를 막습니다.
        timeSinceLastIncrease = 0f;

        Debug.Log($"Authority Increased by {amountToIncrease:F2}! Current Gauge: {authorityGauge:F2}");
    }
}
