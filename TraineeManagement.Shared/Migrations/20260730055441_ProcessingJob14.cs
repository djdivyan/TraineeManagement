using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Shared.Migrations
{
    /// <inheritdoc />
    public partial class ProcessingJob14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "ProcessingJobs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "ProcessingJobs");
        }
    }
}
