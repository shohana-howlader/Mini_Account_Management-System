using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mini_Account_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserRoleMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMapping_Roles_RoleId",
                table: "UserRoleMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMapping_Users_UserId",
                table: "UserRoleMapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoleMapping",
                table: "UserRoleMapping");

            migrationBuilder.RenameTable(
                name: "UserRoleMapping",
                newName: "UserRoleMappings");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoleMapping_UserId",
                table: "UserRoleMappings",
                newName: "IX_UserRoleMappings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoleMapping_RoleId",
                table: "UserRoleMappings",
                newName: "IX_UserRoleMappings_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoleMappings",
                table: "UserRoleMappings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleMappings_Roles_RoleId",
                table: "UserRoleMappings",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleMappings_Users_UserId",
                table: "UserRoleMappings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMappings_Roles_RoleId",
                table: "UserRoleMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoleMappings_Users_UserId",
                table: "UserRoleMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoleMappings",
                table: "UserRoleMappings");

            migrationBuilder.RenameTable(
                name: "UserRoleMappings",
                newName: "UserRoleMapping");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoleMappings_UserId",
                table: "UserRoleMapping",
                newName: "IX_UserRoleMapping_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoleMappings_RoleId",
                table: "UserRoleMapping",
                newName: "IX_UserRoleMapping_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoleMapping",
                table: "UserRoleMapping",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleMapping_Roles_RoleId",
                table: "UserRoleMapping",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoleMapping_Users_UserId",
                table: "UserRoleMapping",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
