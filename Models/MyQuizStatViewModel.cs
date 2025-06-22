using System.ComponentModel.DataAnnotations;

namespace Quizowa.Models
{
    public class MyQuizStatViewModel
    {
        // Użycie `required` dla właściwości, które zawsze powinny mieć wartość
        public required Quiz Quiz { get; set; }
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
    }
}