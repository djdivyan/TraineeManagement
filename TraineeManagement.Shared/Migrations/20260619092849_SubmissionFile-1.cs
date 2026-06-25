using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeManagement.Shared.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionFile1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "SubmissionFiles",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Size",
                table: "SubmissionFiles",
                type: "tinyint unsigned",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
