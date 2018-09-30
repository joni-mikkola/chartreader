using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Point(Vector2 from, Vector2 to) {
        var heading = to - from;
        var distance = heading.magnitude;
        var direction = heading / distance;

        Vector3 centerPos = new Vector3(from.x + to.x, from.y + to.y) / 2;
        transform.position = centerPos;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        //var objectWidthSize = 5f; // 10 = pixels of line sprite, 5f = pixels per units of line sprite.
        transform.localScale = new Vector3(distance*2.6f, 0.5f, transform.localScale.z);
    }
}
