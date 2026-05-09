using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InstantDeathScript : MonoBehaviour
{
	[SerializeField] bool destroyAfterUse = true;
	private bool hasActivated;

	void Awake()
	{
		Collider hazardCollider = GetComponent<Collider>();
		if(hazardCollider != null)
		{
			hazardCollider.isTrigger = false;
		}
	}

	public void ActivateInstantDeath(PlayerHP playerHP)
	{
		if(hasActivated || playerHP == null) return;

		hasActivated = true;
		playerHP.forceKill();

		if(destroyAfterUse)
			Destroy(gameObject);
	}
}
