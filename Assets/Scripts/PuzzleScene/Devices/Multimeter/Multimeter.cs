using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Components;
public class Multimeter : Device, CircuitComponent
{
    [SerializeField] private float testDisplayValue;
    [SerializeField] private Display display;
    [SerializeField] private bool turnedOn;
    public bool Connected {get; set;}
    [field: SerializeField] public Terminal[] Terminals { get; private set; }

    public static Action created;
    public static Action destroyed;

    private static string prefabName = "Multimeter Device";

    private void Awake()
    {
        created?.Invoke();
        Terminals = gameObject.GetComponentsInChildren<Terminal>(true);

    }

    private void OnDestroy()
    {
        destroyed?.Invoke();
    }

    private void Start()
    {
        display = transform.Find("Multimeter").gameObject.AddComponent<Display>();
        transform.Find("Multimeter").gameObject.AddComponent<Body>();
        DeviceMode = new OffMode();
        turnedOn = false;
        DeviceMode.OnEnter(this);
    }
    public override void ChangeMode(DeviceMode newMode)
    {
        DeviceMode.OnExit();
        newMode.OnEnter(this);
        DeviceMode = newMode;

    }
    private void Update()
    {
    }

    public void DisplayMessage(string message)
    {
        display.Write(message);
    }

    public void TurnOff()
    {
        turnedOn = false;
        display.TurnOff();
    }

    public void TurnOn()
    {
        turnedOn = true;
    }

    public SpiceSharp.Components.Component GetSpiceComponent(string positiveWire, string negativeWire)
    {
        Debug.Log(RichText.Color("Multimeter get spice called", PaletteColor.Red));
        if (DeviceMode is VoltageMode)
        {
            return new SpiceSharp.Components.Resistor(
                GetInstanceID().ToString(),
                positiveWire,
                negativeWire,
                10e6
                );
        }
        else if (DeviceMode is CurrentMode)
        {
                return new SpiceSharp.Components.VoltageSource(
                GetInstanceID().ToString(),
                positiveWire,
                negativeWire,
                0
                );

        }else
        throw new NotImplementedException("Set the multimeter to current or voltage modes for now");
    }


    public static void Spawn()
    {
        var inScene = GameObject.Find(prefabName);
        if (inScene != null) Debug.Log("Multimeter already in scene, cannot spawn.");
        else GameObject.Instantiate(Resources.Load<GameObject>($"Devices/{prefabName}")).name = prefabName;
    }

    public static bool IsAvailable()
    {
        var inScene = GameObject.Find(prefabName);
        return inScene != null;
    }

    public static void Destroy()
    {
        var inScene = GameObject.Find(prefabName);
        if (inScene == null) Debug.Log("Multimeter not in scene, cannot destroy.");
        else GameObject.Destroy(inScene);
    }
}
