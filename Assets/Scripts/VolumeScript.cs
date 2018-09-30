using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeScript : MonoBehaviour {
    private const float BodySizeInPixels = 32.0f;

    public float volume;
	// Use this for initialization
	void Start () {

	}

    public void Set(float volume) {
        this.volume = volume;

        GameObject bodyObject = transform.Find("Body").gameObject;
        var sprite = transform.Find("Body").GetComponent<SpriteRenderer>().sprite;
        bodyObject.transform.localScale = new Vector3(1.2f, volume, 0.0f);

        float cameraHeightPixels = Camera.main.pixelHeight;
        float heightWorldSize = (1 * volume) / cameraHeightPixels;

        float realHeight = volume * sprite.bounds.size.y;
        Vector3 bodyPosition = bodyObject.transform.position;
        bodyPosition.y = -Camera.main.orthographicSize + realHeight - realHeight*0.5f;
        bodyObject.transform.position = bodyPosition;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
