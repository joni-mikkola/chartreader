using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Csv;
using System.Linq;

public class StockPriceReader {
    public class StockPriceReaderModel {
        public string name;
        public List<StockPriceModel> prices;

        public StockPriceReaderModel() {
            prices = new List<StockPriceModel>();
            name = "";
        }
    }

    public StockPriceReaderModel ParsePrices(string file)
    {
        var stockPriceReaderModel = new StockPriceReaderModel();

        List<string> row = new List<string>();
        List<StockPriceModel> s = new List<StockPriceModel>();
        stockPriceReaderModel.name = "Stock: " + System.IO.Path.GetFileNameWithoutExtension(file);
        using (var reader = new CsvFileReader(file))
        {
            while (reader.ReadRow(row))
            {
                //Ignore premarket and aftermarket trading eg. before 0930AM and after 0400PM
                int time = int.Parse(row[1]);
                if (time >= 930 && time <= 1600)
                {
                    float open = float.Parse(row[2]);
                    float high = float.Parse(row[3]);
                    float low = float.Parse(row[4]);
                    float close = float.Parse(row[5]);
                    float volume = float.Parse(row[6]);

                    var stockPriceModel = new StockPriceModel(open, close, high, low, volume);
                    stockPriceReaderModel.prices.Add(stockPriceModel);

                }
            }
        }

        RSI.Calculate(stockPriceReaderModel.prices);
        SMA.Calculate(stockPriceReaderModel.prices);

        //identify and name patterns
        MyStrategy.Apply(stockPriceReaderModel.prices);

        return stockPriceReaderModel;
    }
}
