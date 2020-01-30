using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelTriggerType {Half, End};
public class LevelController : MonoBehaviour
{
    public LevelTriggerType type;
    public EnemyActions startEnemy, endEnemy;
    
    void Start()
    {

    }


    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Globals.playerLayer)
        {
            if (type == LevelTriggerType.End)
            {
                PlayerActions.playerActions.EndLevel();
                startEnemy.EnableEnemy();
            }
            else 
            {
                endEnemy.EnableEnemy();
            }
        }
    }
}