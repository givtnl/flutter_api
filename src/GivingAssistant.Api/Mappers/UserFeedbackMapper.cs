using AutoMapper;
using GivingAssistant.Api.Requests;
using GivingAssistant.Business.Feedback;
using MediatR;

namespace GivingAssistant.Api.Mappers
{
    public class UserFeedbackMapper: Profile
    {
        public UserFeedbackMapper()
        {
            CreateMap<CreateUserFeedbackRequest, CreateUserFeedbackCommand>();
            CreateMap<Unit, CreateUserFeedbackResponse>();
        }
    }
}