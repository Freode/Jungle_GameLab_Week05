using UnityEngine;
using UnityEngine.EventSystems;

public class StructureApperance : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LevelAppearance[] levelAppearances;
    public bool isClearStructure = false;

    [SerializeField] private VoidEventChannelSO OnStructureInfoActive;
    [SerializeField] private VoidEventChannelSO OnStructureInfoInactive;

    private SpriteRenderer spriteRenderer;
    private int finalLevel = 0;

    void Start()
    {
        // 에디터에서 값 변경 시 실시간으로 적용
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        finalLevel = levelAppearances[levelAppearances.Length - 1].level;
    }

    // 레벨에 따른 외형 변경
    public void UpdateApperanceByLevel(int level)
    {
        // 클리어 구조체를 모두 완성한 경우
        if (isClearStructure && level >= finalLevel)
        {
            GameManager.instance.SetIsGameOver(true);
        }

        for (int i = levelAppearances.Length - 1; i >= 0; i--)
        {
            if (level < levelAppearances[i].level)
                continue;

            spriteRenderer.sprite = levelAppearances[i].sprite;
            transform.localScale = levelAppearances[i].scale;
            break;
        }
    }

    // 마우스 올려 놓기
    public void OnPointerEnter(PointerEventData eventData)
    {
        //// 위치 초기화가 되지 않았을 때만 진행
        //if (upperY == 5000f)
        //{
        //    Vector3[] corners = new Vector3[4];
        //    totalRectTransform.GetWorldCorners(corners);
        //    upperY = corners[1].y;
        //    leftX = corners[1].x;
        //}

        //Vector3 loc = new Vector3(leftX, upperY, 0);
        //OnActiveInfo?.Invoke(techState.techData.techName, techState.techData.techDescription, techState.techData.techIcon, loc);
    }

    // 마우스가 빠져 나감
    public void OnPointerExit(PointerEventData eventData)
    {
       // OnInactiveInfo?.Invoke();
    }
}
