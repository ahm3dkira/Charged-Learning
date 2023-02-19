using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Dialogs;
using Components;
using GameManagement;

public class Level1 : MonoBehaviour
{
    #region UIElements' references
    UIDocument document;
    VisualElement root;
    VisualElement visualElement;

    VisualElement controlsSection;
    Button circuitBreakerBtn, hintBtn, resetBtn, pauseBtn;

    Image gameModeIndicator;

    VisualElement dialog;

    VisualElement editorControls;

    VisualElement tools;

    Button submitBtn;

    VisualElement componentsBar;
    #endregion

    #region UIElements' references for tutorial assets
    List<Image> arrows;
    LevelComponent battery, resistor;
    UILevelComponent UIBattery, UIResistor;
    #endregion

    #region  init

    private void DisableAllSystems()
    {
        // controlsSection.visible = false;
        circuitBreakerBtn.visible = false;
        hintBtn.visible = false;
        resetBtn.visible = false;

        gameModeIndicator.visible = false;

        SetEnabled(editorControls.Q("book-btn"), false);
        SetEnabled(editorControls.Q("normal-btn"), false);
        SetEnabled(editorControls.Q("zoom-in-btn"), false);
        SetEnabled(editorControls.Q("zoom-out-btn"), false);
        SetEnabled(editorControls.Q("wiring-btn"), false);
        SetEnabled(editorControls.Q("pan-btn"), false);

        SetEnabled(tools.Q("devices"), false);
        SetEnabled(tools.Q("terminal-btn"), false);


        submitBtn.visible = false;

        document.gameObject.GetComponent<UILevelComponentsCollapse>().ToggleCollapse(null);

        componentsBar.Q("components-bar-header").SetEnabled(false);
        componentsBar.Q("level-components").SetEnabled(false);

        Camera.Disable();
        Clipboard.Disable();
        FeebackTerminal.Disable();
        GameManager.Disable();
        Selection.Disable();
        WireManager.Disable();
    }
    private void Start()
    {
        InitializeAssets();
        DisableAllSystems();
        PlayDialogSequence1();
    }

    private void InitializeAssets()
    {
        entries = new List<DialogEntry>();
        arrows = new List<Image>();
        for (int i = 0; i < 6; i++) arrows.Add(visualElement.Q<Image>($"arrow{i}"));
        CreateBattery();
    }

    private void CreateBattery()
    {
        UILevelComponent.created += InitBatteryUI;
        battery = Puzzle.CreateLevelComponent("battery", ComponentType.Battery, 1);
        Puzzle.AddProperty(battery, PropertyType.Voltage, 12, 1, "v", true);
    }
    private void InitBatteryUI(UILevelComponent c)
    {
        if (c.levelComponent.Name == "battery")
        {
            UIBattery = c;
            UILevelComponent.created -= InitBatteryUI;
        }
    }

    private void CreateResistor()
    {
        UILevelComponent.created += InitResisorUI;
        resistor = Puzzle.CreateLevelComponent("resistor", ComponentType.Battery, 5);
        Puzzle.AddProperty(resistor, PropertyType.Resistance, 5, 1, "ohm", true);
    }
    private void InitResisorUI(UILevelComponent c)
    {
        if (c.levelComponent.Name == "resistor")
        {
            UIResistor = c;
            UILevelComponent.created -= InitResisorUI;
        }
    }
    private void Awake()
    {
        document = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        root = document.rootVisualElement;

        controlsSection = root.Q("controls-section");
        circuitBreakerBtn = controlsSection.Q<Button>("circuit-breaker");
        hintBtn = controlsSection.Q<Button>("hint-btn");
        resetBtn = controlsSection.Q<Button>("reset-btn");
        pauseBtn = controlsSection.Q<Button>("pause-btn");

        gameModeIndicator = root.Q<Image>("gamemode-indicator");

        dialog = root.Q("dialog");

        editorControls = root.Q("editor-controls");

        tools = root.Q("tools");

        submitBtn = root.Q<Button>("submit-btn");

        componentsBar = root.Q("components-bar");

        VisualTreeAsset uxml = Resources.Load<VisualTreeAsset>("Tutorials/One/content");
        StyleSheet styleSheet = Resources.Load<StyleSheet>("Tutorials/One/style");
        visualElement = uxml.Instantiate().Q("tutorial1");
        root.styleSheets.Add(styleSheet);
        root.Add(visualElement);
    }
    #endregion

    private void ShowImage(Image image, DialogEntry entry)
    {
        image.RemoveFromClassList("disabled");
        entry.ended += () => image.AddToClassList("disabled");
    }

    private void SetEnabled(VisualElement v, bool enabled, bool? visible = null)
    {
        if (visible == null) visible = enabled;
        v.SetEnabled(enabled);
        v.visible = visible.Value;
    }

