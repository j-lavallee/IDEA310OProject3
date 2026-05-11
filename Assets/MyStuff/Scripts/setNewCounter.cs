using UnityEngine;

public class setNewCounter : MonoBehaviour
{
    public sword sword;
    public enemyCounter newCounter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //sword.enemyCounter = newCounter;
        }
    }
}
