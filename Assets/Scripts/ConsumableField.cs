using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MoneyField;

public class ConsumableField : MonoBehaviour
{
    [SerializeField] public GameModel.ConsumableTypes Type;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _button;
    [SerializeField] private MoneyField _buttonMoneyField;

    private int _count, _coinPrice, _creditPrice;

    private void OnEnable()
    {
        SetCount(_count);
        SetPrice(_coinPrice, _creditPrice);
    }

    public void SetCount(int count)
    {
        _count = count;
        _countText.text = count.ToString();
    }

    public void SetPrice(int coinPrice, int creditPrice)
    {
        _coinPrice = coinPrice;
        _creditPrice = creditPrice;
        if (_button)
        {
            if (coinPrice == 0 && creditPrice == 0)
                _buttonMoneyField.gameObject.SetActive(false);
            else
            {
                _buttonMoneyField.gameObject.SetActive(true);

                var moneyType = coinPrice != 0 ? MoneyTypes.coins : MoneyTypes.credits;
                var count = moneyType == MoneyTypes.coins ? coinPrice : creditPrice;
                _buttonMoneyField.SetData(count, moneyType, false);
                _button.GetComponent<Image>().color = moneyType == MoneyTypes.coins ? MoneyField.CoinsColor : MoneyField.CreditsColor;
            }
        }            
    }
}