    private List<DialogEntry> entries;
    private DialogEntry lastEntry;

    private void AddEntry(RichText content)
    {
        var t = new DialogEntry(content);
        entries.Add(t);
        lastEntry = t;
    }

    private void PlayDialogSequence1()
    {
        // 0
        AddEntry("init");

        // 1
        AddEntry("Hi there welcome to the main editor at first let me show you around the place");

        // 2
        AddEntry("First Lets discover the components we have. Click Here.");
        lastEntry.started += () =>
        {
            Dialog.Pause();
            ContinueOnClick(componentsBar.Q("components-bar-header"));
            UIBattery.visualElement.SetEnabled(false);
            componentsBar.Q("components-bar-header").SetEnabled(true);
            ShowImage(arrows[0], entries[2]);
        };

        // 3
        AddEntry("Oh! You Found a Battery.");

        // 4
        AddEntry("Battery Page Unlocked. A battery is a device that stores electric power in the form of chemical energy. When necessary, the energy is again released as electric power.");
        lastEntry.started += () =>
        {
            ShowImage(arrows[2], entries[4]);
            SetEnabled(editorControls.Q("book-btn"), true);
        };
        lastEntry.ended += Book.ShowEmpty;

        // 5
        AddEntry("The collected items information can be found in the collection tab. Work hard to collect all the pages of the book as it your main weapon against the awaited danger.");

        // 6
        AddEntry("Now what are you waiting for grab the Battery abd put it in the grid to start working.");
        lastEntry.started += () =>
        {
            UIBattery.visualElement.SetEnabled(true);
            ShowImage(arrows[1], entries[6]);
            Dialog.Pause();
            ContinueOnClick(UIBattery.visualElement);
        };

        // 7
        AddEntry("Did you know!! YOU can move through as you like through the grid either using the tools up there or the shortcut for each tool");
        lastEntry.started += () =>
        {
            ShowImage(arrows[3], entries[7]);

            SetEnabled(editorControls.Q("normal-btn"), true);
            SetEnabled(editorControls.Q("zoom-in-btn"), true);
            SetEnabled(editorControls.Q("zoom-out-btn"), true);
            SetEnabled(editorControls.Q("pan-btn"), true);

            GameManager.Enable();
            Selection.Enable();
            Camera.Enable();
        };

        // 8
        AddEntry("As we mentioned, the Battery is a DC power source. But we don't know how much energy it gives us. This energy is called Voltage which has it’s own measuring unit Volts");

        // 9
        AddEntry("To measure The voltage of the battery we can a use a Multi-meter (Multi-meter Unlocked). Open the devices menu!");
        lastEntry.started += () =>
        {
            Dialog.Pause();
            ContinueOnClick(tools.Q("devices-btn"));
            ShowImage(arrows[4], entries[9]);
            SetEnabled(tools.Q("devices"), true);
        };

        // 10 
        AddEntry("Now spawn the multi-meter in the editor by clicking on its icon.");
        lastEntry.started += () =>
        {
            Dialog.Pause();
            ShowImage(arrows[5], entries[10]);
            ContinueOnClick(tools.Q("multimeter-btn"));
        };

        // 11 
        AddEntry("Okay the multi-meter is now in the editor. We want to measure the battery's voltage, so set the multi-meter to voltage mode.");
        lastEntry.started += () =>
        {
            Dialog.Pause();
            Handle<MultimeterMode>(ref Multimeter.modeChanged, m =>
            {
                Debug.Log("here");
                if (m is VoltageMode)
                {
                    Knob.SetEnabled(false);
                    Dialog.Continue();
                    return true;
                }
                return false;
            }, (handler) => Multimeter.modeChanged -= handler);
        };



        // 12
        AddEntry("Okay now click on the wire icon to go into wire mode. You can also press 'w' on your keyboard if you're a shortcut person.");


        // WiP...

        var seq1 = new DialogSequence(entries);
        Dialog.PlaySequence(seq1);
    }

    /// <summary>
    /// Subscribe to an action with a handler.
    /// If your handler returns true, it'll call your 'clean' handler.
    /// Otherwise, it'll continue its subscription.
    /// </summary>
    void Handle<T>(ref Action<T> action, Func<T, bool> handle, Action<Action<T>> clean)
    {
        Action<T> _handler = null;
        _handler = c => { if (handle(c)) clean(_handler); };
        action += _handler;
    }

    void ContinueOnClick(VisualElement v)
    {
        EventCallback<ClickEvent> doOnce = null;
        doOnce = ev =>
        {
            Dialog.Continue();
            v.UnregisterCallback<ClickEvent>(doOnce);
        };
        v.RegisterCallback<ClickEvent>(doOnce);
    }
}
