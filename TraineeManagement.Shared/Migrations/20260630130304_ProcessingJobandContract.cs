using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ProcessingJobandContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FileId",
                table: "ProcessingJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubmissionId",
                table: "ProcessingJobs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileId",
                table: "ProcessingJobs");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "ProcessingJobs");
        }
    }
}
