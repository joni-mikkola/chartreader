using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SMA {
    public static void Calculate(List<StockPriceModel> prices)
    {
        int smaSteps = 10;
        float smaSum = 0.0f;
        for (int i = 0; i < prices.Count; ++i)
        {
            var price = prices[i];
            if (i >= smaSteps)
            {
                price.sma = smaSum / smaSteps;

                List<float> means = new List<float>();
                for (int j = i - smaSteps; j < i; ++j)
                {
                    float diff = prices[j].close - price.sma;
                    float mean = Mathf.Pow(diff, 2.0f);
                    means.Add(mean);
                }
                float meanOfMeans = means.Average();
                float std = Mathf.Sqrt(meanOfMeans);
                price.smaUpper = price.sma + std * 2.0f;
                price.smaLower = price.sma - std * 2.0f;

                //undo
                smaSum += -prices[i - smaSteps].close;
            }
            smaSum += price.close;
        }
    }
}
