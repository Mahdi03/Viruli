using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Attack3DDroppableItem : MonoBehaviour {

    private IEnumerator itemTimeout;
    void Start() {
        itemTimeout = timedOutDestroy(5f);
        StartCoroutine(itemTimeout);
    }
    IEnumerator timedOutDestroy(float timeDelay) {
        yield return new WaitForSeconds(timeDelay);
        if (gameObject != null) {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision) {
        //Check to see if the other object is an enemy
        if (((1 << collision.gameObject.layer) & GameManager.LAYER_Enemy) != 0) {
            //If it is an enemy, do some damage and then destroy ourselves
            
            //We should be guaranteed an enemy controller now
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            enemyController.DamageHealth(5);
            //Remove this 3D object
            StopCoroutine(itemTimeout);
            Destroy(gameObject);
        }

        
    }
}
