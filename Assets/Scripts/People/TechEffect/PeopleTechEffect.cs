using UnityEngine;

[CreateAssetMenu(fileName = "PeopleTechEffect", menuName = "Scriptable Objects/Tech Effect/People Tech Effect")]
public class PeopleTechEffect : BaseTechEffect
{
    [SerializeField] private AreaType targetArea;

    public override void ApplyTechEffect()
    {
        GameObject obj = PeopleManager.Instance.SelectOnePerson(targetArea);
        Debug.Log("PeopleTechEffect 적용: " + (obj != null ? obj.name : "선택된 사람 없음"));
        if (obj == null) return;

        switch (targetArea)
        {
            case AreaType.Normal:
                Debug.Log("Normal 영역의 사람을 선택했습니다: " + obj.name);
                break;
            case AreaType.Mine:
                Debug.Log("Mine 영역의 사람을 선택했습니다: " + obj.name);
                PeopleManager.Instance.MoveToArea(obj, AreaType.Mine);
                break;
            case AreaType.Carrier:
                PeopleManager.Instance.MoveToArea(obj, AreaType.Carrier);
                break;
            case AreaType.Architect:
                PeopleManager.Instance.MoveToArea(obj, AreaType.Architect);
                break;
            case AreaType.StoneCarving:
                PeopleManager.Instance.MoveToArea(obj, AreaType.StoneCarving);
                break;
            case AreaType.Gold:
                Debug.Log("Gold 영역의 사람을 선택했습니다: " + obj.name);
                PeopleManager.Instance.MoveToArea(obj, AreaType.Gold);
                break;
            case AreaType.Prison:
                Debug.Log("Prison 영역의 사람을 선택했습니다: " + obj.name);
                break;
            default:
                Debug.LogWarning("알 수 없는 영역 타입입니다: " + targetArea);
                break;
        }
    }
}
