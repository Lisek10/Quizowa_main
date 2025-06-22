using Microsoft.EntityFrameworkCore;
using Quizowa.Data;
using Quizowa.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Quizowa.Services
{
    public class QuizStatisticsService
    {
        private readonly ApplicationDbContext _context;

        public QuizStatisticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<QuizResult>> GetUserQuizResults(string userId)
        {
            // Pamiętaj, że QuizResult.Quiz i QuizResult.ApplicationUser są teraz 'required'
            // Więc EF Core powinien je załadować, jeśli są dołączone.
            return await _context.QuizResults
                                 .Where(qr => qr.ApplicationUserId == userId)
                                 .Include(qr => qr.Quiz) // Nadal dołączamy Quiz
                                 .OrderByDescending(qr => qr.QuizDate)
                                 .ToListAsync();
        }

    
    }
}