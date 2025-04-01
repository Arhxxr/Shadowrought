using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttack : MonoBehaviour
{
    [SerializeField] GameObject Boss;
    private Collider2D attackHitBox;
    void Start()
    {
        attackHitBox = GetComponent<BoxCollider2D>();
        BossAI BossData = Boss.GetComponent<BossAI>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Attackable"))
        {
            BossAI BossData = other.gameObject.GetComponent<BossAI>();
            BossData.bossHP--;
        }
    }
}
