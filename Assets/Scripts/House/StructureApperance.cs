using UnityEngine;
using UnityEngine.EventSystems;

public class StructureApperance : MonoBehaviour
{
    public AreaType areaType;
    public LevelAppearance[] levelAppearances;
    public bool isClearStructure = false;
    public GameObject InfoUI;

    private SpriteRenderer spriteRenderer;
    private int currentLevel = 0;
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
        currentLevel = level;
        // 클리어 구조체를 모두 완성한 경우
        if (isClearStructure && level >= finalLevel)
        {
            // GameManager.instance.SetIsGameOver(true);
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
    private void OnMouseEnter()
    {
        InfoUI.TryGetComponent(out TechInfo techInfo);
        if (techInfo == null) return;

        techInfo.OnActiveInfo(areaType, currentLevel, finalLevel, null, new Vector3(1920f, 0f, 0f));
    }

    // 마우스가 빠져 나감
    private void OnMouseExit()
    {
        InfoUI.TryGetComponent(out TechInfo techInfo);
        if (techInfo == null) return;

        techInfo.OnInactiveInfo();
    }
}
