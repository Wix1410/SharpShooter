using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] int damage = 3;
    [SerializeField] float radius = 1.5f;

    private void Start()
    {
        Explode();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position,radius);
        foreach(Collider hitCollider in hitColliders)
        {
            PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
            if(!playerHealth) continue;
            playerHealth.TakeDamage(damage);
            break;
        }
    }
}
