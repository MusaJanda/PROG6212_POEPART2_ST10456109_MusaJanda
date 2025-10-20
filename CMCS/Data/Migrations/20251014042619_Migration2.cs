using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS.Data.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicManagers_AspNetUsers_UserId",
                table: "AcademicManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_Claims_AcademicManagers_ApprovedByManagerId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Claims_ProgrammeCoordinators_ApprovedByCoordinatorId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgrammeCoordinators_AspNetUsers_UserId",
                table: "ProgrammeCoordinators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProgrammeCoordinators",
                table: "ProgrammeCoordinators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicManagers",
                table: "AcademicManagers");

            migrationBuilder.RenameTable(
                name: "ProgrammeCoordinators",
                newName: "ProgrammeCoordinator");

            migrationBuilder.RenameTable(
                name: "AcademicManagers",
                newName: "AcademicManager");

            migrationBuilder.RenameIndex(
                name: "IX_ProgrammeCoordinators_UserId",
                table: "ProgrammeCoordinator",
                newName: "IX_ProgrammeCoordinator_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicManagers_UserId",
                table: "AcademicManager",
                newName: "IX_AcademicManager_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProgrammeCoordinator",
                table: "ProgrammeCoordinator",
                column: "CoordinatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicManager",
                table: "AcademicManager",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicManager_AspNetUsers_UserId",
                table: "AcademicManager",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_AcademicManager_ApprovedByManagerId",
                table: "Claims",
                column: "ApprovedByManagerId",
                principalTable: "AcademicManager",
                principalColumn: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_ProgrammeCoordinator_ApprovedByCoordinatorId",
                table: "Claims",
                column: "ApprovedByCoordinatorId",
                principalTable: "ProgrammeCoordinator",
                principalColumn: "CoordinatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgrammeCoordinator_AspNetUsers_UserId",
                table: "ProgrammeCoordinator",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicManager_AspNetUsers_UserId",
                table: "AcademicManager");

            migrationBuilder.DropForeignKey(
                name: "FK_Claims_AcademicManager_ApprovedByManagerId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_Claims_ProgrammeCoordinator_ApprovedByCoordinatorId",
                table: "Claims");

            migrationBuilder.DropForeignKey(
                name: "FK_ProgrammeCoordinator_AspNetUsers_UserId",
                table: "ProgrammeCoordinator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProgrammeCoordinator",
                table: "ProgrammeCoordinator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicManager",
                table: "AcademicManager");

            migrationBuilder.RenameTable(
                name: "ProgrammeCoordinator",
                newName: "ProgrammeCoordinators");

            migrationBuilder.RenameTable(
                name: "AcademicManager",
                newName: "AcademicManagers");

            migrationBuilder.RenameIndex(
                name: "IX_ProgrammeCoordinator_UserId",
                table: "ProgrammeCoordinators",
                newName: "IX_ProgrammeCoordinators_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicManager_UserId",
                table: "AcademicManagers",
                newName: "IX_AcademicManagers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProgrammeCoordinators",
                table: "ProgrammeCoordinators",
                column: "CoordinatorId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_ProgrammeCoordinators_ApprovedByCoordinatorId",
                table: "Claims",
                column: "ApprovedByCoordinatorId",
                principalTable: "ProgrammeCoordinators",
                principalColumn: "CoordinatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgrammeCoordinators_AspNetUsers_UserId",
                table: "ProgrammeCoordinators",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
