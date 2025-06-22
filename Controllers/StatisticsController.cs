using Microsoft.AspNetCore.Mvc;
using Quizowa.Services; // Potrzebne do wstrzykiwania serwisu
using System.Threading.Tasks; // Potrzebne dla async/await
using System.Security.Claims; // Potrzebne do pobierania UserId
using System.Linq; // Potrzebne dla metod LINQ, np. Any()

namespace Quizowa.Controllers
{
    // Kontroler obsługujący logikę związaną ze statystykami użytkowników
    public class StatisticsController : Controller
    {
        private readonly QuizStatisticsService _quizStatisticsService;

        // Konstruktor kontrolera, wstrzykuje QuizStatisticsService
        public StatisticsController(QuizStatisticsService quizStatisticsService)
        {
            _quizStatisticsService = quizStatisticsService;
        }

        // Akcja Index wyświetlająca statystyki dla zalogowanego użytkownika
        public async Task<IActionResult> Index()
        {
            // Pobierz ID aktualnie zalogowanego użytkownika
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Jeśli użytkownik nie jest zalogowany, przekieruj go na stronę logowania
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Pobierz wyniki quizów dla danego użytkownika za pomocą serwisu statystyk
            var userQuizResults = await _quizStatisticsService.GetUserQuizResults(userId);

            // Przekaż listę wyników quizów do widoku
            return View(userQuizResults);
        }

    
    }
}