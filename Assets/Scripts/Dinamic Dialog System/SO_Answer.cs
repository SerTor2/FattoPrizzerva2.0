﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogues
{
    [CreateAssetMenu(fileName = "Answer", menuName = "Answers/New Answer")]
    public class SO_Answer : ScriptableObject 
    {
        //[SerializeField] private List<LanguageBasedString> answers;
        [SerializeField] private SO_DialogStructure targetStructure;

        [SerializeField] private SO_langaugeBasedStringContainer answer;
        
        public string GetAnswerBody(Language _targetLanguage)
        {
            /*
            if (LanguageBasedString.CheckListIntegrity(answers,_targetLanguage,this.name))
            {
                for (int i = 0; i < answers.Count; i++)
                {
                    if (answers[i].language == _targetLanguage)
                        return answers[i].text;
                }
            }
            return null;
            */

            SO_LanguageBasedString desiredLBS = null;
            desiredLBS = answer.GetLanguageBasedString(_targetLanguage, this.name);
            return desiredLBS.text;

        }

        public SO_DialogStructure GetTargetStructure()
        {
            if (targetStructure)
                return targetStructure;
            else
            {
                Debug.LogError("Target strucuture is not set on " + this.name);
                return null;
            }
                
        }


    }



}

