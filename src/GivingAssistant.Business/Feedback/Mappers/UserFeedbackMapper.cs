using AutoMapper;
using GivingAssistant.Business.Infrastructure;
using GivingAssistant.Domain;

namespace GivingAssistant.Business.Feedback.Mappers
{
    public class UserFeedbackMapper: Profile
    {
        public UserFeedbackMapper()
        {
            CreateMap<CreateUserFeedbackCommand, Persistence.UserFeedback>()
                .ForMember(x => x.PrimaryKey, x => x.MapFrom(y => $"{Constants.FeedbackPlaceholder}"))
                .ForMember(x => x.SortKey, x => x.MapFrom(y => $"{Constants.UserPlaceholder}#{y.UserId}"))
                .ForMember(x => x.Feedback, x => x.MapFrom(y => y.UserFeedback));
        }
    }
}