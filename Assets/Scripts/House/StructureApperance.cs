using UnityEngine;

public class StructureApperance : MonoBehaviour
{
    public LevelAppearance[] levelAppearances;
    public bool isClearStructure = false;

    private SpriteRenderer spriteRenderer;
    private int finalLevel = 0;

    void OnValidate()
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
}
