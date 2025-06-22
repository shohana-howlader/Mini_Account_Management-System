using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mini_Account_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class next : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRoleMappings_UserId",
                table: "UserRoleMappings");

            migrationBuilder.AddColumn<int>(
                name: "RoleId1",
                table: "UserRoleMappings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "UserRoleMappings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMapping_UserId_RoleId",
                table: "UserRoleMappings",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappings_RoleId1",
                table: "UserRoleMappings",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappings_UserId1",
                table: "UserRoleMappings",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleMappings_Roles_RoleId1",
                table: "UserRoleMappings",
                column: "RoleId1",
                principalTable: "Roles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleMappings_Users_UserId1",
                table: "UserRoleMappings",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMappings_Roles_RoleId1",
                table: "UserRoleMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMappings_Users_UserId1",
                table: "UserRoleMappings");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleMapping_UserId_RoleId",
                table: "UserRoleMappings");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleMappings_RoleId1",
                table: "UserRoleMappings");

            migrationBuilder.DropIndex(
                name: "IX_UserRoleMappings_UserId1",
                table: "UserRoleMappings");

            migrationBuilder.DropColumn(
                name: "RoleId1",
                table: "UserRoleMappings");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserRoleMappings");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleMappings_UserId",
                table: "UserRoleMappings",
                column: "UserId");
        }
    }
}
