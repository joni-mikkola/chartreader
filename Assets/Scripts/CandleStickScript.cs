using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleStickScript : MonoBehaviour {
    public float open;
    public float close;
    public float high;
    public float low;

    private const float BodySizeInPixels = 32.0f;

    // Use this for initialization
    void Start () {
		
	}

    public void Set(float open, float close, float high, float low) {
        this.open = open;
        this.close = close;
        this.high = high;
        this.low = low;

        //calculate line center
        float lineY = (high - low) / 2.0f + low;
        Vector3 linePosition = transform.Find("Line").transform.position;
        linePosition.y = lineY;
        transform.Find("Line").transform.position = linePosition;

        //calculate line size
        var sprite = transform.Find("Line").GetComponent<SpriteRenderer>().sprite;
        float size = (high - low) / sprite.bounds.size.y;
        transform.Find("Line").transform.localScale = new Vector3(0.4f, size, 0.0f);

        //calculate body center
        bool green = true;
        float bodyY = (close - open) / 2.0f + open;
        float bodySize = close - open;
        if (open > close) {
            green = false;
            bodyY = (open - close) / 2.0f + close;
            bodySize = open - close;
        }
        Vector3 bodyPosition = transform.Find("Body").transform.position;
        bodyPosition.y = bodyY;
        transform.Find("Body").transform.position = bodyPosition;

        transform.Find("Body").GetComponent<SpriteRenderer>().color = green ? Color.green : Color.red;

        //calculate body size
        bodySize = bodySize / sprite.bounds.size.y;
        if(bodySize < 3.0f) {
            bodySize = 5.0f;
            transform.Find("Body").GetComponent<SpriteRenderer>().color = Color.black;
        }

        transform.Find("Body").transform.localScale = new Vector3(0.85f, bodySize, 0.0f);

    }

    public void SetGridAlpha(float alpha) {
        var c = transform.Find("GridLine").GetComponent<SpriteRenderer>().color;
        c.a = alpha;
        transform.Find("GridLine").GetComponent<SpriteRenderer>().color = c;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
