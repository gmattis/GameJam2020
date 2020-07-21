using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ShooterController : EnemyScript
{
    public float retreatDistance;
    bool contournement = false;
    float contournementSpeed = 5f; // Vitesse à laquelle le mob se déplace latéralement
    public new Statistics Statistics { get; set; } = new Statistics() { Damages = 1, FireRate = 1f, Life = 5, Speed = 2f };
    // FixedUpdate is called 30 time each seconds
    void FixedUpdate()
    {
        agent = GetComponent<NavMeshAgent>();
        int notTargetID = 1 - targetID;
        Vector3 directionTarget = (targets[targetID].transform.position - transform.position).normalized;
        Vector3 directionNotTarget = (targets[notTargetID].transform.position - transform.position).normalized;
        float distanceTarget = Vector3.Distance(transform.position, targets[targetID].transform.position);
        float distanceNotTarget = Vector3.Distance(transform.position, targets[notTargetID].transform.position);

        if (distanceTarget > agent.stoppingDistance) // Zone de trop loin
        {
            SetDestinationOnPlayer();
            contournement = false;
        }
        else
        {
            if (distanceTarget >= retreatDistance) // Zone de tir
            {
                agent.ResetPath();
                // Contourne si il voit pas sa cible directement
                if (contournement)
                {
                    agent.Move(new Vector3(directionTarget.z, 0, -directionTarget.x) * contournementSpeed * Time.fixedDeltaTime);
                }
                FaceTarget(directionTarget);
                //Attack the target
            }
            else
            {
                if (distanceTarget < retreatDistance) // Zone de danger
                {
                    agent.ResetPath();
                    FaceTarget(directionTarget);
                    agent.Move(directionTarget * agent.speed * Time.fixedDeltaTime * -1f);
                    contournement = false;
                }
            }
        }
        int layerMask = ~(1 << 8 | 1 << 9); // faut voir layerMask c'est compliqué
        Debug.Log(Physics.Linecast(transform.position, targets[targetID].transform.position, layerMask));
        Debug.DrawLine(transform.position, targets[targetID].transform.position);
        if (Physics.Linecast(transform.position, targets[targetID].transform.position, layerMask))
        {
            // Si la cible n'est pas en vision directe
            if (!Physics.Linecast(transform.position, targets[notTargetID].transform.position, layerMask))
            {
                // Si l'autre est en vision directe
                targetID = 1 - targetID;
                contournement = false;
                // On switche de target
            }
            else
            {
                // Si l'autre n'est pas en vision directe
                targetID = ChangementCible(distanceTarget, distanceNotTarget);
                // On prend la plus proche
                contournement = true;
            }
        }
        else
        {
            contournement = false;
            // Si la cible est vision directe
            if (!Physics.Linecast(transform.position, targets[notTargetID].transform.position, layerMask))
            {
                // Si l'autre est en vision directe
                targetID = ChangementCible(distanceTarget, distanceNotTarget); 
                // On prend la plus proche
            }
            // Sinon on change pas de target
        }
    }
}