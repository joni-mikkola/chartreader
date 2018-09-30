using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSI {
    public static void Calculate(List<StockPriceModel> prices)
    {
        int steps = 14;
        List<float> nums = new List<float>();
        float avgGain = 0.0f;
        float avgLoss = 0.0f;
        float gain = 0.0f;
        float loss = 0.0f;
        for (int i = 0; i < prices.Count - 1; ++i)
        {
            var price = prices[i];
            var nextPrice = prices[i + 1];
            float val = nextPrice.close - price.close;
            if (val < 0.0f)
            {
                loss += -val;
            }
            else
            {
                gain += val;
            }
            if (i >= steps)
            {
                avgGain = gain / steps;
                avgLoss = loss / steps;
                float rs = avgGain / avgLoss;
                float rsi = 3 - (3 / (1 + rs));
                price.rsi = rsi;

                //undo
                var firstPrice = prices[i - steps].close;
                var firstNextPrice = prices[i - steps + 1].close;
                var firstVal = firstNextPrice - firstPrice;
                if (firstVal < 0.0f)
                {
                    loss += firstVal;
                }
                else
                {
                    gain += -firstVal;
                }
            }
        }
    }
}
