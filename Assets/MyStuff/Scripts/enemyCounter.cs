using Unity.VisualScripting;
using UnityEngine;

public class enemyCounter : MonoBehaviour
{
    [SerializeField] private int count = 0;
    [SerializeField] private int specifiedCount;
    public GameObject door;

    void Update()
    {
        if (count == specifiedCount)
        {
            Animator doorAnimator = door.GetComponent<Animator>();
            doorAnimator.Play("Open");
            Destroy(gameObject);
        }
    }

    public void addOne()
    {
        count++;
    }
}
