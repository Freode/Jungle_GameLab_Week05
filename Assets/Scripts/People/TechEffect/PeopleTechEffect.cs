using UnityEngine;

[CreateAssetMenu(fileName = "PeopleTechEffect", menuName = "Scriptable Objects/Tech Effect/People Tech Effect")]
public class PeopleTechEffect : BaseTechEffect
{
    [SerializeField] private AreaType targetArea;

    public override void ApplyTechEffect()
    {
        switch (targetArea)
        {
            case AreaType.Normal:
                Debug.LogWarning("PeopleTechEffect: Target area is None. Effect not applied.");
                break;
            default:
                break;
        }
    }
}
