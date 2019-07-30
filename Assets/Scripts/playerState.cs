using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerState : MonoBehaviour
{
    public static string playerId { get; set; } = "3";
    public static string sessionId { get; set; } = "4";
    public static int id { get; set; } = 0;
    public static string gameResult { get; set; }
    public static Queue<System.Object> messageQueue = new Queue<System.Object>();
}
