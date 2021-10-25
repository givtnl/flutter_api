using AutoMapper;

namespace GivingAssistant.Business.Feedback.Mappers
{
    public class UserFeedbackMapper: Profile
    {
        public UserFeedbackMapper()
        {
            CreateMap<CreateUserFeedbackCommand, Persistence.UserFeedback>()
                .ForMember(x => x.Feedback, x => x.MapFrom(y => y.UserFeedback));
        }
    }
}