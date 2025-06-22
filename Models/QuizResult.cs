using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Quizowa.Models
{
    public class QuizResult
    {
        public int Id { get; set; }

        // Klucz obcy do quizu, którego dotyczy wynik
        public int QuizId { get; set; }
        [Required] // Quiz jest wymagany
        public required Quiz Quiz { get; set; } // Dodaj 'required'

        // Klucz obcy do użytkownika, który rozwiązał quiz
        [Required]
        public required string ApplicationUserId { get; set; } // Dodaj 'required'
        [Required] // ApplicationUser jest wymagany
        public required ApplicationUser ApplicationUser { get; set; } // Dodaj 'required'

        public int Score { get; set; }
        public int MaxScore { get; set; }

        public DateTime QuizDate { get; set; } = DateTime.UtcNow;

        // JSON string przechowujący odpowiedzi użytkownika (QuestionId, SelectedAnswerId)
        [Required] // Ta właściwość też musi być wymagana, bo jest źródłem UserAnswers
        public required string UserAnswersJson { get; set; }

        // Właściwość NotMapped do łatwego dostępu do odpowiedzi w postaci słownika
        [NotMapped]
        public Dictionary<int, int> UserAnswers
        {
            get => string.IsNullOrEmpty(UserAnswersJson) ? new Dictionary<int, int>() : JsonConvert.DeserializeObject<Dictionary<int, int>>(UserAnswersJson)!; // Użyj '!' tutaj
            set => UserAnswersJson = JsonConvert.SerializeObject(value);
        }
    }
}