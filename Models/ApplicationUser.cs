using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Quizowa.Models
{
    
    public class ApplicationUser : IdentityUser
    {
        
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

        
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();

    }
}