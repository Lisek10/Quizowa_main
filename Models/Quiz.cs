// W Quizowa/Models/Quiz.cs

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizowa.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł quizu jest wymagany.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tytuł musi mieć od 3 do 200 znaków.")]
        public required string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public required string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        public List<Question> Questions { get; set; } = new List<Question>();

        // --- DODAJ TĘ LINIĘ, JEŚLI JEJ NIE MASZ ---
        // Dodajemy kolekcję QuizResults do modelu Quiz
        public ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
        // --- KONIEC DODAWANIA LINI ---
    }
}