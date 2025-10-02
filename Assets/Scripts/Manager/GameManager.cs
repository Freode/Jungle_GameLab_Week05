using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public event Action OnCurrentGoldAmountChanged;                 // 현재 금 소지량 변경 시, 모두 호출
    public event Action OnClickIncreaseTotalAmountChanged;          // 현재 한 번 클릭할 때, 얻는 금의 양 변경 시, 모두 호출
    public event Action OnPeriodIncreaseAmountChanged;              // 주기적으로 얻는 금의 양이 변화했을 때, 모두 호출
    public event Action<int, Color> OnClickIncreaseGoldAmount;      // 현재 클릭으로 금의 양을 새롭게 얻었다고 호출
    public event Action<TechData, int> OnMaxCapacityUpgrade;        // 다른 테크의 최대 수용량 업그레이드 시, 호출
    public event Action<TechData, int> OnCurrentCapacityChanged;    // 다른 테크의 현재 수용량 변동 시, 호출
    public event Action<TechData, int> OnModifyStructureLevel;      // 테크의 레벨이 변경되어 구조체 외형 변경을 호출
    public event Action<float> OnModifyRespawnUselessPeople;        // 테크 레벨에 따라 백수 생성 주기 조정

    [SerializeField] int currentGoldAmount = 0;             // 현재 소지하고 있는 금의 양
    [SerializeField] int clickIncreaseGoldAmountLinear = 1; // 클릭 한 번 시, 획득하는 금의 선형적인 양
    [SerializeField] int periodIncreaseGoldAmountLinear = 0;// 주기적으로 얻는 금의 양이 선형적으로 증가
    [SerializeField] int clickIncreaseGoldAmountRate = 0;   // 클릭 한 번 시, 획득하는 금의 비율 증가 양
    [SerializeField] int periodIncreaseGoldAmountRate = 0;  // 주기적으로 얻는 금의 양이 비율적으로 증가
    // Ending
    [SerializeField] GameObject fadeOutImage;

    private bool isGameOver = false;                        // 게임 종료 여부
    private int clickIncreaseTotalAmount = 0;               // 클릭 한 번 시, 획득하는 양
    private int periodIncreaseTotalAmount = 0;              // 주기적으로 획득하는 총 양

    private Dictionary<AreaType, IncreaseInfo> increaseGoldAmounts;


    private void Awake()
    {
        instance = this;
        Screen.SetResolution(1920, 1080, false);
        increaseGoldAmounts = new Dictionary<AreaType, IncreaseInfo>();
    }

    private void Start()
    {
        AddClickIncreaseTotalAmount();
        AddPeriodIncreaseGoldAmount();
        StartCoroutine(UpdateGoldAmount());
    }

    // 주기적으로 값이 금이 추가
    IEnumerator UpdateGoldAmount()
    {
        while (isGameOver == false)
        {
            yield return new WaitForSeconds(1f);
            AddCurrentGoldAmount(periodIncreaseTotalAmount);
        }
    }

    // 한 번 클릭했을 때, 금의 양을 업데이트하라고 호출
    public void IncreaseGoldAmountWhenClicked(int amount, Color color)
    {
        OnClickIncreaseGoldAmount?.Invoke(amount, color);
        AddCurrentGoldAmount(amount);
    }

    // 특정 테크의 수용량(유사 최대 레벨) 업그레이드
    public void ModifyMaxCapacityEffect(TechData targetTechData, int amount)
    {
        OnMaxCapacityUpgrade?.Invoke(targetTechData, amount);
    }

    // 특정 테크의 현재 수용량 변경
    public void ModifyCurrentCapacity(TechData targetTechData, int amount)
    {
        OnCurrentCapacityChanged?.Invoke(targetTechData, amount);
    }

    // 특정 테크의 레벨이 증가함에 따라 구조물 변경
    public void ModifyStructureLevel(TechData targetType, int amount)
    {
        OnModifyStructureLevel?.Invoke(targetType, amount);
    }

    // 리스폰 주기 변경
    public void ModifyRespawnUselessPeople(float amount)
    {
        OnModifyRespawnUselessPeople?.Invoke(amount);
    }

    // ==========================================================
    //                     Modify Member Value
    // ==========================================================

    // 현재 소지하고 있는 금의 양 변화
    public void AddCurrentGoldAmount(int amount)
    {
        currentGoldAmount += amount;
        // 현재 소유하고 있는 금의 양이 변경되었다고 알림
        OnCurrentGoldAmountChanged?.Invoke();
    }

    // 한 번 클릭할 때, 얻는 금의 양에 대한 선형적 변화
    public void AddClickIncreaseGoldAmountLinear(AreaType type, int amount)
    {
        if (increaseGoldAmounts.ContainsKey(type) == false)
            increaseGoldAmounts.Add(type, new IncreaseInfo());

        increaseGoldAmounts[type].clickLinear += amount;
        AddClickIncreaseTotalAmount();
    }

    // 한 번 클릭할 때, 얻는 금의 양에 대한 비율 변화
    public void AddClickIncreaseGoldAmountRate(AreaType type, int amount)
    {
        if (increaseGoldAmounts.ContainsKey(type) == false)
            increaseGoldAmounts.Add(type, new IncreaseInfo());

        increaseGoldAmounts[type].clickRate += amount;
        AddClickIncreaseTotalAmount();
    }

    // 주기적으로 얻는 금의 선형적 수 변화
    public void AddPeriodIncreaseGoldAmountLinear(AreaType type, int amount)
    {
        if (increaseGoldAmounts.ContainsKey(type) == false)
            increaseGoldAmounts.Add(type, new IncreaseInfo());

        increaseGoldAmounts[type].periodLinear += amount;
        AddPeriodIncreaseGoldAmount();
    }

    // 주기적으로 얻는 금의 비율 변화
    public void AddPeriodIncreaseGoldAmountRate(AreaType type, int amount)
    {
        if (increaseGoldAmounts.ContainsKey(type) == false)
            increaseGoldAmounts.Add(type, new IncreaseInfo());

        increaseGoldAmounts[type].periodRate += amount;
        AddPeriodIncreaseGoldAmount();
    }

    // 주기적으로 얻는 금의 총 변화
    public void AddPeriodIncreaseGoldAmount()
    {
        periodIncreaseTotalAmount = 0;
        foreach (var data in increaseGoldAmounts)
        {
            periodIncreaseTotalAmount += data.Value.periodLinear + (100 + data.Value.periodRate) / 100;
        }
        OnPeriodIncreaseAmountChanged?.Invoke();
    }

    // 한 번 클릭할 때, 얻는 금의 양에 대한 총 변화
    public void AddClickIncreaseTotalAmount()
    {
        clickIncreaseTotalAmount = 0;
        foreach (var data in increaseGoldAmounts)
        {
            clickIncreaseTotalAmount += data.Value.clickLinear + (100 + data.Value.clickRate) / 100;
        }
        OnClickIncreaseTotalAmountChanged?.Invoke();
    }

    // ==========================================================
    //                            Setter
    // ==========================================================


    public void SetIsGameOver(bool isGameOver)
    {
        this.isGameOver = isGameOver;
        // event 추가 예정
        if (this.isGameOver)
            StartCoroutine(CoFadeOut()); // 임시
    }


    public IEnumerator CoFadeOut(float duration = 2.0f)
    {
        fadeOutImage.SetActive(true);
        Image fadeOut = fadeOutImage.GetComponent<Image>();
        if (fadeOut == null) yield break;
        

        // 시작 알파(현재값)와 목표 알파(1.0)
        Color c = fadeOut.color;
        float startA = c.a;
        float endA = 1f;

        // 필요 시 Raycast 막기 (UI 클릭 차단)
        fadeOut.raycastTarget = true;
        fadeOut.gameObject.SetActive(true);

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;                // 일시정지 중에도 동작
            float p = Mathf.Clamp01(t / duration);      // 0→1
            c.a = Mathf.Lerp(startA, endA, p);
            fadeOut.color = c;
            yield return null;
        }

        // 마무리 보정
        c.a = endA;
        fadeOut.color = c;
        SceneManager.LoadScene("EndingScene");
    }



    // ==========================================================
    //                            Getter
    // ==========================================================

    public int GetCurrentGoldAmount() { return currentGoldAmount; }

    public int GetClickIncreaseGoldAmountLinear() { return clickIncreaseGoldAmountLinear; }

    public int GetPeriodIncreaseGoldAmountLinear() { return periodIncreaseGoldAmountLinear; }

    public int GetClickIncreaseGoldAmountRate() { return clickIncreaseGoldAmountRate; }

    public int GetPeriodIncreaseGoldAmountRate() { return periodIncreaseGoldAmountRate; }

    public int GetClickIncreaseTotalAmount() { return clickIncreaseTotalAmount; }

    public int GetPeriodIncreaseTotalAmount() { return periodIncreaseTotalAmount; }

    public bool GetIsGameOver() { return isGameOver; }
}
