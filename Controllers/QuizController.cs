using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json; 
using Quizowa.Data;
using Quizowa.Models;
using Quizowa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Quizowa.Controllers
{
    public class QuizController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static int _tempAnswerIdCounter = -1;
        private readonly QuizStatisticsService _quizStatisticsService;

        public QuizController(ApplicationDbContext context, QuizStatisticsService quizStatisticsService)
        {
            _context = context;
            _quizStatisticsService = quizStatisticsService;
        }

        
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Quizzes.Include(q => q.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.ApplicationUser)
                .Include(q => q.Questions!)
                    .ThenInclude(q => q.Answers!)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        
        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var quiz = new Quiz
            {
                Title = "",
                Description = "",
                ApplicationUserId = userId,
                Questions = new List<Question>
                {
                    new Question
                    {
                        QuestionText = "",
                        Points = 1,
                        Answers = new List<Answer>
                        {
                            new Answer { TempId = GetNextTempAnswerId(), AnswerText = "" },
                            new Answer { TempId = GetNextTempAnswerId(), AnswerText = "" }
                        }
                    }
                }
            };
            return View(quiz);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Quiz quiz)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            quiz.ApplicationUserId = userId;

            if (quiz.Questions == null || quiz.Questions.Count == 0)
                ModelState.AddModelError("", "Quiz musi zawierać co najmniej jedno pytanie.");

            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var key = $"Questions[{i}].QuestionText";

                if (ModelState.TryGetValue(key, out var entry) &&
                    entry.ValidationState == ModelValidationState.Invalid &&
                    !string.IsNullOrWhiteSpace(quiz.Questions[i].QuestionText))
                {
                    entry.Errors.Clear();
                    entry.ValidationState = ModelValidationState.Valid;
                }
            }

            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                var q = quiz.Questions[i];

                if (string.IsNullOrWhiteSpace(q.QuestionText))
                    ModelState.AddModelError($"Questions[{i}].QuestionText", "Treść pytania nie może być pusta.");

                if (q.Answers == null || q.Answers.Count < 2)
                    ModelState.AddModelError($"Questions[{i}].Answers", "Każde pytanie musi mieć co najmniej 2 odpowiedzi.");

                for (int j = 0; j < q.Answers.Count; j++)
                {
                    if (string.IsNullOrWhiteSpace(q.Answers[j].AnswerText))
                        ModelState.AddModelError($"Questions[{i}].Answers[{j}].AnswerText", "Treść odpowiedzi nie może być pusta.");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var q in quiz.Questions!)
                {
                    if (q.Answers == null)
                        q.Answers = new List<Answer>();
                }
                return View(quiz);
            }

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            foreach (var q in quiz.Questions!)
            {
                if (q.CorrectAnswerTempId.HasValue)
                {
                    var correctTempId = q.CorrectAnswerTempId.Value;
                    var correctAnswer = q.Answers!.FirstOrDefault(a => a.TempId == correctTempId);

                    if (correctAnswer != null)
                    {
                        q.CorrectAnswerId = correctAnswer.Id;
                        correctAnswer.IsCorrect = true;
                    }
                }

                _context.Questions.Update(q);
                foreach (var a in q.Answers!)
                {
                    _context.Answers.Update(a);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions!)
                    .ThenInclude(q => q.Answers!)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }
            return View(quiz);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Questions,ApplicationUserId")] Quiz quiz)
        {
            if (id != quiz.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(quiz.ApplicationUserId))
            {
                 quiz.ApplicationUserId = userId!;
            }

            if (quiz.Questions == null || !quiz.Questions.Any())
            {
                ModelState.AddModelError("", "Quiz musi zawierać co najmniej jedno pytanie.");
            }
            else
            {
                for (int i = 0; i < quiz.Questions.Count; i++)
                {
                    var question = quiz.Questions[i];
                    if (string.IsNullOrWhiteSpace(question.QuestionText))
                    {
                        ModelState.AddModelError($"Questions[{i}].QuestionText", "Treść pytania nie może być pusta.");
                    }
                    if (question.Answers == null || question.Answers.Count < 2)
                    {
                        ModelState.AddModelError($"Questions[{i}].Answers", "Każde pytanie musi zawierać co najmniej dwie odpowiedzi.");
                    }
                    if (!question.CorrectAnswerId.HasValue || question.CorrectAnswerId.Value == 0)
                    {
                        ModelState.AddModelError($"Questions[{i}].CorrectAnswerId", "Musisz zaznaczyć poprawną odpowiedź dla każdego pytania.");
                    }
                    else
                    {
                        
                        if (!question.Answers!.Any(a => a.Id == question.CorrectAnswerId.Value))
                        {
                             ModelState.AddModelError($"Questions[{i}].CorrectAnswerId", "Wybrana poprawna odpowiedź nie istnieje w tym pytaniu.");
                        }
                    }

                     for (int j = 0; j < question.Answers.Count; j++)
                    {
                        var answer = question.Answers[j];
                        if (string.IsNullOrWhiteSpace(answer.AnswerText))
                        {
                            ModelState.AddModelError($"Questions[{i}].Answers[{j}].AnswerText", "Treść odpowiedzi nie może być pusta.");
                        }
                    }
                }
            }


            if (ModelState.IsValid)
            {
                try
                {
                    var originalQuiz = await _context.Quizzes
                        .Include(q => q.Questions!)
                            .ThenInclude(q => q.Answers!)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(q => q.Id == id);

                    if (originalQuiz == null) return NotFound();

                    _context.Update(quiz);

                    var tempIdMap = new Dictionary<int, Answer>();

                    foreach (var updatedQuestion in quiz.Questions!)
                    {
                        var existingQuestion = originalQuiz.Questions!.FirstOrDefault(q => q.Id == updatedQuestion.Id);

                        if (existingQuestion != null)
                        {
                            _context.Entry(updatedQuestion).State = EntityState.Modified;

                            foreach (var updatedAnswer in updatedQuestion.Answers!)
                            {
                                var existingAnswer = existingQuestion.Answers!.FirstOrDefault(a => a.Id == updatedAnswer.Id);

                                if (existingAnswer != null)
                                {
                                    _context.Entry(updatedAnswer).State = EntityState.Modified;
                                }
                                else
                                {
                                    if (updatedAnswer.Id < 0)
                                    {
                                        tempIdMap[updatedAnswer.Id] = updatedAnswer;
                                        updatedAnswer.Id = 0;
                                    }
                                    updatedAnswer.QuestionId = updatedQuestion.Id;
                                    _context.Answers.Add(updatedAnswer);
                                }
                            }

                            foreach (var oldAnswer in existingQuestion.Answers!.ToList())
                            {
                                if (!updatedQuestion.Answers!.Any(a => a.Id == oldAnswer.Id && a.Id > 0))
                                {
                                    _context.Answers.Remove(oldAnswer);
                                }
                            }
                        }
                        else
                        {
                            updatedQuestion.QuizId = quiz.Id;
                            if (updatedQuestion.Id < 0)
                            {
                                updatedQuestion.Id = 0;
                            }
                            foreach(var newAnswer in updatedQuestion.Answers!)
                            {
                                if(newAnswer.Id < 0)
                                {
                                    tempIdMap[newAnswer.Id] = newAnswer;
                                    newAnswer.Id = 0;
                                }
                            }
                            _context.Questions.Add(updatedQuestion);
                        }
                    }

                    foreach (var oldQuestion in originalQuiz.Questions!.ToList())
                    {
                        if (!quiz.Questions!.Any(q => q.Id == oldQuestion.Id && q.Id > 0))
                        {
                            
                            foreach (var answer in oldQuestion.Answers!)
                            {
                                _context.Answers.Remove(answer);
                            }

                            _context.Questions.Remove(oldQuestion);
                        }
                    }

                    await _context.SaveChangesAsync();

                    foreach (var question in quiz.Questions!)
                    {
                        var originalQuestion = _context.Questions.Local.FirstOrDefault(q => q.Id == question.Id)
                                                ?? await _context.Questions.FirstOrDefaultAsync(q => q.Id == question.Id);

                        if (originalQuestion == null) continue;

                        var selectedAnswerId = question.CorrectAnswerId;

                        if (selectedAnswerId.HasValue)
                        {
                            int finalCorrectAnswerId;
                            if (selectedAnswerId.Value < 0)
                            {
                                if (tempIdMap.TryGetValue(selectedAnswerId.Value, out var actualAnswer))
                                {
                                    finalCorrectAnswerId = actualAnswer!.Id;
                                }
                                else
                                {
                                    finalCorrectAnswerId = 0;
                                }
                            }
                            else
                            {
                                finalCorrectAnswerId = selectedAnswerId.Value;
                            }

                            originalQuestion.CorrectAnswerId = finalCorrectAnswerId;

                            foreach (var ans in originalQuestion.Answers!)
                            {
                                ans.IsCorrect = (ans.Id == finalCorrectAnswerId);
                            }
                        } else {
                            foreach(var ans in originalQuestion.Answers!)
                            {
                                ans.IsCorrect = false;
                            }
                            originalQuestion.CorrectAnswerId = null;
                        }
                        _context.Update(originalQuestion);
                    }
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(_context.Quizzes?.Any(e => e.Id == quiz.Id) ?? false))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            if (string.IsNullOrEmpty(quiz.ApplicationUserId))
            {
                 quiz.ApplicationUserId = userId!;
            }
            if (quiz.Questions == null) quiz.Questions = new List<Question>();
            foreach (var question in quiz.Questions)
            {
                if (question.Answers == null) question.Answers = new List<Answer>();
            }
            return View(quiz);
        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quiz = await _context.Quizzes
                .Include(x => x.QuizResults)
                .Include(q => q.Questions!)
                    .ThenInclude(q => q.Answers!)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                return NotFound();

            foreach (var question in quiz.Questions!)
            {
                question.CorrectAnswerId = null;
                question.CorrectAnswer = null;
            }

            await _context.SaveChangesAsync();

            foreach (var question in quiz.Questions!)
            {
                _context.Answers.RemoveRange(question.Answers!);
                await _context.SaveChangesAsync();
            }

            _context.Questions.RemoveRange(quiz.Questions!);

            var quizResults = await _context.QuizResults.Where(x => x.QuizId == quiz.Id).ToListAsync();
            if (quizResults != null && quizResults.Any())
                _context.QuizResults.RemoveRange(quiz.QuizResults);

            _context.Quizzes.Remove(quiz);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        
        public async Task<IActionResult> Solve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions!)
                    .ThenInclude(q => q.Answers!)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            return View(quiz);
        }

        
        [HttpPost]
        public async Task<IActionResult> Submit(int quizId, IFormCollection form)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var quiz = await _context.Quizzes
                .Include(q => q.Questions!)
                    .ThenInclude(q => q.Answers!)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return Unauthorized();
            }

            int score = 0;
            var userAnswersDictionary = new Dictionary<int, int>(); 

            foreach (var question in quiz.Questions!)
            {
                string fieldName = $"q_{question.Id}";
                if (form.ContainsKey(fieldName))
                {
                    if (int.TryParse(form[fieldName], out int selectedAnswerId))
                    {
                        userAnswersDictionary[question.Id] = selectedAnswerId;
                        if (selectedAnswerId == question.CorrectAnswerId)
                        {
                            score += question.Points;
                        }
                    }
                }
            }

            
            var quizResult = new QuizResult
            {
                QuizId = quiz.Id,
                Quiz = quiz,
                ApplicationUserId = userId,
                ApplicationUser = user,
                Score = score,
                MaxScore = quiz.Questions!.Sum(q => q.Points),
                
                UserAnswersJson = JsonConvert.SerializeObject(userAnswersDictionary), 
                QuizDate = DateTime.UtcNow
            };
            

            _context.QuizResults.Add(quizResult);
            await _context.SaveChangesAsync();

            return View("Result", quizResult);
        }

        
        public IActionResult Result(QuizResult quizResult)
        {
            
            if (quizResult == null || quizResult.Quiz == null || !quizResult.Quiz.Questions.Any()) 
            {
                
                return RedirectToAction(nameof(Index));
            }
            return View(quizResult);
        }

        
        public async Task<IActionResult> UserStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userQuizResults = await _context.QuizResults
                                                .Where(qr => qr.ApplicationUserId == userId)
                                                .Include(qr => qr.Quiz)
                                                .OrderByDescending(qr => qr.QuizDate)
                                                .ToListAsync();

            return View(userQuizResults);
        }

       
        [HttpPost]
        public IActionResult AddQuestion(int questionIndex)
        {
            var newQuestion = new Question
            {
                QuestionText = "",
                Points = 1,
                Answers = new List<Answer>
                {
                    new Answer { Id = GetNextTempAnswerId(), AnswerText = "" },
                    new Answer { Id = GetNextTempAnswerId(), AnswerText = "" }
                }
            };
            ViewData["QuestionIndex"] = questionIndex;
            return PartialView("_QuestionFormPartial", newQuestion);
        }

        
        [HttpPost]
        public IActionResult AddAnswer(int questionIndex, int answerIndex)
        {
            var answer = new Answer
            {
                TempId = GetNextTempAnswerId(),
                AnswerText = ""
            };
            ViewData["QuestionIndex"] = questionIndex;
            ViewData["AnswerIndex"] = answerIndex;
            return PartialView("_AnswerFormPartial", answer);
        }

        private bool QuizExists(int id)
        {
            return _context.Quizzes.Any(e => e.Id == id);
        }

        private int GetNextTempAnswerId()
        {
            return Interlocked.Decrement(ref _tempAnswerIdCounter);
        }

    }
}