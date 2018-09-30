using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockPriceScript : MonoBehaviour {
    private CandleStickScript candleStickScript;
    private VolumeScript volumeScript;
    private GameObject textObject = null;

    // Use this for initialization
    private void Awake()
    {
        candleStickScript = this.transform.Find("CandleStick").GetComponent<CandleStickScript>();
        volumeScript = this.transform.Find("Volume").GetComponent<VolumeScript>();
    }

    void Start () {

    }

    public CandleStickScript GetCandleStick() {
        return candleStickScript;
    }

    public VolumeScript GetVolume() {
        return volumeScript;
    }
	
    public void Load(float open, float close, float high, float low, float volume, string text) {
        candleStickScript.Set(open, close, high, low);
        volumeScript.Set(volume);

        if(textObject == null) {
            textObject = Instantiate(Resources.Load<GameObject>("Prefabs/CandleText"));
            textObject.GetComponent<TextMesh>().text = text;
            textObject.transform.SetParent(this.transform, false);
            textObject.transform.localPosition = new Vector3(0.0f, 4.15f, 0.0f);
        }
    }

    public void SetVolumeVisible(bool visible) {
        volumeScript.gameObject.SetActive(visible);
    }

    public void SetGridAlpha(float alpha) {
        candleStickScript.SetGridAlpha(alpha);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
