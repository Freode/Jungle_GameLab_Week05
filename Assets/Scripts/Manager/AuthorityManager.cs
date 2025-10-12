using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 파라오의 권위(Authority)를 관리하는 싱글톤 매니저.
/// 권위 게이지는 상한 없이 증가하며, 증가량은 로그 함수처럼 점차 감소합니다.
/// </summary>
public class AuthorityManager : MonoBehaviour
{
    #region Singleton
    public static AuthorityManager instance { get; private set; }
    #endregion

    [Header("Fever Time Settings")]
    [Tooltip("피버 타임의 지속 시간(초)입니다.")]
    public float feverTimeDuration = 10f;
    [Tooltip("피버 타임 동안 적용될 최종 배율입니다.")]
    public float feverTimeMultiplier = 100f; // 예: 10배


    [Header("Authority Settings")]
    [Tooltip("현재 권위 게이지. 상한 없이 계속 증가할 수 있습니다.")]
    public float authorityGauge = 0f;

    // ★ 현재 피버 타임 상태인지 알려주는 변수
    private bool isFeverTime = false;
    // ★ 외부에서 현재 피버 타임인지 확인할 수 있는 창구
    public bool IsFeverTime => isFeverTime;

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
    
    [Tooltip("게이지 수치에 따라 동적으로 변하는 배율입니다.")]
    public float authorityMultiplier = 1f;
    
    [Header("방송할 채널")]
    public FloatEventChannelSO onAuthorityChangedChannel;

    [Header("Gage Slider")]
    public Slider authorityGaugeSlider;
    public TextMeshProUGUI authorityGaugeText;
    
    // 마지막으로 권위가 증가한 시간을 추적합니다.
    private float timeSinceLastIncrease = 0f;
    
    // 게이지 최대치 및 초기화 로직을 위한 변수
    private const float MaxAuthorityGauge = 500f;
    private bool _isGaugeFrozen = false;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        UpdateAuthorityMultiplier();
        UpdateAuthorityUI();
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
            UpdateAuthorityMultiplier();
            UpdateAuthorityUI();
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
        
        UpdateAuthorityMultiplier();
        UpdateAuthorityUI();

        Debug.Log($"Authority Increased by {amountToIncrease:F2}! Current Gauge: {authorityGauge:F2}");

        // 게이지가 최대치에 도달하면 초기화 코루틴을 시작합니다.
        if (authorityGauge >= MaxAuthorityGauge)
        {
            StartCoroutine(FeverTimeCoroutine());
        }
    }

    /// <summary>
    /// 게이지가 최대치에 도달하면 피버 타임을 시작하고, 시간이 지나면 종료합니다.
    /// </summary>
    private IEnumerator FeverTimeCoroutine()
    {
        _isGaugeFrozen = true;
        isFeverTime = true;

        Debug.Log($"★★★ 피버 타임 시작! {feverTimeDuration}초 동안 지속됩니다. ★★★");
        
        // ★★★ 여기가 폐하의 명에 따라 개정된 법률이옵니다 ★★★
        // 1. 다른 신하들을 깨우는 보고(UpdateAuthorityMultiplier) 대신, 내부적으로만 조용히 처리합니다.
        authorityMultiplier = feverTimeMultiplier;
        Mover.moveSpeed = Mover.defaultMoveSpeed * authorityMultiplier;
        // 2. 피버타임 시작을 알리는 방송은 딱 한 번만 송출합니다.
        if (onAuthorityChangedChannel != null)
        {
            onAuthorityChangedChannel.RaiseEvent(authorityMultiplier);
        }

        // 정해진 축제 시간만큼 기다립니다.
        yield return new WaitForSeconds(feverTimeDuration);
        
        // --- 축제 종료 ---
        isFeverTime = false;
        authorityGauge = 0f;
        timeSinceLastIncrease = 0f;
        _isGaugeFrozen = false;

        Debug.Log("피버 타임 종료. 권위가 0으로 초기화되었습니다.");

        // ★ 3. 축제가 끝났을 때도, 내부적으로만 조용히 원래 상태로 되돌립니다.
        authorityMultiplier = 1f; // 0레벨의 기본 배율
        Mover.moveSpeed = Mover.defaultMoveSpeed;
        // 4. 피버타임 종료를 알리는 방송도 딱 한 번만 송출합니다.
        if (onAuthorityChangedChannel != null)
        {
            onAuthorityChangedChannel.RaiseEvent(authorityMultiplier);
        }
    }

    /// <summary>
    /// 현재 권위 게이지에 따라 배율 변수를 업데이트합니다.
    /// </summary>
    private void UpdateAuthorityMultiplier()
    {
        // ★ 1. 가장 먼저, 지금이 피버 타임인지 확인합니다.
        if (isFeverTime)
        {
            // 피버 타임이 맞다면, 모든 일반 계산을 건너뛰고
            // 폐하께서 정하신 피버 타임 전용 배율을 즉시 적용합니다.
            authorityMultiplier = feverTimeMultiplier;
            Mover.moveSpeed = Mover.defaultMoveSpeed * authorityMultiplier;
        }
        // ★ 2. 피버 타임이 아니라면, 기존의 레벨별 계산법을 따릅니다.
        else
        {
            int level = Mathf.FloorToInt(authorityGauge / 100);
            authorityMultiplier = Mathf.Max(1f, level + 1);
            Mover.moveSpeed = Mover.defaultMoveSpeed * authorityMultiplier;
        }
        
        // 최종적으로 결정된 배율을 왕국 전체에 방송합니다.
        if (onAuthorityChangedChannel != null)
        {
            onAuthorityChangedChannel.RaiseEvent(authorityMultiplier);
        }
    }

    /// <summary>
    /// 현재 권위 게이지에 따라 UI를 업데이트합니다.
    /// </summary>
    private void UpdateAuthorityUI()
    {
        if (authorityGaugeSlider == null || authorityGaugeText == null) return;

        // 게이지가 최대치일 경우 "MAX"를 표시합니다.
        if (authorityGauge >= MaxAuthorityGauge)
        {
            authorityGaugeText.gameObject.SetActive(true);
            authorityGaugeText.text = "MAX";
            authorityGaugeSlider.value = 100f;
            return;
        }

        int hundreds = Mathf.FloorToInt(authorityGauge / 100);
        float sliderValue = authorityGauge % 100f;

        // 게이지가 100, 200 등 정확히 100의 배수일 때의 예외 처리
        if (authorityGauge > 0 && authorityGauge % 100 == 0)
        {
            sliderValue = 100f;
            hundreds -= 1;
        }
        
        // 텍스트 업데이트 (100 단위 레벨)
        if (hundreds <= 0)
        {
            authorityGaugeText.gameObject.SetActive(false);
        }
        else
        {
            authorityGaugeText.gameObject.SetActive(true);
            authorityGaugeText.text = $"x {hundreds}";
        }

        // 슬라이더 값 업데이트
        authorityGaugeSlider.value = sliderValue;
    }
}
