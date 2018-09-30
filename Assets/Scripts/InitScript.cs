using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Crosstales.FB;

public class InitScript : MonoBehaviour {
    public Camera mainCamera;
    public Text percentageText;
    public Text stockText;

    private int numGuesses;
    private int numCorrect;

    private float offset;

    private bool volumeVisible;
    private int currentIndex;
    private List<GameObject> stockPriceObjects;
    private List<LineScript> rsiLines;
    private List<LineScript> smaLines;
    private List<LineScript> smaLowerLines;
    private List<LineScript> smaUpperLines;
    private GameObject predictionsObject;
    private GameObject stockPricesObject;
    private GameObject RSIObject;
    private GameObject SMAObject;
    private GameObject gridObject;

    private StockPriceReader.StockPriceReaderModel stockPriceReaderModel;

    private const float HorizontalGap = 0.85f;
    private const float VolumeHorizontalScale = 400.0f;
    private const int InterpolationSteps = 3;

    float CenterNormalizeScale(float value, float max, float min) {
        float diff = (max) - (max - min) * 0.5f;
        float coeff = ((max - min) / 2.0f);
        value -= diff;
        value /= coeff;
        return value * Camera.main.orthographicSize * HorizontalGap;
    }

    void AddPrices(List<StockPriceModel> prices) {
        float highMax = 0.0f;
        float lowMin = 10000000.0f;
        float volMax = 0.0f;

        int gapMin = 0;
        int gapMax = currentIndex + prices.Count;
        gapMin = Mathf.Max(0, gapMax - 50);

        for (int i = gapMin; i < gapMax; ++i)
        {
            var s = stockPriceReaderModel.prices[i];
            if (s.high > highMax)
            {
                highMax = s.high;
            }
            if (s.low < lowMin)
            {
                lowMin = s.low;
            }
            if (s.volume > volMax)
            {
                volMax = s.volume;
            }
        }

        //Generate Candles, RSI, SMA objects
        for (int i = 0; i < prices.Count; ++i)
        {
            //candles
            GameObject stockPriceObject = Instantiate(Resources.Load<GameObject>("Prefabs/StockPrice"));
            stockPriceObject.transform.SetParent(stockPricesObject.transform);
            stockPriceObject.transform.position = new Vector3(stockPriceObjects.Count * 0.25f, 0.0f, 0.0f);
            stockPriceObjects.Add(stockPriceObject);

            var stockPriceScript = stockPriceObject.GetComponent<StockPriceScript>();
            stockPriceScript.SetVolumeVisible(volumeVisible);
            float gridLineAlpha = (30.0f + (stockPriceObjects.Count % 2) * 50) / 255.0f;
            stockPriceScript.SetGridAlpha(gridLineAlpha);

            //if(i < prices.Count-1) {
            //sma
            GameObject smaLineObject = Instantiate(Resources.Load<GameObject>("Prefabs/Line"));
            var smaLineScript = smaLineObject.GetComponent<LineScript>();
            smaLineScript.transform.SetParent(SMAObject.transform, false);
            smaLines.Add(smaLineScript);

            GameObject smaLowerLineObject = Instantiate(Resources.Load<GameObject>("Prefabs/SMALine"));
            var smaLowerLineScript = smaLowerLineObject.GetComponent<LineScript>();
            smaLowerLineScript.transform.SetParent(SMAObject.transform, false);
            smaLowerLines.Add(smaLowerLineScript);

            GameObject smaUpperLineObject = Instantiate(Resources.Load<GameObject>("Prefabs/SMALine"));
            var smaUpperLineScript = smaUpperLineObject.GetComponent<LineScript>();
            smaUpperLineScript.transform.SetParent(SMAObject.transform, false);
            smaUpperLines.Add(smaUpperLineScript);

            if (stockPriceObjects.Count < 14)
            {
                smaLineScript.gameObject.SetActive(false);
                smaLowerLineScript.gameObject.SetActive(false);
                smaUpperLineScript.gameObject.SetActive(false);
            }

            //rsi
            GameObject lineObject = Instantiate(Resources.Load<GameObject>("Prefabs/RSILine"));
            var lineScript = lineObject.GetComponent<LineScript>();
            lineScript.transform.SetParent(RSIObject.transform, false);
            rsiLines.Add(lineScript);
        }

        //rsi
        for (int i = 0; i < stockPriceObjects.Count-1; ++i) {
            var line = rsiLines[i];
            line.gameObject.SetActive(true);
            var prevPrice = stockPriceReaderModel.prices[i];
            var price = stockPriceReaderModel.prices[i+1];
            line.Point(new Vector2(i * 0.25f, prevPrice.rsi - 4.5f), new Vector2((i + 1) * 0.25f, price.rsi - 4.5f));
        }
        rsiLines.Last().gameObject.SetActive(false);

        //candles, volume and scaling(candles,volume,sma)
        for (int i = gapMin; i < gapMax; ++i)
        {
            var stockPriceScript = stockPriceObjects[i].GetComponent<StockPriceScript>();
            var s = stockPriceReaderModel.prices[i];

            float high = CenterNormalizeScale(s.high, highMax, lowMin);
            float low = CenterNormalizeScale(s.low, highMax, lowMin);
            float close = CenterNormalizeScale(s.close, highMax, lowMin);
            float open = CenterNormalizeScale(s.open, highMax, lowMin);
            float newVolume = (s.volume / volMax) * VolumeHorizontalScale;
            stockPriceScript.Load(open, close, high, low, newVolume, s.text);

        }

        //scale and draw sma lines
        for (int i = 1; i < smaLines.Count-1; ++i) {
            var price = stockPriceReaderModel.prices[i];
            var prevPrice = stockPriceReaderModel.prices[i-1];
            float sma = CenterNormalizeScale(price.sma, highMax, lowMin);
            float prevSma = CenterNormalizeScale(prevPrice.sma, highMax, lowMin);
            var smaLine = smaLines[i];

            smaLine.Point(new Vector2(i * 0.25f, prevSma), new Vector2((i + 1) * 0.25f, sma));

            float smaUpper = CenterNormalizeScale(price.smaUpper, highMax, lowMin);
            float prevSmaUpper = CenterNormalizeScale(prevPrice.smaUpper, highMax, lowMin);
            var smaUpperLine = smaUpperLines[i];
            smaUpperLine.Point(new Vector2(i * 0.25f, prevSmaUpper), new Vector2((i + 1) * 0.25f, smaUpper));

            float smaLower= CenterNormalizeScale(price.smaLower, highMax, lowMin);
            float prevSmaLower= CenterNormalizeScale(prevPrice.smaLower, highMax, lowMin);
            var smaLowerLine = smaLowerLines[i];
            smaLowerLine.Point(new Vector2(i * 0.25f, prevSmaLower), new Vector2((i + 1) * 0.25f, smaLower));

            if(i > 14) {
                smaLine.gameObject.SetActive(true);
                smaUpperLine.gameObject.SetActive(true);
                smaLowerLine.gameObject.SetActive(true);
            }
        }

        smaLowerLines.Last().gameObject.SetActive(false);
        smaUpperLines.Last().gameObject.SetActive(false);
        smaLines.Last().gameObject.SetActive(false);
    }

