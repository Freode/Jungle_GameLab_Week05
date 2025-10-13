// 파일 이름: WhipController.cs
using UnityEngine;
using System.Collections.Generic; // 여러 백성을 담기 위함

/// <summary>
/// 폐하의 왼손(좌클릭) 어명에 따라, 지정된 범위의 백성들에게 채찍을 내리는 형벌 집행관입니다.
/// </summary>
public class WhipController : MonoBehaviour
{
    [Header("채찍 설정")]
    [Tooltip("채찍이 직접 닿는 안쪽 범위입니다.")]
    public float innerRadius = 2.0f;
    [Tooltip("채찍의 충격이 미치는 바깥쪽 범위입니다.")]
    public float outerRadius = 4.0f;

    [Header("형벌 설정")]
    [Tooltip("안쪽 범위의 백성들이 잃을 충성심입니다.")]
    public int innerLoyaltyPenalty = 5;
    [Tooltip("바깥쪽 범위의 백성들이 잃을 충성심입니다.")]
    public int outerLoyaltyPenalty = 1;
    [Header("시각 효과")]
    [Tooltip("채찍이 떨어질 때 생성할 폭발 효과 프리팹입니다.")]
    public GameObject whipExplosionPrefab;
    

    // Update 감찰 초소에서 폐하의 왼손을 주시합니다.
    void Update()
    {
        // 폐하께서 '왼손'을 내리치는 순간을 감지합니다.
        if (Input.GetMouseButtonDown(0))
        {
            // 어명을 집행합니다!
            ExecutePunishment();
        }
    }

    // 형벌을 집행하는 핵심 임무 (개정안)
    void ExecutePunishment()
    {
        Vector2 whipPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (whipExplosionPrefab != null)
        {
            // 폭발 병기 설계도(Prefab)를 사용하여 실제 폭발을 소환합니다.
            Instantiate(whipExplosionPrefab, whipPoint, Quaternion.identity);
        }

        Collider2D[] allAffected = Physics2D.OverlapCircleAll(whipPoint, outerRadius);
        Collider2D[] directHits = Physics2D.OverlapCircleAll(whipPoint, innerRadius);

        HashSet<Collider2D> punishedCitizens = new HashSet<Collider2D>(directHits);

        // === 중죄인 처벌 (안쪽 원) ===
        foreach (var citizenCollider in directHits)
        {
            HandleDirectHit(citizenCollider.gameObject);
        }

        // === 경범죄인 처벌 (바깥쪽 원) ===
        foreach (var citizenCollider in allAffected)
        {
            if (punishedCitizens.Contains(citizenCollider)) continue;
            HandleNearMiss(citizenCollider.gameObject);
        }

        // ★★★ 추가된 임무: 형벌 결과를 취합하여 권위 상승을 보고하라! ★★★
        if (AuthorityManager.instance != null)
        {
            // 1. 중죄인과 경범죄인의 수를 각각 계산합니다.
            int directHitCount = directHits.Length;
            int nearMissCount = allAffected.Length - directHitCount;

            // 2. 각 죄인 수에 따른 권위 상승량을 계산합니다.
            // (규정은 AuthorityManager가 가지고 있으니, 그에게 물어봅니다)
            float totalAuthorityGained = 
                (directHitCount * AuthorityManager.instance.directHitAuthorityGain) + 
                (nearMissCount * AuthorityManager.instance.nearMissAuthorityGain);
            
            // 3. 만약 상승량이 0보다 크다면, 권위 관리관에게 보고를 올립니다.
            if (totalAuthorityGained > 0)
            {
                AuthorityManager.instance.IncreaseAuthorityByAmount(totalAuthorityGained);
            }
        }
    }

    // 중죄인(안쪽)을 다스리는 절차
    void HandleDirectHit(GameObject citizen)
    {
        PeopleActor actor = citizen.GetComponent<PeopleActor>();
        EmotionController emotion = citizen.GetComponent<EmotionController>();
        CitizenHighlighter highlighter = citizen.GetComponent<CitizenHighlighter>();

        if (actor != null && emotion != null && highlighter != null)
        {
            actor.ChangeLoyalty(-innerLoyaltyPenalty); // 충성심 5 감소
            emotion.ExpressEmotion("Emotion_Angry"); // 분노 표출
            highlighter.FlashRed(); // 붉은 섬광
        }
    }

    // 경범죄인(바깥쪽)을 다스리는 절차
    void HandleNearMiss(GameObject citizen)
    {
        PeopleActor actor = citizen.GetComponent<PeopleActor>();
        EmotionController emotion = citizen.GetComponent<EmotionController>();

        if (actor != null && emotion != null)
        {
            actor.ChangeLoyalty(-outerLoyaltyPenalty); // 충성심 1 감소
            // 폐하께서 말씀하신대로 "Emotion_exclamation"를 표출합니다.
            emotion.ExpressEmotion("Emotion_exclamation"); 
        }
    }

    // 폐하께서 형벌의 범위를 시각적으로 확인하실 수 있도록 돕는 기능이옵니다.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, innerRadius);
    }
}