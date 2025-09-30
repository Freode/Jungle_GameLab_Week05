using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public event Action OnCurrentGoldAmountChanged;

    [SerializeField] int currentGoldAmount = 0;
    [SerializeField] int increaseGoldAmount = 0;

    private void Awake()
    {
        instance = this;
    }


    // ==========================================================
    //                          Modifier
    // ==========================================================

    public void AddCurrentGoldAmount(int amount)
    {
        currentGoldAmount += amount;
        // 현재 소유하고 있는 금의 양이 변경되었다고 알림
        OnCurrentGoldAmountChanged?.Invoke();
    }

    public void AddIncreaseGoldAmount(int amount)
    {
        increaseGoldAmount += amount;
    }


    // ==========================================================
    //                      Getter & Setter
    // ==========================================================

    public int GetCurrentGoldAmount() {  return currentGoldAmount; }
    public int GetIncreaseGoldAmount() { return increaseGoldAmount; }
}
