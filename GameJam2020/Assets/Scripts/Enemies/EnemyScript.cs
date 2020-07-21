using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyScript : MonoBehaviour
{
	protected GameObject[] targets;
	public NavMeshAgent agent;
	protected int targetID;
	public float turnSpeed = 12f;

	public Statistics Statistics { get; set; } = new Statistics() { Damages = 1, FireRate = 1f, Life = 5, Speed = 2f };
	public float offsetChangementTarget = 2.5f;

	// INITIALISATION : Le bot cible le joueur le plus proche
	void Start()
	{
		targets = GameObject.FindGameObjectsWithTag("Player");

		// Le rammer cible le joueur le plus proche
		if (Vector3.Distance(transform.position, targets[0].transform.position) < Vector3.Distance(transform.position, targets[1].transform.position))
		{ targetID = 0; }
		else
		{ targetID = 1; }
		Debug.Log("Targeting" + targetID);
	}

	public void Damage(int damages)
	{
		if (Statistics.Life <= damages) { OnKill(); }
		else { Statistics.Life -= damages; OnDamages(); }
	}

	public virtual void OnDamages() { }

	public void OnKill() { Destroy(gameObject); }

	// FONCTION : Le bot se tourne vers sa cible
	protected void FaceTarget(Vector3 directionTarget)
	{
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionTarget.x, 0, directionTarget.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed); // ça c'est pour smooth la rotation
	}
	// FONCTION : Le bot se déplace vers sa cible
	protected void SetDestinationOnPlayer()
	{
		if (targets[targetID] != null)
		{
			Vector3 targetVector = targets[targetID].transform.position;
			agent.SetDestination(targetVector);
		}
		else
		{
			Debug.Log(agent.name + "ne se déplace pas");
		}
	}
	// FONCTION : Le bot change sa cible selon la distance et de l'offset
	protected int ChangementCible(float distanceTarget, float distanceNotTarget)
	{
		if (distanceNotTarget + offsetChangementTarget < distanceTarget)
		{
			return 1 - targetID;
			// On change l'autre est plus proche
		}
		return targetID;
		// On change pas la target est deja le plus proche
	}
}
