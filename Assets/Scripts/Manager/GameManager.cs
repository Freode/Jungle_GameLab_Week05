using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public event Action OnCurrentGoldAmountChanged;         // 현재 금 소지량 변경 시, 모두 호출
    public event Action OnClickIncreaseTotalAmountChanged;  // 현재 한 번 클릭할 때, 얻는 금의 양 변경 시, 모두 호출
    public event Action OnPeriodIncreaseAmountChanged;      // 주기적으로 얻는 금의 양이 변화했을 때, 모두 호출
    public event Action<int> OnClickIncreaseGoldAmount;          // 현재 클릭으로 금의 양을 새롭게 얻었다고 호출

    [SerializeField] int currentGoldAmount = 0;             // 현재 소지하고 있는 금의 양
    [SerializeField] int clickIncreaseGoldAmountLinear = 0; // 클릭 한 번 시, 획득하는 금의 선형적인 양
    [SerializeField] int periodIncreaseGoldAmount = 0;      // 주기적으로 얻는 금의 양
    [SerializeField] int clickIncreaseGoldAmountRate = 0;   // 클릭 한 번 시, 획득하는 금의 비율 증가 양

    private bool isGameOver = false;                                // 게임 종료 여부
    private int clickIncreaseTotalAmount = 0;               // 클릭 한 번 시, 획득하는 양

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(UpdateGoldAmount());
    }

    // 주기적으로 값이 금이 추가
    IEnumerator UpdateGoldAmount()
    {
        while (isGameOver == false)
        {
            yield return new WaitForSeconds(1f);
            AddCurrentGoldAmount(periodIncreaseGoldAmount);
        }
    }

    // 한 번 클릭했을 때, 금의 양을 업데이트하라고 호출
    public void IncreaseGoldAmountWhenClicked(int amount)
    {
        OnClickIncreaseGoldAmount?.Invoke(amount);
        AddCurrentGoldAmount(amount);
    }

    // ==========================================================
    //                          Modifier
    // ==========================================================

    // 현재 소지하고 있는 금의 양 변화
    public void AddCurrentGoldAmount(int amount)
    {
        currentGoldAmount += amount;
        // 현재 소유하고 있는 금의 양이 변경되었다고 알림
        OnCurrentGoldAmountChanged?.Invoke();
    }

    // 한 번 클릭할 때, 얻는 금의 양에 대한 선형적 변화
    public void AddClickIncreaseGoldAmountLinear(int amount)
    {
        clickIncreaseGoldAmountLinear += amount;
        AddClickIncreaseTotalAmount();
    }

    // 한 번 클릭할 때, 얻는 금의 양에 대한 비율 변화
    public void AddClickIncreaseGoldAmountRate(int amount)
    {
        clickIncreaseGoldAmountRate += amount;
        AddClickIncreaseTotalAmount();
    }

    // 주기적으로 얻는 금의 양 변화
    public void AddPeriodIncreaseGoldAmount(int amount)
    {
        periodIncreaseGoldAmount += amount;
        OnPeriodIncreaseAmountChanged?.Invoke();
    }

    // 한 번 클릭할 때, 얻는 금의 양에 대한 총 변화
    public void AddClickIncreaseTotalAmount()
    {
        clickIncreaseTotalAmount = clickIncreaseGoldAmountLinear * (100 + clickIncreaseGoldAmountRate) / 100;
        OnClickIncreaseTotalAmountChanged?.Invoke();
    }

    // ==========================================================
    //                            Setter
    // ==========================================================

    public void SetIsGameOver(bool isGameOver) { this.isGameOver = isGameOver; }

    // ==========================================================
    //                            Getter
    // ==========================================================

    public int GetCurrentGoldAmount() {  return currentGoldAmount; }
    public int GetClickIncreaseGoldAmountLinear() { return clickIncreaseGoldAmountLinear; }

    public int GetPeriodIncreaseGoldAmount() {return periodIncreaseGoldAmount; }

    public int GetClickIncreaseGoldAmountRate() { return clickIncreaseGoldAmountRate; }

    public int GetClickIncreaseTotalAmount() { return clickIncreaseTotalAmount; }

    public bool GetIsGameOver() {  return isGameOver; }
}
