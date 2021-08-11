using System.Collections.Generic;
using GivingAssistant.Business.Questions.Models;
using MediatR;

namespace GivingAssistant.Business.Questions.Queries.GetList
{
    public class GetQuestionsListQuery : IRequest<IEnumerable<QuestionListModel>>
    {
        
    }
}