using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LootState {Idle, PickingUp, InInventory};
public enum Type {Weapon, Bonus};
public class Loot : MonoBehaviour
{
    public static Dictionary<GameObject, Loot> objectLootMap = new Dictionary<GameObject, Loot>();

    public LootState state = LootState.Idle;
    public Type type;
    public new Collider collider;
    public float pickUpTime = 0.6f;
    float timerPickUpTime = 0;
    public Animator animator;

    Transform hand;
    Vector3 idlePosition;
    Quaternion idleRotation;
    //Vector3 handPosition;

    void Start()
    {
        objectLootMap.Add(this.gameObject, this);
        idlePosition = transform.position;
        idleRotation = transform.rotation;

    }

    void Update()
    {
        PickingUp();
    }

    public void PickUp(Transform hand) 
    {
        this.hand = hand;
        state = LootState.PickingUp;
    }
    void PickingUp()
    {
        if (state == LootState.PickingUp) 
        {
            timerPickUpTime += Time.deltaTime;
            float pickUpLerpFactor = timerPickUpTime / pickUpTime;
            transform.rotation = Quaternion.Slerp(idleRotation, hand.rotation, pickUpLerpFactor);

            transform.position = Vector3.Lerp(idlePosition,hand.position, pickUpLerpFactor);
            if (timerPickUpTime >= pickUpTime) 
            {
                state = LootState.InInventory;
            }
            
        }
    }

    public void DisableCollider() 
    {
        collider.enabled = false;
    }
    public void EnableCollider() 
    {
        collider.enabled = true;
    }
}
