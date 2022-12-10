using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public interface ISpellAction {
	public virtual void EnableSpell() { }
}

[RequireComponent(typeof(ItemInstance))]
public class SpellAction : MonoBehaviour, ISpellAction {

	//TODO: Add potion timeout bar at the top of the potion
	private HealthBarBehavior potionTimeoutController;

	protected int itemID;

	protected float attackRadius;
	protected float timeout;
	protected float timeRemaining;
	private bool isActive = false;
	private AudioSource spellSoundEffect;


	/*Use gizmos to visualize the Physics.OverlapSphere in the editor to see if it matches with the ring prefab - won't be called in the actual build*/
	protected virtual void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, attackRadius);
	}

	protected virtual void Start() {
		itemID = GetComponent<ItemInstance>().itemID;
		IItem potion = InGameItemsDatabaseManager.Instance.getItemByID(itemID);
		attackRadius = potion.EffectRadius * 1.5f;
		timeout = potion.EffectTimeout;
		timeRemaining = timeout;
		potionTimeoutController = transform.GetChild(0).GetComponent<HealthBarBehavior>(); //The first child will be the health bar controller prefab with changed public booleans

		spellSoundEffect = GetComponent<AudioSource>(); //There should be one attached to this prefab

    }
	public virtual void EnableSpell() {
		isActive = true;
		//Play spell's sound effect
		spellSoundEffect.Play();
		spellSoundEffect.loop = true;
		//StartCoroutine(destroySpell());
	}
	protected virtual void EndSpellEffects() { }
	protected IEnumerator destroySpell() {
		yield return new WaitForSeconds(timeout);
		EndSpellEffects();
		Destroy(gameObject);
	}
    protected virtual void Update() { //Make overridable so that different child types of spells can customize Update() functionality
        if (isActive) {
			timeRemaining -= Time.deltaTime;
			potionTimeoutController.UpdateHealthBar(timeRemaining, timeout);
			if (timeRemaining < 0) {
                EndSpellEffects();
                Destroy(gameObject);
            }
		}
    }
}
