using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class lightBallController : MonoBehaviour
{
    public GameObject lightBallPrefab;
    public GameObject lightBall;
    public GameObject holder;
    private GameObject currentBall;
    public healthBar healthBar;
    public Coroutine regen;

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (currentBall == null)
            {
                lightBall.SetActive(false);
                currentBall = Instantiate(lightBallPrefab, holder.transform.position, holder.transform.rotation);
                currentBall.GetComponent<lightBall>().holder = holder;
                currentBall.GetComponent<lightBall>().player = gameObject;
            }
            else
            {
                currentBall.GetComponent<lightBall>().recall = true;
                currentBall.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            }
        }

        bool shouldRegen = currentBall == null && healthBar.health < healthBar.maxHealth;

        if (shouldRegen)
        {
            if (regen == null)
            {
                regen = StartCoroutine(RegenHealth());
            }
        }
        else
        {
            if (regen != null)
            {
                StopCoroutine(regen);
                regen = null;
            }
        }
    }

    private IEnumerator RegenHealth()
    {
        healthBar hB = healthBar;

        while (hB.health < hB.maxHealth)
        {
            hB.health += 7.5f;
            hB.health = Mathf.Min(hB.health, hB.maxHealth);
            yield return new WaitForSeconds(1.25f);
        }

        regen = null;
    }

    public void recalled()
    {
        currentBall = null;
    }
}
