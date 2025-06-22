using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Quizowa.Models;
using System.Reflection.Emit; 

namespace Quizowa.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Quiz>()
                .HasOne(q => q.ApplicationUser)
                .WithMany(u => u.Quizzes) 
                .HasForeignKey(q => q.ApplicationUserId);

            
            builder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId);

            builder.Entity<Question>()
               .HasOne(q => q.CorrectAnswer)
               .WithMany()                   
               .HasForeignKey(q => q.CorrectAnswerId)
               .OnDelete(DeleteBehavior.SetNull);

           
            builder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId);

            
            builder.Entity<QuizResult>()
                .HasOne(qr => qr.Quiz)
                .WithMany()
                .HasForeignKey(qr => qr.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<QuizResult>()
                .HasOne(qr => qr.ApplicationUser)
                .WithMany(u => u.QuizResults) 
                .HasForeignKey(qr => qr.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<QuizResult>()
                .Ignore(qr => qr.UserAnswers);
        }
    }
}