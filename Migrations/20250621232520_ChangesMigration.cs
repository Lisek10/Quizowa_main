using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizowa.Migrations
{
    /// <inheritdoc />
    public partial class ChangesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Answers_CorrectAnswerId1",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_Quizzes_QuizId",
                table: "QuizResults");

            migrationBuilder.DropTable(
                name: "QuizQuestionAnswer");

            migrationBuilder.DropTable(
                name: "QuizAttempt");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CorrectAnswerId1",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CorrectAnswerId1",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "DateCompleted",
                table: "QuizResults",
                newName: "QuizDate");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "QuizResults",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuizId1",
                table: "QuizResults",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "Answers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_QuizId1",
                table: "QuizResults",
                column: "QuizId1");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CorrectAnswerId",
                table: "Questions",
                column: "CorrectAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Answers_CorrectAnswerId",
                table: "Questions",
                column: "CorrectAnswerId",
                principalTable: "Answers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_Quizzes_QuizId",
                table: "QuizResults",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_Quizzes_QuizId1",
                table: "QuizResults",
                column: "QuizId1",
                principalTable: "Quizzes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Answers_CorrectAnswerId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_Quizzes_QuizId",
                table: "QuizResults");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_Quizzes_QuizId1",
                table: "QuizResults");

            migrationBuilder.DropIndex(
                name: "IX_QuizResults_QuizId1",
                table: "QuizResults");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CorrectAnswerId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "QuizId1",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "QuizDate",
                table: "QuizResults",
                newName: "DateCompleted");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswerId1",
                table: "Questions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuizAttempt",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationUserId = table.Column<string>(type: "TEXT", nullable: false),
                    QuizId = table.Column<int>(type: "INTEGER", nullable: false),
                    AttemptDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAttempt_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizAttempt_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestionAnswer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnswerId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuizAttemptId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestionAnswer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestionAnswer_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizQuestionAnswer_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizQuestionAnswer_QuizAttempt_QuizAttemptId",
                        column: x => x.QuizAttemptId,
                        principalTable: "QuizAttempt",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CorrectAnswerId1",
                table: "Questions",
                column: "CorrectAnswerId1");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempt_ApplicationUserId",
                table: "QuizAttempt",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempt_QuizId",
                table: "QuizAttempt",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswer_AnswerId",
                table: "QuizQuestionAnswer",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswer_QuestionId",
                table: "QuizQuestionAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestionAnswer_QuizAttemptId",
                table: "QuizQuestionAnswer",
                column: "QuizAttemptId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Answers_CorrectAnswerId1",
                table: "Questions",
                column: "CorrectAnswerId1",
                principalTable: "Answers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_Quizzes_QuizId",
                table: "QuizResults",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
