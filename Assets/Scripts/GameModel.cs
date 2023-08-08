using System;
using System.Collections.Generic;

public static class GameModel
{
    public enum ConsumableTypes
    {
        None = 0,
        Medpack = 1,
        ArmorPlate = 2
    }

    public struct ConsumableConfig
    {
        public int CreditPrice;
        public int CoinPrice;
    }
    
    public const int CoinToCreditRate = 11;
    public static readonly Dictionary<ConsumableTypes, ConsumableConfig> ConsumablesPrice = new Dictionary<ConsumableTypes, ConsumableConfig>
    {
        {ConsumableTypes.Medpack, new ConsumableConfig { CoinPrice = 100, CreditPrice = 0 }},
        {ConsumableTypes.ArmorPlate, new ConsumableConfig { CoinPrice = 0, CreditPrice = 10 }},
    };
    
    private class OperationInfo
    {
        public Guid Guid;
        public DateTime DateTime;
        public string Type;
        public ConsumableTypes ConsumableType;
        public int Value1;
    }

    public class OperationResult
    {
        public Guid Guid;
        public bool IsSuccess;
        public string ErrorDescription;
    }

    public static event Action<OperationResult> OperationComplete;
    public static event Action ModelChanged;
    public static int CoinCount { get; private set; }
    public static int CreditCount { get; private set; }

    public static int GetConsumableCount(ConsumableTypes consumableTypes)
    {
        return _consumableCount[consumableTypes];
    }
    
    public static bool HasRunningOperations => _operationQueue.Count > 0;

    private static readonly Dictionary<ConsumableTypes, int> _consumableCount = new Dictionary<ConsumableTypes, int>();
    private static readonly Queue<OperationInfo> _operationQueue = new Queue<OperationInfo>();
    
    static GameModel()
    {
        CoinCount = 180;
        CreditCount = 2000;
        _consumableCount[ConsumableTypes.Medpack] = 5;
        _consumableCount[ConsumableTypes.ArmorPlate] = 2;
    }

    private static void HandleOperationComplete(Guid guid, string errorDescription = null)
    {
        OperationComplete?.Invoke(new OperationResult
        {
            Guid = guid,
            IsSuccess = string.IsNullOrEmpty(errorDescription),
            ErrorDescription = errorDescription
        });
    }

    private static bool TryPeek(out OperationInfo result)
    {
        if (_operationQueue.Count > 0)
        {
            result = _operationQueue.Peek();
            return true;
        }
        result = null;
        return false;
    }
    
    public static void Update()
    {
        while (TryPeek(out var operation) && operation.DateTime < DateTime.Now)
        {
            _operationQueue.Dequeue();
            switch (operation.Type)
            {
                case "coin-to-credit":
                {
                    if (CoinCount < operation.Value1)
                    {
                        HandleOperationComplete(operation.Guid, "Not enough gold!");
                    }
                    else
                    {
                        CoinCount -= operation.Value1;
                        CreditCount += operation.Value1 * CoinToCreditRate;
                        HandleOperationComplete(operation.Guid);
                    }
                    break;
                }
                case "consumable-for-credit":
                {
                    if (!ConsumablesPrice.TryGetValue(operation.ConsumableType, out var consumableConfig))
                    {
                        HandleOperationComplete(operation.Guid, "Consumable config not found!");
                    }
                    else if (CreditCount < consumableConfig.CreditPrice)
                    {
                        HandleOperationComplete(operation.Guid, "Not enough credit!");
                    }
                    else
                    {
                        CreditCount -= consumableConfig.CreditPrice;
                        _consumableCount[operation.ConsumableType]++;
                        HandleOperationComplete(operation.Guid);
                    }
                    break;
                }
                case "consumable-for-gold":
                {
                    if (!ConsumablesPrice.TryGetValue(operation.ConsumableType, out var consumableConfig))
                    {
                        HandleOperationComplete(operation.Guid, "Consumable config not found!");
                    }
                    if (CoinCount < consumableConfig.CoinPrice)
                    {
                        HandleOperationComplete(operation.Guid, "Not enough gold!");
                    }
                    else
                    {
                        CoinCount -= consumableConfig.CoinPrice;
                        _consumableCount[operation.ConsumableType]++;
                        HandleOperationComplete(operation.Guid);
                    }
                    break;
                }
            }
            ModelChanged?.Invoke();
        }
    }

    public static Guid ConvertCoinToCredit(int coinToConvert)
    {
        var guid = Guid.NewGuid();
        _operationQueue.Enqueue(new OperationInfo
        {
            Guid = guid,
            DateTime = DateTime.Now + TimeSpan.FromSeconds(3),
            Type = "coin-to-credit",
            Value1 = coinToConvert
        });
        return guid;
    }
    
    public static Guid BuyConsumableForSilver(ConsumableTypes consumableType)
    {
        var guid = Guid.NewGuid();
        _operationQueue.Enqueue(new OperationInfo
        {
            Guid = guid,
            DateTime = DateTime.Now + TimeSpan.FromSeconds(3),
            Type = "consumable-for-credit",
            ConsumableType = consumableType,
        });
        return guid;
    }
    
    public static Guid BuyConsumableForGold(ConsumableTypes consumableType)
    {
        var guid = Guid.NewGuid();
        _operationQueue.Enqueue(new OperationInfo
        {
            Guid = guid,
            DateTime = DateTime.Now + TimeSpan.FromSeconds(3),
            Type = "consumable-for-gold",
            ConsumableType = consumableType,
        });
        return guid;
    }
}