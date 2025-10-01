using UnityEngine;

[CreateAssetMenu(fileName = "AddClickGoldAmount", menuName = "Scriptable Objects/Tech Effect/Add Click Gold Amount")]
// 한 번 클릭할 때, 얻는 금의 양 증가 효과
public class AddClickIncreaseGoldAmountEffect : BaseTechEffect
{
    public int amount = 0;
    public override void ApplyTechEffect()
    {
        GameManager.instance.AddClickIncreaseGoldAmount(amount);
    }
}
