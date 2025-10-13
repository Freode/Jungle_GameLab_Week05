// 파일 이름: CitizenHighlighter.cs (개정안)
using UnityEngine;

public class CitizenHighlighter : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("마우스 커서가 올라갔을 때 적용할 색상입니다.")]
    public Color mouseOverHighlightColor = new Color(1f, 1f, 0.5f, 1f); // 연한 노랑
    [Tooltip("정보창에 선택되었을 때 적용할 색상입니다.")]
    public Color selectedHighlightColor = Color.yellow; // 진한 노랑

    [Header("구독할 방송 채널")]
    public PeopleActorEventChannelSO OnPeopleSelectedChannel;
    public VoidEventChannelSO OnDeselectedChannel;

    // --- 내부 변수 ---
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isSelected = false;
    private bool isMouseOver = false;
    // 드래그 중에는 마우스 오버 하이라이트를 무시하기 위한 깃발
    private bool isBeingDragged = false; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnEnable()
    {
        OnPeopleSelectedChannel.OnEventRaised += OnPeopleSelected;
        OnDeselectedChannel.OnEventRaised += OnDeselected;
    }

    private void OnDisable()
    {
        OnPeopleSelectedChannel.OnEventRaised -= OnPeopleSelected;
        OnDeselectedChannel.OnEventRaised -= OnDeselected;
    }
    
    // 폐하의 손길이 닿았을 때 (드래그 시작)
    private void OnMouseDown()
    {
        isBeingDragged = true;
        // 드래그 시작 시에는 마우스 오버 상태를 즉시 해제하여 색상 충돌을 막습니다.
        isMouseOver = false; 
        UpdateHighlight();
    }
    
    // 폐하의 손길이 떠났을 때 (드래그 종료)
    private void OnMouseUp()
    {
        isBeingDragged = false;
        // 드래그가 끝난 후, 마우스가 여전히 위에 있는지 다시 확인합니다.
        // (Raycast를 통해 정확히 확인하는 것이 정석이나, 일단 간단한 방법으로 구현)
        isMouseOver = IsMouseCurrentlyOver(); 
        UpdateHighlight();
    }

    private void OnMouseEnter()
    {
        // 드래그 중이 아닐 때만 마우스 오버를 감지합니다.
        if (!isBeingDragged) 
        {
            isMouseOver = true;
            UpdateHighlight();
        }
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        UpdateHighlight();
    }
    
    private void OnPeopleSelected(PeopleActor selectedActor)
    {
        isSelected = (selectedActor.gameObject == this.gameObject);
        UpdateHighlight();
    }

    private void OnDeselected()
    {
        isSelected = false;
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (spriteRenderer == null) return;

        // 폐하의 지엄한 '지목'이 최우선 순위를 갖습니다.
        if (isSelected)
        {
            spriteRenderer.color = selectedHighlightColor;
        }
        // 지목되지 않았을 때만 '시선'에 반응합니다.
        else if (isMouseOver)
        {
            spriteRenderer.color = mouseOverHighlightColor;
        }
        // 그 외의 경우에는 본래의 색을 유지합니다.
        else
        {
            spriteRenderer.color = originalColor;
        }
    }

    // 마우스가 현재 이 오브젝트 위에 있는지 간단히 확인하는 함수
    private bool IsMouseCurrentlyOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        return (hit.collider != null && hit.collider.gameObject == this.gameObject);
    }
}