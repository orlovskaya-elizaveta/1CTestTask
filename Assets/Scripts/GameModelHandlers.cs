using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameModel;

public class GameModelHandlers : MonoBehaviour
{
    [SerializeField]
    private UIManager UIManager;

    private int _coinCount;
    private int _creditCount;

    private Dictionary<ConsumableTypes, int> _consumableCount = new Dictionary<ConsumableTypes, int>()
    {
        { ConsumableTypes.Medpack, 0},
        { ConsumableTypes.ArmorPlate, 0}
    };
    private ConsumableTypes[] _supportedConsumableTypes;

    public static event Action<int> OnCoinToCreditRateUpdate;
    public static event Action<ConsumableTypes, int, int> OnConsumablesPriceUpdate;
    public static event Action<int> OnCoinCountUpdate;
    public static event Action<int> OnCreditCountUpdate;
    public static event Action<ConsumableTypes, int> OnConsumableCountUpdate;
    public static event Action<OperationResult> OnErrorMessageUpdate;

    void Update()
    {
        GameModel.Update();
    }

    void Start()
    {
        GameModel.ModelChanged += OnModelChanged;
        GameModel.OperationComplete += OnOperationComplete;

        _supportedConsumableTypes = new ConsumableTypes[_consumableCount.Keys.Count];
        _consumableCount.Keys.CopyTo(_supportedConsumableTypes, 0);

        SetStaticValues();
        UpdateViewData();
    }
    private void OnModelChanged()
    {
        UpdateViewData();
    }

    private void OnOperationComplete(OperationResult operationResult)
    {
        OnErrorMessageUpdate?.Invoke(operationResult);
    }

    public void SetStaticValues()
    {
        Debug.Log("coinToCreditRate = " + GameModel.CoinToCreditRate);
        OnCoinToCreditRateUpdate?.Invoke(GameModel.CoinToCreditRate);

        foreach (var cons in ConsumablesPrice)
        {
            Debug.Log("ConsumablesPrice for " + cons.Key + " = " + cons.Value.CoinPrice + " (CoinPrice), " + cons.Value.CreditPrice + " (CreditPrice)");
            OnConsumablesPriceUpdate?.Invoke(cons.Key, cons.Value.CoinPrice, cons.Value.CreditPrice);
        }
    }
    private void UpdateViewData()
    {
        if (_coinCount != GameModel.CoinCount)
        {
            Debug.Log("update coins " + _coinCount + " -> " + GameModel.CoinCount);
            _coinCount = GameModel.CoinCount;
            OnCoinCountUpdate?.Invoke(_coinCount);
        }
        if (_creditCount != GameModel.CreditCount)
        {
            Debug.Log("update credits " + _creditCount + " -> " + GameModel.CreditCount);
            _creditCount = GameModel.CreditCount;
            OnCreditCountUpdate?.Invoke(_creditCount);
        }

        foreach (var type in _supportedConsumableTypes)
        {
            if (GameModel.GetConsumableCount(type) != _consumableCount[type])
            {
                Debug.Log("update consumable " + type + " " + _consumableCount[type] + " -> " + GameModel.GetConsumableCount(type));
                _consumableCount[type] = GameModel.GetConsumableCount(type);
                OnConsumableCountUpdate?.Invoke(type, GameModel.GetConsumableCount(type));
            }
        }
    }
}
