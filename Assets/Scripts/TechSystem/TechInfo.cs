using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechInfo : MonoBehaviour
{
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDescription;
    public Image techIcon;
    public RectTransform infoTranform;

    private float modifyX = 0f;

    private void Start()
    {
        gameObject.SetActive(false);

        modifyX = infoTranform.rect.width + 10f;
    }

    // 테크 정보 활성화
    public void OnActiveInfo(string name, string description, Sprite icon, Vector3 loc)
    {
        textName.text = name;
        textDescription.text = description;

        if (techIcon != null)
        {
            techIcon.sprite = icon;
        }

        // 기준보다 아래에 있으면, UI 위치를 위로 올리기
        float baseY = infoTranform.rect.height / 2f;
        if (loc.y <= baseY)
            loc.y += 20f;

        gameObject.transform.position = new Vector3(loc.x - modifyX, loc.y, 0);
        gameObject.SetActive(true);
    }

    // 테크 정보 비활성화
    public void OnInactiveInfo()
    {
        gameObject.SetActive(false);
    }

}
