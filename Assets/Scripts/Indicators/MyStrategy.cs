using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyStrategy {
    public static void Apply(List<StockPriceModel> prices) {
        for (int i = 0; i < prices.Count-1; ++i) {
            var price = prices[i];
            var nextPrice = prices[i + 1];
            bool greenPrice = price.open < price.close;
            bool greenNextPrice = nextPrice.open < nextPrice.close;
            if(!greenPrice && greenNextPrice) {
                if(nextPrice.close >= price.open && nextPrice.open <= price.close) {
                    nextPrice.text += "<color=green>Bullish engulfing</color>";
                    nextPrice.trade++;
                }
            } else if(greenPrice && !greenNextPrice) {
                if (price.close <= nextPrice.open && price.open >= nextPrice.close)
                {
                    nextPrice.trade--;
                    nextPrice.text += "<color=red>Bearish engulfing</color>";
                }
            }
        }
    }
}
