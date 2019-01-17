using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class complexDecryptingScript : MonoBehaviour
{
    public KMBombInfo bomb;
    public KMBombModule module;
    public KMAudio audio;
    private string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
    public KMSelectable[] keyboard;
    public KMSelectable Clear;
    public KMSelectable Submit;
    public KMSelectable Del;
    public TextMesh Display;
    private KMSelectable[] press;
    private bool clicked;
    private int textLength;
    private string answer;
    private string original;
    


    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    

    void Awake()
    {
        //  Debug.LogFormat("[Rot Decoding #{0}] " + "Display is", moduleId);
        int batteries = bomb.GetBatteryCount();
        if (batteries == 0)
        {
            batteries = 13;
        }
        moduleId = moduleIdCounter++;
        foreach (KMSelectable key in keyboard)
        {
            KMSelectable pressedKey = key;
            key.OnInteract += delegate () { keyPressed(pressedKey); return false; };
        }
        Clear.OnInteract += delegate () { ClearDisplay(); return false; };
        Del.OnInteract += delegate () { Delete(); return false; };
        Submit.OnInteract += delegate () { SubmitDisplay(); return false; };
        Display.text = "";
        for (int i = 0; i < 12; i++)
        {
            Display.text = Display.text + alphabet[UnityEngine.Random.Range(0, 26)];
            
            
        }
        Debug.LogFormat("[Rot Decoding #{0}] " + "Answer is " + Display.text, moduleId);
        answer = Display.text;

        Invoke("Encrypt", 1);
        press = new KMSelectable[12];
    }

    



    void Encrypt()
    {
        
        int batteries = bomb.GetBatteryCount();
        if (batteries == 0)
        {
            batteries = 13;
        }
        for (int i = 0; i < 12; i++)
        {

            StringBuilder sb = new StringBuilder(Display.text);
            sb[i] = (char)((Display.text[i] - 'A' + batteries) % 26 + 'A');
            Display.text = sb.ToString();

        }

        Debug.LogFormat("[Rot Decoding #{0}] " + "Display is " + Display.text, moduleId);
        original = Display.text;

    }

    void keyPressed(KMSelectable pressedKey)
    {
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.TypewriterKey, transform);
        if (!clicked)
        {
            Display.text = "";
            clicked = true;
        }

        

        if (!moduleSolved)
        {
            Display.text = Display.text + pressedKey.GetComponentInChildren<TextMesh>().text;
           
            Debug.LogFormat("[Rot Decoding #{0}] " + "You pressed " + pressedKey.GetComponentInChildren<TextMesh>().text, moduleId);
        }
       


    }

    void Delete()
    {
        if (!moduleSolved)
        {
            Display.text = Display.text.Remove(Display.text.Length - 1);
            
            Debug.LogFormat("[Rot Decoding #{0}] " + "You removed a letter.", moduleId);
        }
        
    }
    void ClearDisplay()
    {
        if (!moduleSolved)
        {
            Display.text = "";
            
            Debug.LogFormat("[Rot Decoding #{0}] " + "You cleared the display.", moduleId);
        }
        
    }
    void SubmitDisplay()
    {
        if (!moduleSolved)
        {
            
            Debug.LogFormat("[Rot Decoding #{0}] " + "Submitted " + Display.text, moduleId);
            if (Display.text == answer)
            {
                module.HandlePass();
                
                Debug.LogFormat("[Rot Decoding #{0}] " + "Module solved", moduleId);
                moduleSolved = true;
                
            }
            else
            {
                module.HandleStrike();
                Display.text = original;
                clicked = false;
                
                Debug.LogFormat("[Rot Decoding #{0}] " + "Wrong answer. Gave a strike. Returned to original text", moduleId);
                audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.Strike, transform);
            }
        }
        
    }

    KMSelectable[] ProcessTwitchCommand(string command)
    {
        command = command.ToLower();
        press = new KMSelectable[15];
        
        if (command == "Clear" || command == "clear")
        {
            press[0] = Clear;
            return press;
        }

        if (command == "Submit" || command == "submit")
        {
            press[0] = Submit;
            

            return press;
        }
        if (command == "Del" || command == "Delete" || command == "del" || command == "delete")
        {
            press[0] = Del;
            return press;
        }

        if (command != "del" && command != "delete" && command != "clear" && command != "submit" && command.Length == 12) 
        {
            for (int i = 0; i < command.Length; i++)
            {
                command = command.ToUpper();
                press[i] = keyboard[Array.IndexOf(alphabet, command[i].ToString())];
                
            }
            press[12] = Submit;
            return press;
        }
        
        return press;

    }
}