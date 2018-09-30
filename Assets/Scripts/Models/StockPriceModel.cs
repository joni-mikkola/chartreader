using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockPriceModel {
    public float open;
    public float close;
    public float high;
    public float low;
    public float volume;
    public float rsi;

    public int trade;
    public string text;

    public float sma;
    public float smaUpper;
    public float smaLower;

    public StockPriceModel(float open, float close, float high, float low, float volume) {
        this.open = open;
        this.close = close;
        this.high = high;
        this.low = low;
        this.volume = volume;
        this.trade = 0;

        //rsi
        this.rsi = 0.0f;

        //sma
        this.sma = 0.0f;
        this.smaUpper = 0.0f;
        this.smaLower = 0.0f;
    }
}
