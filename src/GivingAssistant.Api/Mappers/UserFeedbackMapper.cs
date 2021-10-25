using AutoMapper;
using GivingAssistant.Api.Requests;
using GivingAssistant.Business.Feedback;

namespace GivingAssistant.Api.Mappers
{
    public class UserFeedbackMapper: Profile
    {
        public UserFeedbackMapper()
        {
            CreateMap<CreateUserFeedbackRequest, CreateUserFeedbackCommand>();
        }
    }
}