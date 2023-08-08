using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyField : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] Sprite coinsIcon, creditsIcon;
    public static Color32 CoinsColor = new Color32(255, 213, 76, 255);
    public static Color32 CreditsColor = new Color32(0, 239, 245, 255);
    public static Color32 NeutralColor = new Color32(0, 0, 0, 255);

    private int _count;
    public MoneyTypes Type;
    private bool _setTypeColor = false;

    public enum MoneyTypes
    {
        none = 0,
        coins = 1,
        credits = 2
    }

    private void OnEnable()
    {
        SetData(_count, Type, _setTypeColor);
    }

    public void SetData(int count, MoneyTypes type = MoneyTypes.none, bool setTypeColor = false)
    {
        _count = count;
        Type = type;
        _setTypeColor = setTypeColor;

        text.text = count.ToString();

        if (type == MoneyTypes.coins)
            icon.sprite = coinsIcon;
        if (type == MoneyTypes.credits)
            icon.sprite = creditsIcon;

        if (setTypeColor) SetColor(type);

        RebuildLayout();
    }

    private void SetColor(MoneyTypes moneyType)
    {
        switch (moneyType)
        {
            case MoneyTypes.coins:
                icon.color = CoinsColor;
                text.color = CoinsColor;
                break;
            case MoneyTypes.credits:
                icon.color = CreditsColor;
                text.color = CreditsColor;
                break;
            case MoneyTypes.none:
                icon.color = NeutralColor;
                text.color = NeutralColor;
                break;
            default:
                break;
        }
    }
    public void RebuildLayout()
    {
        if (this.isActiveAndEnabled)
            StartCoroutine(WaitOneFrameThenRebuild());
    }
    private IEnumerator WaitOneFrameThenRebuild()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}
