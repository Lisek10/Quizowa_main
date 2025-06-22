using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizowa.Models
{
    public class Question
    {
        public int Id { get; set; }

        [NotMapped]
        public int? CorrectAnswerTempId { get; set; }

        public int QuizId { get; set; }
        [ForeignKey("QuizId")]
        public Quiz? Quiz { get; set; }

        [Required(ErrorMessage = "Treść pytania jest wymagana.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Treść pytania musi mieć od 5 do 500 znaków.")]
        public required string QuestionText { get; set; }

        [Range(1, 100, ErrorMessage = "Punkty muszą być od 1 do 100.")]
        public int Points { get; set; }

        // CorrectAnswerId jest używane do wskazywania, która Answer jest poprawna.
        public int? CorrectAnswerId { get; set; }
        [ForeignKey("CorrectAnswerId")]
        public Answer? CorrectAnswer { get; set; }

        // --- USUŃ TĘ LINIĘ, JEŚLI JĄ MASZ ---
        // [NotMapped]
        // public string? CorrectAnswerTempId { get; set; }
        // --- KONIEC USUWANIA LINI ---

        public List<Answer> Answers { get; set; } = new List<Answer>();
    }
}