    void Start () {
        volumeVisible = true;
        offset = 0.0f;
        currentIndex = 0;
        rsiLines = new List<LineScript>();
        smaLines = new List<LineScript>();
        smaLowerLines = new List<LineScript>();
        smaUpperLines = new List<LineScript>();

        stockPriceReaderModel = new StockPriceReader.StockPriceReaderModel();
        stockPriceObjects = new List<GameObject>();
        predictionsObject = transform.Find("Predictions").gameObject;
        stockPricesObject = transform.Find("StockPrices").gameObject;
        RSIObject = transform.Find("RSI").gameObject;
        SMAObject= transform.Find("SMA").gameObject;
        gridObject = transform.Find("Grid").gameObject;

        RandomStock();
        UpdateCameraPosition();
    }

    private void DestroyChildren(Transform t) {
        foreach(Transform child in t) {
            Destroy(child.gameObject);
        }
    }

    private void ClearAll() {
        DestroyChildren(predictionsObject.transform);
        DestroyChildren(stockPricesObject.transform);
        DestroyChildren(RSIObject.transform);
        DestroyChildren(SMAObject.transform);

        offset = 0.0f;
        currentIndex = 0;
        numCorrect = 0;
        numGuesses = 0;
        rsiLines.Clear();
        smaLines.Clear();
        smaUpperLines.Clear();
        smaLowerLines.Clear();
        stockPriceObjects.Clear();

        stockPriceReaderModel.name = "";
        stockPriceReaderModel.prices.Clear();

        UpdatePercentage();
    }

    public void RandomStock() {
        ClearAll();
        string[] files = Directory.GetFiles(Application.dataPath + "/Data/stocks",
            "*.csv", SearchOption.AllDirectories);
        string randomStock = files[new System.Random().Next(0, files.Length - 1)];

        var stockPriceReader = new StockPriceReader();

        stockPriceReaderModel = stockPriceReader.ParsePrices(randomStock);
        stockText.text = stockPriceReaderModel.name;

        int numAdd = 30;
        AddPrices(stockPriceReaderModel.prices.GetRange(currentIndex, numAdd));
        currentIndex += numAdd;
        UpdateCameraPosition();
    }

