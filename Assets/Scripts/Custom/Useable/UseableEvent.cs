using DapperDino.DapperTools.ScriptableEvents.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Useable Event", menuName = "Game Events/Useable Event")]
public class UseableEvent : BaseGameEvent<IUseable> { }