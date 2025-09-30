using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int currentGoldAmount = 0;
    private int increaseGoldAmount = 0;

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
