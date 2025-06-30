using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mini_Account_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ////migrationBuilder.CreateTable(
            ////    name: "Roles",
            ////    columns: table => new
            ////    {
            ////        Id = table.Column<int>(type: "int", nullable: false)
            ////            .Annotation("SqlServer:Identity", "1, 1"),
            ////        RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            ////        Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            ////        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            ////    },
            ////    constraints: table =>
            ////    {
            ////        table.PrimaryKey("PK_Roles", x => x.Id);
            ////    });

            //migrationBuilder.CreateTable(
            //    name: "Screens",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ScreenName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        URL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Screens", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserRolePermission",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        RoleId = table.Column<int>(type: "int", nullable: false),
            //        ScreenId = table.Column<int>(type: "int", nullable: false),
            //        CanRead = table.Column<bool>(type: "bit", nullable: false),
            //        CanWrite = table.Column<bool>(type: "bit", nullable: false),
            //        CanEdit = table.Column<bool>(type: "bit", nullable: false),
            //        CanDelete = table.Column<bool>(type: "bit", nullable: false),
            //        UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ScreenName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        URL = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //        Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserRoleMappings",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        RoleId = table.Column<int>(type: "int", nullable: false),
            //        UserId1 = table.Column<int>(type: "int", nullable: true),
            //        RoleId1 = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserRoleMappings", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserRoleMappings_Roles_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "Roles",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_UserRoleMappings_Roles_RoleId1",
            //            column: x => x.RoleId1,
            //            principalTable: "Roles",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_UserRoleMappings_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_UserRoleMappings_Users_UserId1",
            //            column: x => x.UserId1,
            //            principalTable: "Users",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "UserRolePermissions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: true),
            //        RoleId = table.Column<int>(type: "int", nullable: true),
            //        ScreenId = table.Column<int>(type: "int", nullable: false),
            //        CanRead = table.Column<bool>(type: "bit", nullable: false),
            //        CanWrite = table.Column<bool>(type: "bit", nullable: false),
            //        CanEdit = table.Column<bool>(type: "bit", nullable: false),
            //        CanDelete = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_UserRolePermissions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_UserRolePermissions_Roles_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "Roles",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_UserRolePermissions_Screens_ScreenId",
            //            column: x => x.ScreenId,
            //            principalTable: "Screens",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_UserRolePermissions_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "Id");
            //    });

            //migrationBuilder.InsertData(
            //    table: "Roles",
            //    columns: new[] { "Id", "CreatedDate", "Description", "RoleName" },
            //    values: new object[,]
            //    {
            //        { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Admin" },
            //        { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Accountant" },
            //        { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "User" }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Screens",
            //    columns: new[] { "Id", "ScreenName", "URL" },
            //    values: new object[,]
            //    {
            //        { 1, "Dashboard", "/" },
            //        { 2, "Payment Voucher", "/Vouchers/Payment" },
            //        { 3, "Receipt Voucher", "/Vouchers/Receipt" },
            //        { 4, "Journal Entries", "/Journal" },
            //        { 5, "Chart of Accounts", "/Accounts" },
            //        { 6, "User Management", "/Users" }
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRoleMapping_UserId_RoleId",
            //    table: "UserRoleMappings",
            //    columns: new[] { "UserId", "RoleId" },
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRoleMappings_RoleId",
            //    table: "UserRoleMappings",
            //    column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRoleMappings_RoleId1",
            //    table: "UserRoleMappings",
            //    column: "RoleId1");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRoleMappings_UserId1",
            //    table: "UserRoleMappings",
            //    column: "UserId1");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRolePermissions_RoleId",
            //    table: "UserRolePermissions",
            //    column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRolePermissions_ScreenId",
            //    table: "UserRolePermissions",
            //    column: "ScreenId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserRolePermissions_UserId",
            //    table: "UserRolePermissions",
            //    column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "UserRoleMappings");

            //migrationBuilder.DropTable(
            //    name: "UserRolePermission");

            //migrationBuilder.DropTable(
            //    name: "UserRolePermissions");

            //migrationBuilder.DropTable(
            //    name: "Roles");

            //migrationBuilder.DropTable(
            //    name: "Screens");

            //migrationBuilder.DropTable(
            //    name: "Users");
        }
    }
}
