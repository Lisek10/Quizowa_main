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

        
        public int QuizId { get; set; }
        [Required] 
        public required Quiz Quiz { get; set; } 
        [Required]
        public required string ApplicationUserId { get; set; } 
        [Required] 
        public required ApplicationUser ApplicationUser { get; set; } 

        public int Score { get; set; }
        public int MaxScore { get; set; }

        public DateTime QuizDate { get; set; } = DateTime.UtcNow;

        
        [Required] 
        public required string UserAnswersJson { get; set; }

        
        [NotMapped]
        public Dictionary<int, int> UserAnswers
        {
            get => string.IsNullOrEmpty(UserAnswersJson) ? new Dictionary<int, int>() : JsonConvert.DeserializeObject<Dictionary<int, int>>(UserAnswersJson)!; 
            set => UserAnswersJson = JsonConvert.SerializeObject(value);
        }
    }
}