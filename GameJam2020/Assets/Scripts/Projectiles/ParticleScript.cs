using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public float lifeTime = 10f;
    private float lifeEnd;

    private void Start()
    {
        lifeEnd = Time.time + lifeTime;
    }
    
    void FixedUpdate()
    {
        if (Time.time > lifeEnd) { Destroy(gameObject); }
    }
}
