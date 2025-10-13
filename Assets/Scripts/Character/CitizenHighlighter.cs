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
    [Tooltip("황금과 충성심을 하사한 뒤, 다음 하사까지의 재사용 대기시간(초)입니다.")]
    public float rewardCooldown = 5.0f;
    [Tooltip("선택 시 상승할 충성심의 양입니다.")]
    public int loyaltyBoostAmount = 5;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isSelected = false;
    private bool isMouseOver = false;
    private bool isBeingDragged = false;
    private bool isGoldDropOnCooldown = false;
    private PeopleActor selfActor;

    void Awake()
    {
        selfActor = GetComponent<PeopleActor>(); 
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

            // 1. 감정 표현 (변경 없음)
            if (emotionController != null)
            {
                emotionController.ExpressEmotion("Emotion_Love");
            }

            // ★★★ 핵심 개정: 황금과 충성심을 하나의 어명으로 묶습니다. ★★★
            // '보상 쿨타임'이 아닐 때만 아래를 실행합니다.
            if (dropObject != null && !isGoldDropOnCooldown)
            {
                // 2. 황금 하사
                GameManager.instance.DropGoldEasterEgg(dropObject);
                
                // 3. 충성심 고취 (황금 하사 직후 바로 실행)
                if (selfActor != null)
                {
                    selfActor.ChangeLoyalty(loyaltyBoostAmount);
                    Debug.Log($"{selfActor.DisplayName}의 충성도가 {loyaltyBoostAmount}만큼 상승했습니다!");
                }

                // 4. 통합된 쿨타임을 시작합니다.
                isGoldDropOnCooldown = true;
                StartCoroutine(RewardCooldownCoroutine());
            }
        }
        else
        {
            isSelected = false;
        }
        UpdateHighlight();
    }
    // ★★★ 이름 변경 및 통합: GoldDropCooldownCoroutine -> RewardCooldownCoroutine ★★★
    private IEnumerator RewardCooldownCoroutine()
    {
        // 설정된 '보상 쿨타임' 시간만큼 기다립니다.
        yield return new WaitForSeconds(rewardCooldown);

        // 시간이 지나면, 다시 황금과 충성심을 하사할 수 있도록 쿨타임 깃발을 내립니다.
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