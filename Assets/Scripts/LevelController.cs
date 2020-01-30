using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelTriggerType {Half, End};
public class LevelController : MonoBehaviour
{
    public LevelTriggerType type;
    public EnemyActions startEnemy, endEnemy;
    public TankActions tank;
    public Collider stairCollider;
    //public 

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
            else //if(type == LevelTriggerType.Half)
            {
                endEnemy.EnableEnemy();
                tank.PlayerDetected();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == Globals.playerLayer && type == LevelTriggerType.End)
        {
            stairCollider.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other )
    {
        if (other.gameObject.layer == Globals.playerLayer && type == LevelTriggerType.End)
        {
            stairCollider.enabled = false;
        }
    }

}