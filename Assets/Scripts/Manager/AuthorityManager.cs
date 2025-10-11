using UnityEngine;
using System.Collections;

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
    public float decayDelay = 5f;

    // [Original: 5f] 감소 속도를 낮춰서 권위가 천천히 줄어들도록 수정.
    [Tooltip("초당 감소하는 권위의 양입니다.")]
    public float decayRate = 3f;

    [Header("Balancing Settings")]
    [Tooltip("게이지가 0일 때의 기본 권위 증가량입니다.")]
    public float baseIncreaseAmount = 10f;

    // [Original: 0.1f] 스케일링 팩터를 줄여서 권위 증가량의 감소폭을 완만하게 수정.
    [Tooltip("증가량 감소에 영향을 미치는 스케일링 팩터입니다. 값이 클수록 증가량이 더 빠르게 줄어듭니다.")]
    public float scalingFactor = 0.05f;

    // 마지막으로 권위가 증가한 시간을 추적합니다.
    private float timeSinceLastIncrease = 0f;
    
    // 게이지 최대치 및 초기화 로직을 위한 변수
    private const float MaxAuthorityGauge = 500f;
    private bool _isGaugeFrozen = false;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
    }

    private void Update()
    {
        // 게이지가 정지 상태일 때는 감소 로직을 실행하지 않습니다.
        if (_isGaugeFrozen) return;

        // 마지막 증가 후 경과 시간을 계산합니다.
        timeSinceLastIncrease += Time.deltaTime;

        // 설정된 지연 시간보다 오래 증가하지 않았고, 게이지가 0보다 크면 감소를 시작합니다.
        if (timeSinceLastIncrease > decayDelay && authorityGauge > 0)
        {
            authorityGauge -= decayRate * Time.deltaTime;
            authorityGauge = Mathf.Max(authorityGauge, 0f); // 게이지가 0 밑으로 내려가지 않도록 합니다.
        }
    }

    /// <summary>
    /// 권위 게이지를 증가시킵니다. 증가량은 현재 게이지 값에 따라 동적으로 계산됩니다.
    /// </summary>
    public void IncreaseAuthority()
    {
        // 게이지가 정지 상태일 때는 증가 로직을 실행하지 않습니다.
        if (_isGaugeFrozen) return;

        // 현재 게이지 값에 따라 증가량을 계산합니다. (로그 함수와 유사한 형태)
        // 분모에 +1을 하여 authorityGauge가 0일 때도 정상적으로 작동하도록 합니다.
        float amountToIncrease = baseIncreaseAmount / (authorityGauge * scalingFactor + 1);

        // 권위 게이지를 증가시킵니다.
        authorityGauge += amountToIncrease;
        
        // 게이지가 상한선을 넘지 않도록 합니다.
        authorityGauge = Mathf.Min(authorityGauge, MaxAuthorityGauge);

        // 마지막 증가 시간을 초기화하여 감소를 막습니다.
        timeSinceLastIncrease = 0f;

        Debug.Log($"Authority Increased by {amountToIncrease:F2}! Current Gauge: {authorityGauge:F2}");

        // 게이지가 최대치에 도달하면 초기화 코루틴을 시작합니다.
        if (authorityGauge >= MaxAuthorityGauge)
        {
            StartCoroutine(ResetGaugeAfterDelay());
        }
    }

    /// <summary>
    /// 게이지가 최대치에 도달했을 때, 5초 대기 후 0으로 초기화하는 코루틴입니다.
    /// </summary>
    private IEnumerator ResetGaugeAfterDelay()
    {
        _isGaugeFrozen = true;
        Debug.Log($"권위가 최대치({MaxAuthorityGauge})에 도달했습니다! 5초 후 초기화됩니다.");
        
        yield return new WaitForSeconds(5f);
        
        authorityGauge = 0f;
        timeSinceLastIncrease = 0f; // 초기화 후 바로 감소하는 것을 방지합니다.
        _isGaugeFrozen = false;
        
        Debug.Log("권위가 0으로 초기화되었습니다.");
    }
}
