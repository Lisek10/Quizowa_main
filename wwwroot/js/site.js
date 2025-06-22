$(document).ready(function () {
  // --- Funkcje pomocnicze do aktualizacji indeksów ---
  function updateQuestionIndexes() {
    $(".question-block").each(function (
      questionBlockIndex,
      questionBlockElement
    ) {
      var oldQuestionIndex = $(questionBlockElement).data("question-index");
      $(questionBlockElement).data("question-index", questionBlockIndex);
      $(questionBlockElement).attr("data-question-index", questionBlockIndex); // Użyj attr też dla pewności

      // Aktualizacja pól na poziomie pytania
      $(questionBlockElement)
        .find(":input")
        .each(function () {
          var name = $(this).attr("name");
          var id = $(this).attr("id");

          if (name && name.includes(`Questions[${oldQuestionIndex}]`)) {
            $(this).attr(
              "name",
                name.replace(new RegExp(`Questions\\[${oldQuestionIndex}\\]`, 'g'), `Questions[${questionBlockIndex}]`)
            );
          }
          if (id && id.includes(`Questions_${oldQuestionIndex}_`)) {
            $(this).attr(
              "id",
                id.replace(new RegExp(`Questions_${oldQuestionIndex}_`, 'g'), `Questions_${questionBlockIndex}_`)
            );
          }
        });

      // Aktualizacja indeksów odpowiedzi wewnątrz pytania
      $(questionBlockElement)
        .find(".answer-item")
        .each(function (answerItemIndex, answerItemElement) {
          var oldAnswerIndex = $(answerItemElement).data("answer-index");
          $(answerItemElement).data("answer-index", answerItemIndex);
          $(answerItemElement).attr("data-answer-index", answerItemIndex); // Użyj attr też dla pewności

          $(answerItemElement)
            .find(":input")
            .each(function () {
              var name = $(this).attr("name");
              var id = $(this).attr("id");

              // Aktualizuj name dla AnswerText i hidden Id
              if (
                name &&
                name.includes(
                  `Questions[${oldQuestionIndex}].Answers[${oldAnswerIndex}]`
                )
              ) {
                $(this).attr(
                  "name",
                  name.replace(
                    `Questions[${oldQuestionIndex}].Answers[${oldAnswerIndex}]`,
                    `Questions[${questionBlockIndex}].Answers[${answerItemIndex}]`
                  )
                );
              }
              // Aktualizuj ID dla AnswerText
              if (
                id &&
                id.includes(
                  `Questions_${oldQuestionIndex}__Answers_${oldAnswerIndex}__AnswerText`
                )
              ) {
                $(this).attr(
                  "id",
                  `Questions_${questionBlockIndex}__Answers_${answerItemIndex}__AnswerText`
                );
              }

              // Aktualizuj name i ID dla radio buttona CorrectAnswerId
              if ($(this).is('input[type="radio"]')) {
                var radioName = `Questions[${questionBlockIndex}].CorrectAnswerTempId`;
                $(this).attr("name", radioName);
                $(this).attr(
                  "id",
                  `CorrectAnswer_Q${questionBlockIndex}_A${answerItemIndex}`
                ); // Zaktualizuj ID dla radio buttona
              }
            });
        });
    });
  }

  // --- Obsługa zdarzeń (delegacja) ---

  // Przycisk "Dodaj pytanie"
  $("body").on("click", "#add-question-btn", function () {
    var questionIndex = $(".question-block").length; // Nowy indeks pytania
    var url = "/Quiz/AddQuestion";

    $.ajax({
      url: url,
      type: "POST",
      data: { questionIndex: questionIndex },
      success: function (result) {
        $("#questionsContainer").append(result);
        $.validator.unobtrusive.parse($("#questionsContainer")); // Włącz walidację dla nowych elementów
        updateQuestionIndexes(); // Zaktualizuj indeksy po dodaniu
      },
      error: function (xhr, status, error) {
        console.error("Błąd podczas dodawania pytania:", error);
      },
    });
  });

  // Przycisk "Dodaj odpowiedź"
  $("body").on("click", ".add-answer-btn", function (event) {
    // console.log("Kliknięto Dodaj odpowiedź"); // Możesz to odkomentować do debugowania
    event.stopPropagation(); // Powstrzymaj bąbelkowanie zdarzenia

    var questionBlock = $(this).closest(".question-block");
    var questionIndex = questionBlock.data("question-index");
    var answerIndex = questionBlock.find(".answer-item").length; // Nowy indeks odpowiedzi

    var url = "/Quiz/AddAnswer";

    $.ajax({
      url: url,
      type: "POST",
      data: {
        questionIndex: questionIndex,
        answerIndex: answerIndex,
      },
      success: function (result) {
        questionBlock.find(".answers-container").append(result);
        $.validator.unobtrusive.parse(questionBlock); // Włącz walidację dla nowych elementów
        updateQuestionIndexes(); // Aktualizuj indeksy po dodaniu odpowiedzi (dla radio ID)
      },
      error: function (xhr, status, error) {
        console.error("Błąd podczas dodawania odpowiedzi:", error);
      },
    });
  });

  // Przycisk "Usuń pytanie"
  $("body").on("click", ".remove-question-btn", function () {
    if ($(".question-block").length <= 1) {
      // Sprawdź, czy nie próbujesz usunąć ostatniego pytania
      alert("Quiz musi zawierać co najmniej jedno pytanie.");
      return;
    }
    $(this).closest(".question-block").remove();
    updateQuestionIndexes(); // Zawsze aktualizuj indeksy po usunięciu
  });

  // Przycisk "Usuń odpowiedź"
  $("body").on("click", ".remove-answer-btn", function () {
    var questionBlock = $(this).closest(".question-block");
    var currentAnswersCount = questionBlock.find(".answer-item").length;

    // Kluczowa poprawka: Sprawdź liczbę odpowiedzi PRZED usunięciem
    if (currentAnswersCount <= 2) {
      alert("Pytanie musi zawierać co najmniej dwie odpowiedzi.");
      return; // Zablokuj usunięcie
    }

    $(this).closest(".answer-item").remove();
    updateQuestionIndexes(); // Aktualizuj indeksy po usunięciu odpowiedzi
  });

  // Wywołaj updateQuestionIndexes po załadowaniu strony,
  // aby upewnić się, że indeksy są poprawne dla istniejących pytań/odpowiedzi (np. w przypadku edycji)
  updateQuestionIndexes();
});
