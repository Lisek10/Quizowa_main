using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Quizowa.Models
{
    // Odziedzicz po IdentityUser, aby mieć wszystkie właściwości użytkownika systemu Identity
    public class ApplicationUser : IdentityUser
    {
        // Dodaj kolekcję quizów stworzonych przez użytkownika
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();

        // Dodaj kolekcję wyników quizów użytkownika
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();

    }
}