// 파일 이름: CitizenHighlighter.cs (전면 개정안)
using UnityEngine;
using System.Collections;

public class CitizenHighlighter : MonoBehaviour
{
    [Header("설정")]
    public Color mouseOverHighlightColor = new Color(1f, 1f, 0.5f, 1f);
    public Color selectedHighlightColor = Color.yellow;

    // ★★★ 핵심 개정: Animator 병사가 아닌, EmotionController 장군을 직접 섬기도록 변경 ★★★
    [Header("감정 표현 설정")]
    [Tooltip("감정 표현을 총괄하는 EmotionController 스크립트입니다.")]
    public EmotionController emotionController;

    [Header("구독할 방송 채널")]
    public PeopleActorEventChannelSO OnPeopleSelectedChannel;
    public VoidEventChannelSO OnDeselectedChannel;

    [Header("황금 하사 설정")]
    [Tooltip("선택되었을 때 떨어뜨릴 황금 주머니 오브젝트입니다.")]
    public GameObject dropObject;
    [Tooltip("황금을 한번 떨어뜨린 후, 다음 황금을 떨어뜨리기까지의 대기 시간(초)입니다.")]
    public float goldDropCooldown = 5.0f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isSelected = false;
    private bool isMouseOver = false;
    private bool isBeingDragged = false;
    private bool isGoldDropOnCooldown = false;

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

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (IsMouseCurrentlyOver())
            {
                isBeingDragged = true;
                isMouseOver = false;
                UpdateHighlight();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (isBeingDragged)
            {
                isBeingDragged = false;
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
        if (selectedActor.gameObject == this.gameObject)
        {
            isSelected = true;

            // 1. 감정 표현
            if (emotionController != null)
            {
                emotionController.ExpressEmotion("Emotion_Love");
            }

            // ★★★ 핵심 개정: 쿨타임 규정을 확인하고 황금을 하사합니다 ★★★
            // 황금 주머니가 있고, 현재 쿨타임이 아닐 때만 아래를 실행합니다.
            if (dropObject != null && !isGoldDropOnCooldown)
            {
                // 황금 하사 어명을 내립니다.
                GameManager.instance.DropGoldEasterEgg(dropObject);

                // 즉시 쿨타임 깃발을 올리고, 쿨타임이 끝날 때까지 기다리는 임무를 시작합니다.
                isGoldDropOnCooldown = true;
                StartCoroutine(GoldDropCooldownCoroutine());
            }
        }
        else
        {
            isSelected = false;
        }
        UpdateHighlight();
    }
    private IEnumerator GoldDropCooldownCoroutine()
    {
        // 설정된 쿨타임 시간만큼 기다립니다.
        yield return new WaitForSeconds(goldDropCooldown);

        // 시간이 지나면, 다시 황금을 하사할 수 있도록 쿨타임 깃발을 내립니다.
        isGoldDropOnCooldown = false;
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