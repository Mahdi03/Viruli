using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveringLights : MonoBehaviour {

    Light spotLight;


    // Start is called before the first frame update
    void Start() {
        spotLight = GetComponent<Light>();
    }

    float time = 0f;

    // Update is called once per frame
    void Update() {
        time += Time.smoothDeltaTime;
        spotLight.intensity = 3 * -Mathf.Abs(Mathf.Sin(time / 1.5f)) + 3;
        //spotLight.intensity = 3 * Mathf.Pow(Mathf.Sin(time), 2);
    }
}
