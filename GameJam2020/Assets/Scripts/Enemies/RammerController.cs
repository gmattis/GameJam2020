using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RammerController : EnemyScript
{
    public bool isTriggered = false;
    public float explosionDelay = 0.6f;
    // FixedUpdate is called 30 time each seconds
    private void FixedUpdate()
    {
        agent = GetComponent<NavMeshAgent>();
        int notTargetID = 1 - targetID;
        Vector3 directionTarget = (targets[targetID].transform.position - transform.position).normalized;
        if (!isTriggered)
        {
            float distanceTarget = Vector3.Distance(transform.position, targets[targetID].transform.position);
            float distanceNotTarget = Vector3.Distance(transform.position, targets[notTargetID].transform.position);

            // Si le joueur ciblé est à une distance inférieure à celle de la stopping distance, le rammer se tourne vers le joueur et l'attaque
            if (distanceTarget <= agent.stoppingDistance)
            {
                Debug.Log("Explosion initiée");
                isTriggered = true;
                //Attack the target
            }
            if (agent == null)
            {
                Debug.LogError("Le nav mesh agent n'est pas attaché à " + gameObject.name);
            }
            else
            {
                SetDestinationOnPlayer();
            }
            // Si le joueur non ciblé est à une distance inférieure à celle du joueur ciblé (-offset) on change de cible
            targetID = ChangementCible(distanceTarget, distanceNotTarget);
        }
        else
        {
            if (explosionDelay > 0)
            {
                FaceTarget(directionTarget);
                explosionDelay -= Time.fixedDeltaTime;
            }
            else
            {
                Debug.Log("Explosion explosée");
                //Attack the target, faire l'explosion, supprimer le rammer
                //Renderer[] renderers = GetComponentsInChildren<Renderer>();
                //foreach (Renderer renderer in renderers) { renderer.enabled = false; }
                //GetComponent<CapsuleCollider>().enabled = false;
                Destroy(gameObject);
                this.enabled = false;
            }
        }
    }
}
