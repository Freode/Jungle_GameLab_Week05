using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ImageSpriteData
{
    [Header("Image Reference")]
    public Image targetImage;
    
    [Header("Sprite Settings")]
    public Sprite originalSprite;
    public Sprite changedSprite;
    
    [Header("Individual Settings")]
    public float changeDuration = 0.5f;
    public bool useGlobalDuration = true;
    
    [HideInInspector]
    public bool isChanging = false;
}

public class ButtonImageChanger : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private Button targetButton;
    
    [Header("Multiple Image Settings")]
    [SerializeField] private List<ImageSpriteData> imageDataList = new List<ImageSpriteData>();
    
    [Header("Global Settings")]
    [SerializeField] private float globalChangeDuration = 0.5f;
    [SerializeField] private bool changeAllSimultaneously = true;
    
    private void Start()
    {
        // 컴포넌트 자동 할당
        if (targetButton == null)
            targetButton = GetComponent<Button>();
        
        // 이미지 데이터 초기화
        InitializeImageData();
        
        // 버튼 이벤트 연결
        if (targetButton != null)
            targetButton.onClick.AddListener(OnButtonClick);
    }
    
    private void InitializeImageData()
    {
        for (int i = 0; i < imageDataList.Count; i++)
        {
            var data = imageDataList[i];
            
            // 원본 스프라이트 자동 할당
            if (data.targetImage != null && data.originalSprite == null)
            {
                data.originalSprite = data.targetImage.sprite;
            }
            
            // 글로벌 duration 사용 설정
            if (data.useGlobalDuration)
            {
                data.changeDuration = globalChangeDuration;
            }
        }
    }
    
    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 이벤트 해제
        if (targetButton != null)
            targetButton.onClick.RemoveListener(OnButtonClick);
    }
    
    public void OnButtonClick()
    {
        if (changeAllSimultaneously)
        {
            StartCoroutine(ChangeAllImagesSimultaneously());
        }
        else
        {
            StartCoroutine(ChangeImagesSequentially());
        }
    }
    
    private IEnumerator ChangeAllImagesSimultaneously()
    {
        List<Coroutine> runningCoroutines = new List<Coroutine>();
        
        // 모든 이미지 동시에 변경 시작
        foreach (var data in imageDataList)
        {
            if (data.targetImage != null && data.changedSprite != null && !data.isChanging)
            {
                runningCoroutines.Add(StartCoroutine(ChangeImageTemporarily(data)));
            }
        }
        
        // 모든 코루틴이 완료될 때까지 대기
        foreach (var coroutine in runningCoroutines)
        {
            yield return coroutine;
        }
    }
    
    private IEnumerator ChangeImagesSequentially()
    {
        // 하나씩 순차적으로 변경
        foreach (var data in imageDataList)
        {
            if (data.targetImage != null && data.changedSprite != null && !data.isChanging)
            {
                yield return StartCoroutine(ChangeImageTemporarily(data));
            }
        }
    }
    
    private IEnumerator ChangeImageTemporarily(ImageSpriteData data)
    {
        if (data.targetImage == null || data.changedSprite == null || data.isChanging)
            yield break;
        
        data.isChanging = true;
        
        // 원본 스프라이트 백업 (초기화에서 설정되지 않은 경우)
        if (data.originalSprite == null)
            data.originalSprite = data.targetImage.sprite;
        
        // 스프라이트 변경
        data.targetImage.sprite = data.changedSprite;
        
        // 지정된 시간만큼 대기 (개별 설정 또는 글로벌 설정)
        float duration = data.useGlobalDuration ? globalChangeDuration : data.changeDuration;
        yield return new WaitForSeconds(duration);
        
        // 원본 스프라이트로 복원
        data.targetImage.sprite = data.originalSprite;
        
        data.isChanging = false;
    }
    
    // 외부에서 호출 가능한 메서드들
    public void SetGlobalChangeDuration(float duration)
    {
        globalChangeDuration = duration;
        
        // 글로벌 duration을 사용하는 데이터들 업데이트
        foreach (var data in imageDataList)
        {
            if (data.useGlobalDuration)
            {
                data.changeDuration = globalChangeDuration;
            }
        }
    }
    
    public void AddImageData(Image image, Sprite changedSprite, float duration = -1f)
    {
        var newData = new ImageSpriteData
        {
            targetImage = image,
            originalSprite = image?.sprite,
            changedSprite = changedSprite,
            changeDuration = duration > 0 ? duration : globalChangeDuration,
            useGlobalDuration = duration <= 0
        };
        
        imageDataList.Add(newData);
    }
    
    public void RemoveImageData(Image image)
    {
        for (int i = imageDataList.Count - 1; i >= 0; i--)
        {
            if (imageDataList[i].targetImage == image)
            {
                imageDataList.RemoveAt(i);
                break;
            }
        }
    }
    
    public void ClearAllImageData()
    {
        imageDataList.Clear();
    }
    
    public void SetChangeMode(bool simultaneous)
    {
        changeAllSimultaneously = simultaneous;
    }
    
    public void ChangeSpecificImage(int index)
    {
        if (index >= 0 && index < imageDataList.Count)
        {
            StartCoroutine(ChangeImageTemporarily(imageDataList[index]));
        }
    }
    
    public void ChangeSpecificImage(Image targetImage)
    {
        var data = imageDataList.Find(d => d.targetImage == targetImage);
        if (data != null)
        {
            StartCoroutine(ChangeImageTemporarily(data));
        }
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
