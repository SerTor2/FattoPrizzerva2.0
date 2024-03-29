﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dialogues
{
    [CustomEditor(typeof(SO_LanguageBasedString))]
    public class Editor_LanguageBasedString : Editor
    {
        string helpMesage;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SO_LanguageBasedString stringObject = target as SO_LanguageBasedString;


            if (stringObject.language is Language.NONE)
            {
                stringObject.Usable = false;

                helpMesage = "The object " + stringObject.name + " does not have a language defined : LANGUAGE = " + stringObject.language;

                EditorGUILayout.HelpBox(helpMesage, MessageType.Warning, true);

            }
            else
            {
                stringObject.Usable = true;
            }


        }


    }
}


