using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Misce.WalletManager.DAL.Migrations
{
    public partial class Transaction_Category_Type_Refactoring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "TransactionCategories");

            migrationBuilder.AddColumn<bool>(
                name: "IsExpenseCategory",
                table: "TransactionCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpenseCategory",
                table: "TransactionCategories");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "TransactionCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
