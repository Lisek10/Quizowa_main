using Microsoft.AspNetCore.Mvc;
using Quizowa.Services; 
using System.Threading.Tasks; 
using System.Security.Claims; 
using System.Linq; 
namespace Quizowa.Controllers
{
    
    public class StatisticsController : Controller
    {
        private readonly QuizStatisticsService _quizStatisticsService;

        
        public StatisticsController(QuizStatisticsService quizStatisticsService)
        {
            _quizStatisticsService = quizStatisticsService;
        }

        
        public async Task<IActionResult> Index()
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            
            var userQuizResults = await _quizStatisticsService.GetUserQuizResults(userId);

            
            return View(userQuizResults);
        }

    
    }
}