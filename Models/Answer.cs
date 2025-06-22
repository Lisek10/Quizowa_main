// W Quizowa/Models/Answer.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizowa.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [NotMapped]
        public int TempId { get; set; }

        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        [Required(ErrorMessage = "Treść odpowiedzi jest wymagana.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Treść odpowiedzi musi mieć od 1 do 200 znaków.")]
        public required string AnswerText { get; set; }

        // --- DODAJ TĘ LINIĘ, JEŚLI JEJ NIE MASZ ---
        public bool IsCorrect { get; set; } // Zwracamy IsCorrect do modelu Answer
        // --- KONIEC DODAWANIA LINI ---
    }
}