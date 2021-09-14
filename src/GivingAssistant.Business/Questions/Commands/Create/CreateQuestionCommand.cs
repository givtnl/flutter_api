using System.Collections.Generic;
using GivingAssistant.Business.Questions.Models;
using GivingAssistant.Domain;
using MediatR;

namespace GivingAssistant.Business.Questions.Commands.Create
{
    public class CreateQuestionCommand : IRequest<QuestionDetailModel>
    {
        public CreateQuestionCommand()
        {
            CategoryOptions = new List<CreateQuestionCategoryCommandOptions>();
        }
        public int DisplayOrder { get; set; }
        public QuestionType Type { get; set; }
        public Dictionary<string, string> Translations { get; set; }
        public List<CreateQuestionCategoryCommandOptions> CategoryOptions { get; set; }
        public CreateQuestionStatementCommandOptions StatementOptions { get; set; }
        public Dictionary<string, string> MetaTags { get; set; }
    }
}