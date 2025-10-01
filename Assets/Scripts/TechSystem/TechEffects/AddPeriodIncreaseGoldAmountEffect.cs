using UnityEngine;

[CreateAssetMenu(fileName = "AddPeriodGoldAmount", menuName = "Scriptable Objects/Tech Effect/Add Period Gold Amount")]
// 주기적으로 얻는 금의 양 증가 효과
public class AddPeriodIncreaseGoldAmountEffect : BaseTechEffect
{
    public int amount = 0;
    public override void ApplyTechEffect()
    {
        GameManager.instance.AddPeriodIncreaseGoldAmount(amount);
    }
}
