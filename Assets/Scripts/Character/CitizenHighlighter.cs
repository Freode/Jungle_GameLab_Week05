// 파일 이름: CitizenHighlighter.cs (개정안)
using UnityEngine;

public class CitizenHighlighter : MonoBehaviour
{
    [Header("설정")]
    public Color mouseOverHighlightColor = new Color(1f, 1f, 0.5f, 1f);
    public Color selectedHighlightColor = Color.yellow;

    [Header("구독할 방송 채널")]
    public PeopleActorEventChannelSO OnPeopleSelectedChannel;
    public VoidEventChannelSO OnDeselectedChannel;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isSelected = false;
    private bool isMouseOver = false;
    private bool isBeingDragged = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) { originalColor = spriteRenderer.color; }
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

    // ★★★ 이 관리인에게도 새로운 감찰 초소 'Update'를 하사합니다 ★★★
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (IsMouseCurrentlyOver())
            {
                isBeingDragged = true;
                isMouseOver = false; // 드래그 시작 시 마우스오버는 즉시 해제
                UpdateHighlight();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (isBeingDragged)
            {
                isBeingDragged = false;
                // 드래그가 끝난 후, 마우스가 여전히 위에 있는지 다시 확인
                isMouseOver = IsMouseCurrentlyOver(); 
                UpdateHighlight();
            }
        }
    }
    
    private void OnMouseEnter()
    {
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

        if (isSelected) { spriteRenderer.color = selectedHighlightColor; }
        else if (isMouseOver) { spriteRenderer.color = mouseOverHighlightColor; }
        else { spriteRenderer.color = originalColor; }
    }
    
    private bool IsMouseCurrentlyOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        return (hit.collider != null && hit.collider.gameObject == this.gameObject);
    }
}