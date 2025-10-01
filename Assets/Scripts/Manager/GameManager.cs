using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public event Action OnCurrentGoldAmountChanged;

    [SerializeField] int currentGoldAmount = 0;             // 현재 소지하고 있는 금의 양
    [SerializeField] int clickIncreaseGoldAmount = 0;       // 클릭 한 번 시, 획득하는 금의 양
    [SerializeField] int periodIncreaseGoldAmount = 0;      // 주기적으로 얻는 금의 양

    bool isGameOver = false;                                // 게임 종료 여부

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

    // 한 번 클릭할 때, 얻는 금의 양 변화
    public void AddClickIncreaseGoldAmount(int amount)
    {
        clickIncreaseGoldAmount += amount;
    }

    // 주기적으로 얻는 금의 양 변화
    public void AddPeriodIncreaseGoldAmount(int amount)
    {
        periodIncreaseGoldAmount += amount;
    }


    // ==========================================================
    //                      Getter & Setter
    // ==========================================================

    public int GetCurrentGoldAmount() {  return currentGoldAmount; }
    public int GetClickIncreaseGoldAmount() { return clickIncreaseGoldAmount; }

    public int GetPeriodIncreaseGoldAmount() {return periodIncreaseGoldAmount; }
}
