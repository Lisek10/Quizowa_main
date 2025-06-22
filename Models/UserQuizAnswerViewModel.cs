namespace Quizowa.Models
{
    // ViewModel używany do zbierania odpowiedzi użytkownika z formularza "Wypełnij quiz"
    public class UserQuizAnswerViewModel
    {
        public int QuestionId { get; set; }
        public int SelectedAnswerId { get; set; }
    }
}