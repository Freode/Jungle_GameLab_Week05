using TMPro;
using UnityEngine;

public class TechInfo : MonoBehaviour
{
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDescription;
    public RectTransform infoTranform;

    private float modifyX = 0f;

    private void Start()
    {
        gameObject.SetActive(false);

        modifyX = infoTranform.rect.width + 10f;
    }

    // 테크 정보 활성화
    public void OnActiveInfo(string name, string description, Vector3 loc)
    {
        textName.text = name;
        textDescription.text = description;

        gameObject.transform.position = new Vector3(loc.x - modifyX, loc.y, 0);
        gameObject.SetActive(true);
    }

    // 테크 정보 비활성화
    public void OnInactiveInfo()
    {
        gameObject.SetActive(false);
    }

}
