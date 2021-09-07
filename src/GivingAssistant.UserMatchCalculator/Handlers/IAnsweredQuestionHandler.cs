using System.Threading.Tasks;
using GivingAssistant.UserMatchCalculator.Models;

namespace GivingAssistant.UserMatchCalculator.Handlers
{
    public interface IAnsweredQuestionHandler
    {
        int ExecutionOrder { get; }
        Task Handle(HandleAnsweredQuestionRequest request);
        bool CanHandle(HandleAnsweredQuestionRequest request);
    }
}