using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private TMP_Text cmdLine;
    [SerializeField] private TypeWriter typeWriter;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    private float backspaceDelay = 0.1f;
    private float backspaceTimer = 0f;
    private Dictionary<string, Action<List<string>>> cmds;
    private bool cmdDisabled = false;
    private EventInstance typingSound;
    private EventInstance menuBGM;
    private string inputBuffer = "";
    private bool showCaret = false;

    private int charactersPerSecond = 200;

    void Start()
    {
        StartCoroutine(BlinkCaret());
        GameManager.Instance.agentName = "Alpha";
        menuBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.menuBGM, true);
        menuBGM.start();
        typingSound = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.typingSound, false);
        cmds = new Dictionary<string, Action<List<string>>> {
            { "help", args => typeWriter.StartTypeWriter(new List<string>()
                {
                    "> help",
                    "Game Help",
                    "play                                | Starts the game",
                    "settings                            | Prints all setting commands",
                    "exit                                | Exits the game",
                    "credits                             | Prints the credits",
                    "help                                | Prints this message",
                    "",
                    "Color Help",
                    "theme                               | List of available themes",
                    "theme <theme>                       | Changes the theme to the specified one",
                    "",
                    "Agent Help",
                    "agent:info                          | Agent details",
                    "agent:current                       | Prints information about the current agent",
                    "agent:list                          | Lists all available agents currently active",
                    "agent:select <agent>                | Selects the specified agent",
                    "",
                    "Stats",
                    "Agent:                     " + GameManager.Instance.agentName,
                    "Difficulty Rating:         " + GameManager.Instance.difficulty,
                    // "", Thought these might be cool
                    // "System Controls",
                    // "echo <message>                      | Echoes the message back to you",
                    // "clear                               | Clears the terminal",
                    // "",
            }, false, true, charactersPerSecond)
            },

            // ---------------------- Game Help ----------------------//

            { "play", args =>
                {
                    cmdDisabled = true;
                    typeWriter.StartTypeWriter(new List<string>()
                    {
                        "> play",
                        "Starting game...",
                        "Loading assets...",
                        "Initializing game state...",
                        "Please wait..."
                    }, false, true, charactersPerSecond, onFinshed: () => {
                        menuBGM.stop(STOP_MODE.ALLOWFADEOUT);
                        StartCoroutine(FadeOut());
                    });

                }},
            { "exit", args =>
                {
                    cmdDisabled = true;
                    typeWriter.StartTypeWriter(new List<string>()
                    {
                        "> exit",
                        "Exiting game...",
                        "Saving game state...",
                        "Goodbye! Unless this is a web version then no this won't do anything."
                    }, false, true, charactersPerSecond, onFinshed: () => {
                        cmdDisabled = false;
                        GameManager.Instance.ExitGame();
                    });
                }},
            { "settings", args => typeWriter.StartTypeWriter(new List<string>()
                {
                    "> settings",
                    "Settings",
                    "settings:volume <value>             | Sets the volume to the specified value (0-100)",
                    "settings:sfx_volume <value>          | Sets the sound effects volume to the specified value (0-100)",
                    // "settings:toggle_sprint              | Toggles sprinting on or off",
                    // "settings:toggle_crouch              | Toggles crouching on or off",
                    // "settings:toggle_lean                | Toggles leaning on or off",
                    // "settings:toggle_camera_spring       | Toggles camera spring on or off",
                    // "settings:toggle_vignette            | Toggles vignette on or off",
            }, false, true, charactersPerSecond)
            },
            { "settings:volume", args =>
                {
                    if (args.Count < 2 || !float.TryParse(args[1], out float volume) || volume < 0f || volume > 100f)
                    {
                        typeWriter.StartTypeWriter(new List<string>()
                        {
                            "> settings:volume",
                            "Usage: settings:volume <value> (0-100)",
                            "Current volume: " + GameManager.Instance.GetSFXVolume() * 100f
                        }, false, true, charactersPerSecond);
                        return;
                    }

                    GameManager.Instance.SetVolume(volume / 100f);
                    typeWriter.StartTypeWriter(new List<string>()
                    {
                        "> settings:volume " + volume,
                        "Volume set to " + volume + "%"
                    }, false, true, charactersPerSecond);
                }
            },
            { "settings:sfx_volume", args =>
                {
                    if (args.Count < 2 || !float.TryParse(args[1], out float sfxVolume) || sfxVolume < 0f || sfxVolume > 100f)
                    {
                        typeWriter.StartTypeWriter(new List<string>()
                        {
                            "> settings:sfx_volume",
                            "Usage: settings:sfx_volume <value> (0-100)",
                            "Current SFX volume: " + GameManager.Instance.GetSFXVolume() * 100f
                        }, false, true, charactersPerSecond);
                        return;
                    }

                    GameManager.Instance.SetSFXVolume(sfxVolume / 100f);
                    typeWriter.StartTypeWriter(new List<string>()
                    {
                        "> settings:sfx_volume " + sfxVolume,
                        "SFX Volume set to " + sfxVolume + "%"
                    }, false, true, charactersPerSecond);
                }
            },
            { "credits", args => typeWriter.StartTypeWriter(new List<string>()
                {
                    "> credits",
                    "Credits",
                    "Citers             | Concept art, Textures, Asset creation, Character/weapon animator, Level design",
                    "Resben             | Programmer, UI/UX development, Gameplay mechanics, Narrative design",
                    "Josh Bakaimis      | Sound design, Music composition, SFX",
                    "Zloysvin           | Game design, AI agents, Narrative design",
                }, false, true, charactersPerSecond)
            },

            // ---------------------- Color Help ----------------------//

            { "theme", args =>
                {
                    if (args.Count < 2)
                    {
                        typeWriter.StartTypeWriter(new List<string>()
                        {
                            "> theme",
                            "Available themes:",
                            "default                             | The default theme",
                            "cybergreen                          | A cyberpunk green theme",
                            "synthblue                           | A synthwave blue theme",
                        }, false, true, charactersPerSecond);
                        return;
                    }

                    switch (args[1])
                    {
                        case "default":
                            typeWriter.StartTypeWriter(new List<string>()
                            {
                                "> theme default",
                                "Theme set to default."
                            }, false, true, charactersPerSecond);
                            break;
                        case "cybergreen":
                            typeWriter.StartTypeWriter(new List<string>()
                            {
                                "> theme dark",
                                "Theme set to dark."
                            }, false, true, charactersPerSecond);
                            break;
                        case "synthblue":
                            typeWriter.StartTypeWriter(new List<string>()
                            {
                                "> theme light",
                                "Theme set to light."
                            }, false, true, charactersPerSecond);
                            break;
                        default:
                            typeWriter.StartTypeWriter(new List<string>()
                            {
                                "> theme",
                                "Available themes:",
                                "default                             | The default theme",
                                "cybergreen                          | A cyberpunk green theme",
                                "synthblue                           | A synthwave blue theme",
                            }, false, true, charactersPerSecond);
                            break;
                    }
                }
            },

            // ---------------------- Agent Help ----------------------//

            { "agent:info", args => typeWriter.StartTypeWriter(new List<string>()
                {
                    "> agent:info",
                    "Some lore on agents"
                }, false, true, charactersPerSecond)
            },
            { "agent:current", args => typeWriter.StartTypeWriter(new List<string>()
                {
                    "> agent:current",
                    "Current agent: " + GameManager.Instance.agentName,
                }, false, true, charactersPerSecond)
            },
            { "agent:list", args => typeWriter.StartTypeWriter(new List<string>()
                {
                    "> agent:list",
                    "Available agents:",
                    "Alpha:      Status: Currently active        | Level Difficulty: 1",
                    "Bravo:      Status: Diseased                | Level Difficulty: 2",
                    "Echo:       Status: Currently inactive      | Level Difficulty: 3",
                    "Romeo:      Status: Currently inactive      | Level Difficulty: 4"
                }, false, true, charactersPerSecond)
            },
            { "agent:select", args =>
                {
                    if (args.Count < 2)
                    {
                        typeWriter.StartTypeWriter(new List<string>()
                        {
                            "> agent:select",
                            "Usage: agent:select <agent>"
                        }, false, true, charactersPerSecond);
                        return;
                    }

                    string agentName = args[1].ToLower();
                    switch (agentName)
                    {
                        case "alpha":
                            // GameManager.Instance.agentName = "Alpha";
                            // GameManager.Instance.difficulty = 1;
                            typeWriter.StartTypeWriter(new List<string>()
                                {
                                    "> agent:select alpha",
                                    "Already selected Alpha agent."
                                }, false, true, charactersPerSecond);
                            break;
                        case "bravo":
                            // GameManager.Instance.agentName = "Bravo";
                            // GameManager.Instance.difficulty = 2;
                            typeWriter.StartTypeWriter(new List<string>()
                                {
                                    "> agent:select bravo",
                                    "Error: Agent is inoperable"
                                }, false, true, charactersPerSecond);
                            break;
                        case "echo":
                            // GameManager.Instance.agentName = "Echo";
                            // GameManager.Instance.difficulty = 3;
                            typeWriter.StartTypeWriter(new List<string>()
                                {
                                    "> agent:select echo",
                                    "Error: Agent is currently inactive"
                                }, false, true, charactersPerSecond);
                            break;
                        case "romeo":
                            // GameManager.Instance.agentName = "Romeo";
                            // GameManager.Instance.difficulty = 4;
                            typeWriter.StartTypeWriter(new List<string>()
                                {
                                    "> agent:select romeo",
                                    "Error: Agent is currently inactive"
                                }, false, true, charactersPerSecond);
                            break;
                        default:
                            typeWriter.StartTypeWriter(new List<string>()
                                {
                                    "> agent:select",
                                    "Usage: agent:select <agent>"
                                }, false, true, charactersPerSecond);
                            break;
                    }
                }
            }
        };

        typeWriter.StartTypeWriter(new List<string>() { "Type 'Help' for commands." }, false, true, charactersPerSecond);
    }

    void Update()
    {
        cmdLine.text = @"C:\ > " + inputBuffer + (showCaret ? "|" : "");

        if (cmdDisabled)
            return;

        if (Input.anyKeyDown)
            typingSound.start();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputBuffer.Length <= 0)
            {
                if (typeWriter.IsTyping())
                    typeWriter.SkipAll();

                return;
            }

            string cmd = inputBuffer.ToLower();
            List<string> argsList = new List<string>(cmd.Split(' '));

            if (argsList.Count != 0 && cmds.ContainsKey(argsList[0]))
            {
                typeWriter.ClearTypeWriter();
                typeWriter.StartTypeWriter(new List<string>() { @"C:\ > " + inputBuffer }, false, true, charactersPerSecond);
                cmds[argsList[0]].Invoke(argsList);
            }
            else
            {
                typeWriter.StartTypeWriter(new List<string>() { "Command not recognized. Use the 'Help' command for a list of commands" }, false, true, charactersPerSecond);
            }

            inputBuffer = "";
        }

        if (Input.GetKey(KeyCode.Backspace) && inputBuffer.Length > 0)
        {
            backspaceTimer -= Time.deltaTime;

            if (backspaceTimer <= 0f)
            {
                inputBuffer = inputBuffer.Substring(0, inputBuffer.Length - 1);
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
                if (inputBuffer.Length <= 31)
                {
                    inputBuffer += c;
                }
            }
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;

        menuBGM.release();
        GameManager.Instance.LoadGame();
    }

    private IEnumerator BlinkCaret()
    {
        while (true)
        {
            showCaret = !showCaret;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
