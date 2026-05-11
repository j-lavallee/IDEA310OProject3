using UnityEngine;
using UnityEngine.InputSystem;

public class sword : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            gameObject.GetComponent<Animator>().SetTrigger("Swing");
        }
    }
}
