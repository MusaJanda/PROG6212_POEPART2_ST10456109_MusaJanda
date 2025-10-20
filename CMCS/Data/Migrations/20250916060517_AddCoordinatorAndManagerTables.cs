using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordinatorAndManagerTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Managers_ApprovedByManagerId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Managers_AspNetUsers_UserId",
                table: "Managers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Managers",
                table: "Managers");

            migrationBuilder.RenameTable(
                name: "Managers",
                newName: "AcademicManagers");

            migrationBuilder.RenameIndex(
                name: "IX_Managers_UserId",
                table: "AcademicManagers",
                newName: "IX_AcademicManagers_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ProgrammeCoordinators",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AcademicManagers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicManagers",
                table: "AcademicManagers",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicManagers_AspNetUsers_UserId",
                table: "AcademicManagers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_AcademicManagers_ApprovedByManagerId",
                table: "Claims",
                column: "ApprovedByManagerId",
                principalTable: "AcademicManagers",
                principalColumn: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicManagers_AspNetUsers_UserId",
                table: "AcademicManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_Claims_AcademicManagers_ApprovedByManagerId",
                table: "Claims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicManagers",
                table: "AcademicManagers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ProgrammeCoordinators");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AcademicManagers");

            migrationBuilder.RenameTable(
                name: "AcademicManagers",
                newName: "Managers");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicManagers_UserId",
                table: "Managers",
                newName: "IX_Managers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Managers",
                table: "Managers",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Managers_ApprovedByManagerId",
                table: "Claims",
                column: "ApprovedByManagerId",
                principalTable: "Managers",
                principalColumn: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_AspNetUsers_UserId",
                table: "Managers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
