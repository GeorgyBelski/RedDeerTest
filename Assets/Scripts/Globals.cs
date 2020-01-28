using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public static int groundLayer = 8;
    public static int groundMask = 1 << groundLayer;

    public static int lootLayer = 9;

    public static int weaponLayer = 10;

    public static int enemyLayer = 11;
    public static int springboardLayer = 12;
}
