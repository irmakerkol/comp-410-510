using System.Collections;
using UnityEngine;


public class Skill : MonoBehaviour
{
    protected Transform mT;


    public float skillDuration;

    public Manager manager;

    protected virtual void Start()
    {
        mT = GetComponent<Transform>();
    }

    public virtual void onActivate()
    {
        gameObject.SetActive(true);
        skill_CR = StartCoroutine(Skill_CR());
    }

    protected Coroutine skill_CR;

    protected virtual IEnumerator Skill_CR()
    {
        this.transform.position = manager.player.gameObject.transform.position + new Vector3(0, 2, 3);
        float elapsed = 0f; // elapsed time since start of movement
        while (elapsed < skillDuration)
        {
            transform.position += new Vector3(0f, y: 0f, Time.deltaTime * 2 * manager.player.speed);
            elapsed += Time.deltaTime; // update elapsed time
            yield return null; // wait for the next frame
        }
        gameObject.SetActive(false); // disable the object
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Obstacle target = other.gameObject.GetComponentInParent<Obstacle>();
        if (target == null) return;

        this.gameObject.SetActive(false);
        target.mainObject.gameObject.SetActive(false);
    }
}
