using Unity.VisualScripting;
using UnityEngine;

public class lightBallField : MonoBehaviour
{
    public Material shadow;
    public Material vulnerable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponentInParent<EnemyController>();

            if (enemy != null)
            {
                enemy.damageMult = 2f;
                enemy.setMaterial(vulnerable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.damageMult = 1f;
                enemy.setMaterial(shadow);
            }
        }
    }
}
