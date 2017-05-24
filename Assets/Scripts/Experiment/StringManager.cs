using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PsychImmersion.Experiment
{
    public static class StringManager
    {
        private static readonly Dictionary<string, string> _strings;

        static StringManager()
        {
            _strings = new Dictionary<string, string>();
            Reload();
        }

        public static void Reload()
        {
            _strings.Clear();
            if (!File.Exists("strings.txt"))
            {
                CreateDefaultFile();
            }
            var lines = File.ReadAllLines("strings.txt");
            foreach (var line in lines)
            {
                if(string.IsNullOrEmpty(line)) continue;
                var trimmed = line.Trim();
                if(string.IsNullOrEmpty(trimmed)) continue;
                if(trimmed.StartsWith("#")) continue;
                var split = trimmed.Split(new[] {'='}, 2);
                if (split.Length != 2)
                {
                    Debug.LogError("Bad line in strings.txt: \""+trimmed+"\"");
                    continue;
                }
                _strings.Add(split[0], PreprocessString(split[1]));
            }
        }

        public static string GetString(string key)
        {
            return PostProcessString(_strings[key]);
        }

        private static string PostProcessString(string str)
        {
            return str.Replace("{animal}", ExperimentManager.Instance.GetAnimalString().ToLower());
        }

        private static string PreprocessString(string str)
        {
            str = str.Replace("{newline}", "\n");
            str = str.Replace("{button_a}", "<color=green>\uE994</color>");
            str = str.Replace("{button_b}", "<color=red>\uE974</color>");
            str = str.Replace("{button_x}", "<color=blue>\uE997</color>");
            str = str.Replace("{stick_right}", "\uE9B6");
            str = str.Replace("{stick_left}", "\uE9B5");
            str = str.Replace("{dpad}", "\uE9AA");
            return str;
        }

        public static void CreateDefaultFile()
        {
            File.WriteAllLines("strings.txt", DefaultStringsFile);
        }

        private static readonly string[] DefaultStringsFile =
        {
            "# strings.txt",
            "# Use this file to specify all the text used in the experiment",
            "# Lines beginning with '#' will be ignored",
            "",
            "# Tutorial Text",
            "Tutorial_WelcomeText=Welcome!{newline}Let's get familiar with the controls.",
            "Tutorial_AbortText=At any time, hold {button_b} or both triggers to abort the experiment.",
            "Tutorial_LookText=You can use {stick_right} to look around the room.",
            "Tutorial_StressIntroText=You will start with the {animal} in a cage 2m across the rooom.  The cage door will be closed; the {animal} cannot get out.",
            "Tutorial_StressUpDownText=Knowing what you are about to see, move {stick_left} or {dpad} up or down to select your anxiety level on a scale of 0-10 with 0 being no anxiety and 10 being the highest anxiety you have ever experienced.",
            "Tutorial_StressConfirmText=Press {button_a} to confirm your initial anxiety level. You will be asked again for your anxiety level several times throughout the experiment.",
            "Tutorial_NextLevelText=At times, prompts like the one to the right will appear.  When they do, press {button_x} when you are ready to move on to the next level. Press {button_x} to begin.",
            "",
            "# Next level prompt descriptions",
            "NextLevelPanel_MessagePrefix=Press {button_x} to go on when ready.",
            "NextLevelPanel_Beginner1Description=A cage containing the {animal} will appear on a table about 6.5 feet in front of you.",
            "NextLevelPanel_Beginner2Description=The cage will move about two feet closer to you and remain closed.",
            "NextLevelPanel_Beginner3Description=The cage will move directly in front of you and remain closed.",
            "NextLevelPanel_Intermediate1Description=The cage will return to its starting position and the top will open. The {animal} will not leave the cage.",
            "NextLevelPanel_Intermediate2Description=The cage will move about two feet closer to you and remain open.",
            "NextLevelPanel_Intermediate3Description=The cage will move directly in front of you and remain open.",
            "NextLevelPanel_Advanced1Description=The cage will return to its starting position and disappear. The {animal} will be free, but will stay on or above the table.",
            "NextLevelPanel_Advanced2Description=The {animal} will move about two feet closer to you.",
            "NextLevelPanel_Advanced3Description=The {animal} will move directly in front of you.",
            "NextLevelPanel_EndDescription=The experiment will end.",
            "",
            "# Various UI panels",
            "UI_Abort=Hold to abort",
            "UI_StressPanelText=Select Anxiety Level",
            "UI_StressPanelSubmitText={button_a} Submit",
            "UI_WaitForResearcher=Please remove the device and await instructions from the researcher"
        };
    }
}
