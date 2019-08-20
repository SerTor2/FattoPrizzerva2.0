﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogues
{
    public sealed class DC_ImportedQuestionAnswerStructure : I_ImportedDialogStructure
    {
        private SO_DialogObjectCreatorConfig config;

        private string id;

        private string catQuestion;
        private string espQuestion;
        private string engQuestion;

        private List<DC_ImportedAnswerStructure> answers;
        private DC_ImportedAnswerStructure answer;

        public DC_ImportedQuestionAnswerStructure(SO_DialogObjectCreatorConfig configScriptableObject)
        {
            this.config = configScriptableObject;
        }

        public void PrintStoredData()
        {
            throw new System.NotImplementedException();
        }

        public void SetValue(int _indexOnLine, string _fieldData)
        {
            SO_QuestionAnswerStructureCSVImportSettings qaConfig = config.GetQACSVSettings();

            if (!qaConfig)
                throw new MissingReferenceException("SO_QuestionAnswerStructureCSVImportSettings not assigned");

            // question id
            if (_indexOnLine == qaConfig.QuestionIDPosition)
            {
                id = _fieldData;
            }
            // cat question
            else if (_indexOnLine == qaConfig.CatQuestionPosition)
            {
                catQuestion = _fieldData;
            }
            // esp question
            else if (_indexOnLine == qaConfig.EspQuestionPosition)
            {
                espQuestion = _fieldData;
            }
            // eng question;
            else if (_indexOnLine == qaConfig.EngQuestionPosition)
            {
                engQuestion = _fieldData;
            }
            // answers
            else if (_indexOnLine >= qaConfig.AnswerIdPosition)                 // equal or more than the position of the answer id
            {
                if (answer == null) 
                    answer = new DC_ImportedAnswerStructure(qaConfig);

                answer.SetValue(_indexOnLine, _fieldData);

                if (answers == null)
                {
                    answers = new List<DC_ImportedAnswerStructure>();
                    answers.Add(answer);
                }
            }
      
        }
    }

    [System.Serializable]
    public sealed class DC_ImportedAnswerStructure : I_ImportedDialogStructure
    {
        SO_QuestionAnswerStructureCSVImportSettings qaConfig;

        private string id;

        private string catAnswer;
        private string espAnswer;
        private string engAnswer;

        public DC_ImportedAnswerStructure(SO_QuestionAnswerStructureCSVImportSettings configScriptableObject)
        {
            qaConfig = configScriptableObject;
        }

        public string CatAnswer { get => catAnswer; set => catAnswer = value; }
        public string EspAnswer { get => espAnswer; set => espAnswer = value; }
        public string EngAnswer { get => engAnswer; set => engAnswer = value; }
        public string Id { get => id; set => id = value; }

        public void PrintStoredData()
        {
            throw new System.NotImplementedException();
        }

        public void SetValue(int _indexOnLine, string _fieldData)
        {
            // answer id
            if (_indexOnLine == qaConfig.AnswerIdPosition)
            {
                this.Id = _fieldData;
            }
            else if (_indexOnLine == qaConfig.CatAnswerBodyPosition)
            {
                this.CatAnswer = _fieldData;
            }
            else if (_indexOnLine == qaConfig.EspAnswerBodyPosition)
            {
                this.EspAnswer = _fieldData;
            }
            else if (_indexOnLine == qaConfig.EngAnswerBodyPosition)
            {
                this.EngAnswer = _fieldData;
            }
        }
    }

}


