using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketIssue.Infrastructure.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FareModifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ValueType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FareModifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoldProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PassengerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DistanceInKm = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DurationInDays = table.Column<int>(type: "int", nullable: true),
                    BaseFare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalFare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoldProductModifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SoldProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FareModificationId = table.Column<int>(type: "int", nullable: false),
                    AppliedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldProductModifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SoldProductModifications_FareModifications_FareModificationId",
                        column: x => x.FareModificationId,
                        principalTable: "FareModifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SoldProductModifications_SoldProducts_SoldProductId",
                        column: x => x.SoldProductId,
                        principalTable: "SoldProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoldProductModifications_FareModificationId",
                table: "SoldProductModifications",
                column: "FareModificationId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldProductModifications_SoldProductId",
                table: "SoldProductModifications",
                column: "SoldProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoldProductModifications");

            migrationBuilder.DropTable(
                name: "FareModifications");

            migrationBuilder.DropTable(
                name: "SoldProducts");
        }
    }
}
