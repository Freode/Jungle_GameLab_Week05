using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonImageChanger : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Button targetButton;
    
    [Header("Image Settings")]
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private Sprite changedSprite;
    
    [Header("Timing Settings")]
    [SerializeField] private float changeDuration = 0.5f;
    
    private void Start()
    {
        // 컴포넌트 자동 할당
        if (targetButton == null)
            targetButton = GetComponent<Button>();
        
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        
        // 원본 스프라이트 저장
        if (targetImage != null && originalSprite == null)
            originalSprite = targetImage.sprite;
        
        // 버튼 이벤트 연결
        if (targetButton != null)
            targetButton.onClick.AddListener(OnButtonClick);
    }
    
    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 이벤트 해제
        if (targetButton != null)
            targetButton.onClick.RemoveListener(OnButtonClick);
    }
    
    public void OnButtonClick()
    {
        StartCoroutine(ChangeImageTemporarily());
    }
    
    private IEnumerator ChangeImageTemporarily()
    {
        // 이미지나 변경할 스프라이트가 없으면 종료
        if (targetImage == null || changedSprite == null) 
            yield break;
        
        // 원본 스프라이트 백업 (Start에서 설정되지 않은 경우)
        if (originalSprite == null)
            originalSprite = targetImage.sprite;
        
        // 스프라이트 변경
        targetImage.sprite = changedSprite;
        
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(changeDuration);
        
        // 원본 스프라이트로 복원
        targetImage.sprite = originalSprite;
    }
    
    // 외부에서 호출 가능한 메서드들
    public void SetChangeDuration(float duration)
    {
        changeDuration = duration;
    }
    
    public void SetSprites(Sprite original, Sprite changed)
    {
        originalSprite = original;
        changedSprite = changed;
    }
    
    public void SetTargetImage(Image image)
    {
        targetImage = image;
        if (originalSprite == null && image != null)
            originalSprite = image.sprite;
    }
    
    public void SetTargetButton(Button button)
    {
        // 기존 버튼 이벤트 해제
        if (targetButton != null)
            targetButton.onClick.RemoveListener(OnButtonClick);
        
        // 새 버튼 설정 및 이벤트 연결
        targetButton = button;
        if (targetButton != null)
            targetButton.onClick.AddListener(OnButtonClick);
    }
}
