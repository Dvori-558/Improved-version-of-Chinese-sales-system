using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChineseSaleApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ImageUrl is already included in InitialCreate, no changes needed
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No changes needed
        }
    }
}
