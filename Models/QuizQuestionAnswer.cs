using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizowa.Models
{
    public class QuizQuestionAnswer
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("QuizAttempt")]
        public int QuizAttemptId { get; set; }
        public QuizAttempt? QuizAttempt { get; set; }

        [Required]
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        [Required]
        [ForeignKey("Answer")]
        public int AnswerId { get; set; }
        public Answer? Answer { get; set; }
    }
}