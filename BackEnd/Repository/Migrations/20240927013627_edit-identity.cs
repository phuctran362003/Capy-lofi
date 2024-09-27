using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class editidentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserMusics",
                schema: "identity",
                newName: "UserMusics");

            migrationBuilder.RenameTable(
                name: "UserChatRooms",
                schema: "identity",
                newName: "UserChatRooms");

            migrationBuilder.RenameTable(
                name: "UserBackgrounds",
                schema: "identity",
                newName: "UserBackgrounds");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "identity",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "Musics",
                schema: "identity",
                newName: "Musics");

            migrationBuilder.RenameTable(
                name: "Messages",
                schema: "identity",
                newName: "Messages");

            migrationBuilder.RenameTable(
                name: "LearningSessions",
                schema: "identity",
                newName: "LearningSessions");

            migrationBuilder.RenameTable(
                name: "Feedbacks",
                schema: "identity",
                newName: "Feedbacks");

            migrationBuilder.RenameTable(
                name: "ChatRooms",
                schema: "identity",
                newName: "ChatRooms");

            migrationBuilder.RenameTable(
                name: "ChatInvitations",
                schema: "identity",
                newName: "ChatInvitations");

            migrationBuilder.RenameTable(
                name: "Backgrounds",
                schema: "identity",
                newName: "Backgrounds");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "identity",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "identity",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "identity",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "identity",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "identity",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "identity",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "identity",
                newName: "AspNetRoleClaims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.RenameTable(
                name: "UserMusics",
                newName: "UserMusics",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "UserChatRooms",
                newName: "UserChatRooms",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "UserBackgrounds",
                newName: "UserBackgrounds",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Orders",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Musics",
                newName: "Musics",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "Messages",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "LearningSessions",
                newName: "LearningSessions",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Feedbacks",
                newName: "Feedbacks",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "ChatRooms",
                newName: "ChatRooms",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "ChatInvitations",
                newName: "ChatInvitations",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "Backgrounds",
                newName: "Backgrounds",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                newName: "AspNetUserTokens",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "AspNetUsers",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                newName: "AspNetUserRoles",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                newName: "AspNetUserLogins",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                newName: "AspNetUserClaims",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                newName: "AspNetRoles",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                newName: "AspNetRoleClaims",
                newSchema: "identity");
        }
    }
}
