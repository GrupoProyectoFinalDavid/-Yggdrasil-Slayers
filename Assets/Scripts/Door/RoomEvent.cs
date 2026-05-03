using UnityEngine;
using System;

public abstract class RoomEvent : MonoBehaviour
{
    [Header("Evento - Base")]
    public string eventName = "Evento";

    public bool IsRunning  { get; protected set; }
    public bool IsComplete { get; protected set; }

    public event Action OnEventCompleted;
    
    public virtual void StartEvent()
    {
        if (IsRunning || IsComplete) return;
        IsRunning = true;
        Debug.Log($"[RoomEvent] Iniciando evento: '{eventName}'");
        OnEventStart();
    }

    public virtual void StopEvent()
    {
        IsRunning = false;
        OnEventStop();
    }
    
    protected abstract void OnEventStart();
    protected abstract void OnEventStop();
    
    protected void CompleteEvent()
    {
        if (IsComplete) return;
        IsComplete = true;
        IsRunning  = false;
        Debug.Log($"[RoomEvent] '{eventName}' completado. ¡Puertas desbloqueadas!");
        OnEventCompleted?.Invoke();
    }
}