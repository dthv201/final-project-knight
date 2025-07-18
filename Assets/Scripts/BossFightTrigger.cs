using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class BossFightTrigger : MonoBehaviour
{
    [Header("Assignments")]
    [Tooltip("The dragon, disabled (Inactive) in the scene")]
    public GameObject bossPrefab;
    [Tooltip("CanvasGroup of the boss HP bar (optional)")]
    public CanvasGroup bossUI;
    [Tooltip("Event for the intro – roar, music, camera change")]
    public UnityEvent onIntroStart;

    [Header("Intro")]
    public float introDuration = 3f;   // seconds before the fight starts

    bool fightStarted;
    Transform player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (bossPrefab) bossPrefab.SetActive(false);   // boss off by default
        if (bossUI)     bossUI.alpha = 0;              // hide HP bar
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;                          // ensure the Box is a Trigger
    }

    void OnTriggerEnter(Collider other)
    {
        if (fightStarted || other.transform != player) return;
        StartCoroutine(BeginFight());
    }

    IEnumerator BeginFight()
    {
        fightStarted = true;

        // 1. Play intro effects
        onIntroStart?.Invoke();

        // 2. Enable the boss after a delay
        yield return new WaitForSeconds(introDuration);

        if (bossPrefab) bossPrefab.SetActive(true);
        if (bossUI)     bossUI.alpha = 1;

        // 3. Destroy this trigger so the fight can’t restart
        Destroy(gameObject);
    }
}
