using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLights : MonoBehaviour {
    // Start is called before the first frame update
    Light spotLight;
    //bool enabledLight = true;
    float minIntensity = 1f, maxIntensity = 3f;
    int smoothing = 1; //Increase for smoother light intensity transition

    Queue<float> smoothingQueue;
    float lastSum = 0;


    void Start() {
        spotLight = GetComponent<Light>();
        smoothingQueue = new Queue<float>(smoothing);
    }
    float delay = 0f;
    // Update is called once per frame
    void Update() {
        delay += Time.deltaTime;
        if (delay > 0.04f) { 
        delay = 0f;
            while (smoothingQueue.Count >= smoothing) {
                lastSum -= smoothingQueue.Dequeue();
            }

            float newVal = Random.Range(minIntensity, maxIntensity);
            smoothingQueue.Enqueue(newVal);
            lastSum += (float)newVal;

            spotLight.intensity = lastSum / (float)smoothingQueue.Count;
        }
        /*
        if (Random.Range(0, 50) == 6f) {
            enabledLight = !enabledLight;
            spotLight.enabled = enabledLight;
        }
        */
        
    }
}
