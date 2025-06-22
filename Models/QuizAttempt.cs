using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizowa.Models
{
    public class QuizAttempt
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }

        [Required]
        [ForeignKey("Quiz")]
        public int QuizId { get; set; }
        public Quiz? Quiz { get; set; }

        public int Score { get; set; }
        public DateTime AttemptDate { get; set; } = DateTime.Now;

        public ICollection<QuizQuestionAnswer> SelectedAnswers { get; set; }

        public QuizAttempt()
        {
            SelectedAnswers = new List<QuizQuestionAnswer>();
        }
    }
}