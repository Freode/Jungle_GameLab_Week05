using TMPro;
using UnityEngine;

public class GoldClickAreaUI : MonoBehaviour
{
    public TextMeshProUGUI textCurrentGoldAmount;       // 현재 골드 소지량
    public TextMeshProUGUI textClickAmount;             // 현재 골드 클릭 시, 획득량
    public TextMeshProUGUI textPeriodAmount;            // 주기적으로 얻는 골드 양 출력

    private void Start()
    {
        GameManager.instance.OnCurrentGoldAmountChanged += PrintCurrentGoldAmount;
        GameManager.instance.OnClickIncreaseTotalAmountChanged += PrintIncreaseGoldAmount;
        GameManager.instance.OnPeriodIncreaseAmountChanged += PrintPeriodGoldAmount;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnCurrentGoldAmountChanged -= PrintCurrentGoldAmount;
        GameManager.instance.OnClickIncreaseTotalAmountChanged -= PrintIncreaseGoldAmount;
        GameManager.instance.OnPeriodIncreaseAmountChanged -= PrintPeriodGoldAmount;
    }

    // 현재 골드 양 출력
    private void PrintCurrentGoldAmount()
    {
        int amount = GameManager.instance.GetCurrentGoldAmount();
        textCurrentGoldAmount.text = "Current Gold\n" + amount;
    }

    // 한 번 클릭 시, 얻는 골드 양 출력
    private void PrintIncreaseGoldAmount()
    {
        int amount = GameManager.instance.GetClickIncreaseTotalAmount();
        textClickAmount.text = "Click Gold\n" + amount;
    }

    // 주기적으로 얻는 골드 양 출력
    private void PrintPeriodGoldAmount()
    {
        int amount = GameManager.instance.GetPeriodIncreaseGoldAmount();
        textPeriodAmount.text = "Period Gold\n" + amount;
    }
}
