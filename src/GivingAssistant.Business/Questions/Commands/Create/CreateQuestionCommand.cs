using System.Collections.Generic;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Domain;
using MediatR;

namespace GivingAssistant.Business.Questions.Commands.Create
{
    public class CreateQuestionCommand : IRequest<QuestionDetailModel>
    {
        public int DisplayOrder { get; set; }
        public QuestionType Type { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public Dictionary<string, int> TagScores { get; set; }

   
    }
}