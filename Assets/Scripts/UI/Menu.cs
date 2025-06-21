using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private TMP_Text cmdLine;
    [SerializeField] private TypeWriter typeWriter;
    private float backspaceDelay = 0.1f;
    private float backspaceTimer = 0f;
    private Dictionary<string, Action> cmds;
    private bool cmdDisabled = false;
    private EventInstance menuLoopSound;
    private EventInstance typingSound;

    void Start()
    {
        // menuLoopSound = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.menuLoopSound);
        typingSound = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.typingSound);
        cmds = new Dictionary<string, Action> {
            { "help", () => typeWriter.StartTypeWriter(new List<string>()
                {
                    "> help",
                    "Basic Help",
                    "help                                | Prints this message",
                    "play                                | Starts the game",
                    "exit                                | Exits the game",
                    "settings                            | Displays setting commands",
                    "echo <message>                      | Echoes the message back to you",
                    "clear                               | Clears the terminal",
                    "",
                    "Color Help",
                    "theme                               | List of available themes",
                    "theme <theme>                       | Changes the theme to the specified one",
                    "",
                    "Agent Help",
                    "agent:help                          | Prints what are agents",
                    "agent:current                       | Prints the current agent",
                    "agent:list                          | Lists all available agents currently active",
                    "agent:view:[camera]                 | Views the camera feed of the specified network",
                    "agent:view:quit                     | Quits the camera feed"
                }, false, true)
            },
            { "play", () =>
            {
                cmdDisabled = true;
                typeWriter.StartTypeWriter(new List<string>()
                {
                    "Starting game...",
                    "Loading assets...",
                    "Initializing game state...",
                    "Please wait..."
                }, false, true, () => GameManager.Instance.LoadGame());

            }},
            { "settings", () => typeWriter.StartTypeWriter(new List<string>()
                {
                    "Settings",
                    "currently not implemented",
                    // "settings:toggle_sprint              | Toggles sprinting on or off",
                    // "settings:toggle_crouch              | Toggles crouching on or off",
                    // "settings:toggle_lean                | Toggles leaning on or off",
                    // "settings:toggle_camera_spring       | Toggles camera spring on or off",
                    // "settings:toggle_vignette            | Toggles vignette on or off",
                }, false, true)
            },
        };

        typeWriter.StartTypeWriter(new List<string>() { "Type 'Help' for commands." }, false, true);
    }

    void Update()
    {
        if (cmdDisabled)
            return;

        if (Input.anyKeyDown)
            typingSound.start();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (cmdLine.text.Length <= 6)
            {
                if (typeWriter.IsTyping())
                    typeWriter.SkipAll();

                return;
            }

            string cmd = cmdLine.text.Trim().ToLower()[6..];

            if (cmds.ContainsKey(cmd))
            {
                typeWriter.ClearTypeWriter();
                typeWriter.StartTypeWriter(new List<string>() { cmdLine.text }, false, true);
                cmds[cmd].Invoke();
            }
            else
            {
                typeWriter.StartTypeWriter(new List<string>() { "Command not recognized. Use the 'Help' command for a list of commands" }, false, true);
            }

            cmdLine.text = @"C:\ > ";
        }

        if (Input.GetKey(KeyCode.Backspace) && cmdLine.text.Length > 6)
        {
            backspaceTimer -= Time.deltaTime;

            if (backspaceTimer <= 0f)
            {
                cmdLine.text = cmdLine.text.Substring(0, cmdLine.text.Length - 1);
                backspaceTimer = backspaceDelay;
            }
        }
        else
        {
            backspaceTimer = 0f;
        }

        foreach (char c in Input.inputString)
        {
            if (!char.IsControl(c))
            {
                if (cmdLine.text.Length <= 31)
                    cmdLine.text += c;
            }
        }
    }
}
