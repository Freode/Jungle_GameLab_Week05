using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StructureApperance : MonoBehaviour
{
    public AreaType areaType;
    public Sprite areaIcon;                     // 건물 아이콘
    public LevelAppearance[] levelAppearances;
    public bool isClearStructure = false;
    public GameObject InfoUI;
    
    public Queue<bool> levelUpQueue = new Queue<bool>();
    public GameObject levelUpQueueUI;

    private SpriteRenderer spriteRenderer;
    private int currentLevel = 0;
    private int finalLevel = 0;
    private int appliedAppearanceLevel = -1;

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

            // Check if we are applying a sprite from a new, higher level tier
            if (appliedAppearanceLevel < levelAppearances[i].level)
            {
                if (appliedAppearanceLevel != -1)
                {
                    levelUpQueue.Enqueue(true);
                }
                appliedAppearanceLevel = levelAppearances[i].level;
            }

            
            break;
        }
    }

    void CheckLevelUpQueue()
    {
        // ui 가 active false 상태이고, queue 의 count 가 0 이상일때
        if (!levelUpQueueUI.activeSelf && levelUpQueue.Count > 0)
        {
            levelUpQueueUI.SetActive(true);
        } else if (levelUpQueue.Count == 0)
        {
            levelUpQueueUI.SetActive(false);
        }
    }
    
    public void LevelUpStructure(int index)
    {
        spriteRenderer.sprite = levelAppearances[index].sprite;
        transform.localScale = levelAppearances[index].scale;
        levelUpQueue.Dequeue();
        levelUpQueueUI.SetActive(false);
    }

    // 마우스 올려 놓기
    private void OnMouseEnter()
    {
        InfoUI.TryGetComponent(out TechInfo techInfo);
        if (techInfo == null) return;

        techInfo.OnActiveInfo(areaType, currentLevel, finalLevel, areaIcon, new Vector3(1920f, 0f, 0f));
    }

    // 마우스가 빠져 나감
    private void OnMouseExit()
    {
        InfoUI.TryGetComponent(out TechInfo techInfo);
        if (techInfo == null) return;

        techInfo.OnInactiveInfo();
    }
    
    // level Up Queue 출력
    [ContextMenu("Print LevelUpQueue")]
    public void PrintLevelUpQueue()
    {
        foreach (var item in levelUpQueue)
        {
            Debug.Log(item);
        }
    }
}
