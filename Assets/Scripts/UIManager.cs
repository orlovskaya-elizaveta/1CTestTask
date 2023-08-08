using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static GameModel;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _popUpPanel, _consumablesWindow, _exchangeWindow, _exchangeCoinsPanel;

    [SerializeField] private MoneyField _coinsCountAppBar, _creditsCountAppBar, _coinsCountExchangePanel, _creditsCountExchangePanel;
    [SerializeField] private ConsumableField[] _consumableFields = new ConsumableField[4];
    [SerializeField] private ErrorField _convertErrorField, _BuyForSilverErrorField, _BuyForGoldErrorField;
    [SerializeField] private TextMeshProUGUI _coinRate, _creditRate, _expectCreditsText;

    [SerializeField] private Button _openConsumablesWindowButton, _openExchangeWindowButton, _cancelButton, _closeWindowButton;
    [SerializeField] private Button _buyCreditsButton, _buyConsumableForSilverButton, _buyConsumableForGoldButton;
    [SerializeField] private TMP_InputField _inputField;

    private ErrorField[] _errorFields;

    private int _coinToCreditRate;

    private void Awake()
    {
        _errorFields = new ErrorField[3] { _convertErrorField, _BuyForSilverErrorField, _BuyForGoldErrorField };

        GameModelHandlers.OnCoinCountUpdate += delegate (int count)
        {
            _coinsCountAppBar.SetData(count, MoneyField.MoneyTypes.coins);
            _coinsCountExchangePanel.SetData(count, MoneyField.MoneyTypes.coins);
        };

        GameModelHandlers.OnCreditCountUpdate += delegate (int count)
        {
            _creditsCountAppBar.SetData(count, MoneyField.MoneyTypes.credits);
            _creditsCountExchangePanel.SetData(count, MoneyField.MoneyTypes.credits);
        };

        GameModelHandlers.OnConsumableCountUpdate += delegate (GameModel.ConsumableTypes type, int count)
        {
            foreach (var field in _consumableFields)
            {
                if (field.Type == type)
                    field.SetCount(count);
            }
        };

        GameModelHandlers.OnConsumablesPriceUpdate += delegate (GameModel.ConsumableTypes type, int coinPrice, int creditPrice)
        {
            foreach (var field in _consumableFields)
            {
                if (field.Type == type)
                    field.SetPrice(coinPrice, creditPrice);
            }
        };

        GameModelHandlers.OnCoinToCreditRateUpdate += delegate (int count)
        {
            _coinToCreditRate = count;
            UpdateCreditRate();
        };

        GameModelHandlers.OnErrorMessageUpdate += delegate (OperationResult operationResult)
        {
            foreach (var field in _errorFields)
            {
                if (field.Guid == operationResult.Guid)
                {
                    field.SetText(operationResult.ErrorDescription);
                    field.Guid = Guid.Empty;
                }
            }
        };

        _openConsumablesWindowButton.onClick.AddListener(OpenConsumablesWindow);
        _openExchangeWindowButton.onClick.AddListener(OpenExchangeWindow);
        _cancelButton.onClick.AddListener(ClosePopUpPanel);
        _closeWindowButton.onClick.AddListener(ClosePopUpPanel);

        _buyCreditsButton.onClick.AddListener(ConvertCoinToCredit);
        _buyConsumableForSilverButton.onClick.AddListener(BuyConsumableForSilver); 
        _buyConsumableForGoldButton.onClick.AddListener(BuyConsumableForGold);

        _inputField.onValueChanged.AddListener(UpdateExpectedCreditsCount);
        _inputField.onEndEdit.AddListener(delegate { ConvertCoinToCredit(); });
    }

    private void UpdateExpectedCreditsCount(string value)
    {
        int count = 0;
        Int32.TryParse(value, out count);
        _expectCreditsText.text = (count * _coinToCreditRate).ToString();
    }

    private void UpdateCreditRate()
    {
        _coinRate.text = "1";
        _creditRate.text = _coinToCreditRate.ToString();
        StartCoroutine(RebuildCoinsPanel());
    }

    private void OpenConsumablesWindow()
    {
        _popUpPanel.SetActive(true);
        _consumablesWindow.SetActive(true);
        _exchangeWindow.SetActive(false);

        _BuyForGoldErrorField.SetText("");
        _BuyForSilverErrorField.SetText("");
    }
    private void OpenExchangeWindow()
    {
        _popUpPanel.SetActive(true);
        _consumablesWindow.SetActive(false);
        _exchangeWindow.SetActive(true);

        UpdateCreditRate();
        _inputField.text = "";
        _convertErrorField.SetText("");
        _expectCreditsText.text = "0";
    }
    private void ClosePopUpPanel()
    {
        _popUpPanel.SetActive(false);
    }
    private void ConvertCoinToCredit()
    {
        int coinsCount = 0;
        Int32.TryParse(_inputField.text, out coinsCount);
        _convertErrorField.Guid = GameModel.ConvertCoinToCredit(coinsCount);
        _inputField.text = "";
    }
    private void BuyConsumableForSilver()
    {
        _BuyForSilverErrorField.Guid = GameModel.BuyConsumableForSilver(GameModel.ConsumableTypes.ArmorPlate);
    }
    private void BuyConsumableForGold()
    {
        _BuyForGoldErrorField.Guid = GameModel.BuyConsumableForGold(GameModel.ConsumableTypes.Medpack);
    }
    private IEnumerator RebuildCoinsPanel()
    {
        yield return new WaitForEndOfFrame();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_exchangeCoinsPanel.transform);
    }
}