    public void OpenStock() {
        string extensions = "csv";
        string path = FileBrowser.OpenSingleFile("Open File", "", extensions);
        if(path != "") {
            ClearAll();
            var stockPriceReader = new StockPriceReader();
            stockPriceReaderModel = stockPriceReader.ParsePrices(path);
            stockText.text = stockPriceReaderModel.name;

            int numAdd = 30;
            AddPrices(stockPriceReaderModel.prices.GetRange(currentIndex, numAdd));
            currentIndex += numAdd;

            UpdateCameraPosition();
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnGuessUpPressed() {
        GuessUp();
        NextPrice();
    }

    public void GuessUp()
    {
        GameObject arrowObject = Instantiate(Resources.Load<GameObject>("Prefabs/Arrow"));
        numGuesses++;

        if (stockPriceReaderModel.prices[currentIndex-1].close <= stockPriceReaderModel.prices[currentIndex].close) {
            numCorrect++;
            arrowObject.transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.green;
        } else {
            arrowObject.transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.red;
        }
        var lastObject = stockPriceObjects.Last();
        var stockPricePosition = lastObject.transform.position;
        arrowObject.transform.position = new Vector3(stockPricePosition.x, Camera.main.orthographicSize * 0.98f, 0.0f);
        arrowObject.transform.SetParent(predictionsObject.transform, false);

        UpdatePercentage();
    }

    public void OnGuessDownPressed() {
        GuessDown();
        NextPrice();
    }

    public void GuessDown() {
        GameObject predictions = GameObject.Find("Main").transform.Find("Predictions").gameObject;
        GameObject arrowObject = Instantiate(Resources.Load<GameObject>("Prefabs/Arrow"));
        numGuesses++;

        if (stockPriceReaderModel.prices[currentIndex - 1].close > stockPriceReaderModel.prices[currentIndex].close)
        {
            numCorrect++;
            arrowObject.transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.green;
            arrowObject.transform.Find("Body").localScale = new Vector3(1.0f, -1.0f, 0.0f);
        }
        else
        {
            arrowObject.transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.red;
            arrowObject.transform.Find("Body").localScale = new Vector3(1.0f, -1.0f, 0.0f);
        }
        var lastObject = stockPriceObjects.Last();
        var stockPricePosition = lastObject.transform.position;
        arrowObject.transform.position = new Vector3(stockPricePosition.x, Camera.main.orthographicSize * 0.98f, 0.0f);
        arrowObject.transform.SetParent(predictionsObject.transform, false);

        UpdatePercentage();
    }

    private void UpdatePercentage() {
        percentageText.text = ((float)numCorrect / numGuesses).ToString("Correct: ##.##%");
    }
	
    private void UpdateCameraPosition() {
        Vector3 lastStockPricePosition = stockPriceObjects[stockPriceObjects.Count - 1].transform.position;
        mainCamera.transform.position = new Vector3(lastStockPricePosition.x - 3.0f + offset, lastStockPricePosition.y, -10);
        var gridPos = gridObject.transform.position;
        gridPos.x = mainCamera.transform.position.x;
        gridObject.transform.position = gridPos;
    }

    public bool NextPrice() {
        bool added = false;
        if(currentIndex < stockPriceReaderModel.prices.Count - 1) {
            int numAdd = 1;
            AddPrices(stockPriceReaderModel.prices.GetRange(currentIndex, numAdd));
            currentIndex += numAdd;
            UpdateCameraPosition();
            added = true;
        }
        return added;
    }

    public void AutoTrade() {
        while(NextPrice()) {
            if(stockPriceReaderModel.prices[currentIndex-1].trade > 0) {
                GuessUp();
            } else if(stockPriceReaderModel.prices[currentIndex-1].trade < 0) {
                GuessDown();
            }
        }
    }

    public void OnVolumeToggled(Toggle toggle) {
        volumeVisible = toggle.isOn;
        foreach(var p in stockPriceObjects) {
            p.GetComponent<StockPriceScript>().SetVolumeVisible(volumeVisible);
        }
    }

    public void OnSMAToggled(Toggle toggle)
    {
        SMAObject.SetActive(toggle.isOn);
    }

    public void OnRSIToggled(Toggle toggle) {
        RSIObject.SetActive(toggle.isOn);
    }

	// Update is called once per frame
	void Update () {
        if(Input.GetKey(KeyCode.LeftArrow)) 
        {
            offset += -0.25f;
            UpdateCameraPosition();
        }
        if(Input.GetKey(KeyCode.RightArrow)) 
        {
            offset += 0.25f;
            UpdateCameraPosition();
        }
	}
}
