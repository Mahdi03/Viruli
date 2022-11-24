using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public interface ISpellAction {
    public virtual void EnableSpell() { }
}


public class SpellAction : MonoBehaviour, ISpellAction {


    protected int itemID;

    protected float attackRadius;
    protected float timeout;
    private void Start() {
        itemID = GetComponent<ItemInstance>().itemID;
        Potion potion = (Potion)InGameItemsDatabaseManager.Instance.getItemByID(itemID);
    }
    public virtual void EnableSpell() {
        StartCoroutine(destroySpell());
    }
    protected IEnumerator destroySpell() {
        yield return new WaitForSeconds(timeout);
        Destroy(gameObject);
    }
}
