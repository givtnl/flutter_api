using GivingAssistant.Business.Questions.Models;
using MediatR;

namespace GivingAssistant.Business.Questions.Queries.GetDetail
{
    public class GetQuestionDetailQuery : IRequest<QuestionDetailModel>
    {
        public string Id { get; set; }
    }
}