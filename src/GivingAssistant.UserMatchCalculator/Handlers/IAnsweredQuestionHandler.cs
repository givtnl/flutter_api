using System.Threading.Tasks;
using GivingAssistant.UserMatchCalculator.Models;

namespace GivingAssistant.UserMatchCalculator.Handlers
{
    public interface IAnsweredQuestionHandler
    {
        Task Handle(HandleAnsweredQuestionRequest request);
        bool CanHandle(HandleAnsweredQuestionRequest request);
    }
}