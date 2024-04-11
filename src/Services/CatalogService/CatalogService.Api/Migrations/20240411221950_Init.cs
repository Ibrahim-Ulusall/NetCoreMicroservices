using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CatalogService.Api.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "CatalogService");

            migrationBuilder.CreateTable(
                name: "CatalogBrands",
                schema: "CatalogService",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    brand = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogBrands", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogTypes",
                schema: "CatalogService",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    type = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogTypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogItems",
                schema: "CatalogService",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    picture_file_name = table.Column<string>(type: "text", nullable: true),
                    picture_uri = table.Column<string>(type: "text", nullable: true),
                    catalog_type_id = table.Column<int>(type: "integer", nullable: false),
                    catalog_brand_id = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogBrands_catalog_brand_id",
                        column: x => x.catalog_brand_id,
                        principalSchema: "CatalogService",
                        principalTable: "CatalogBrands",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItems_CatalogTypes_catalog_type_id",
                        column: x => x.catalog_type_id,
                        principalSchema: "CatalogService",
                        principalTable: "CatalogTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_catalog_brand_id",
                schema: "CatalogService",
                table: "CatalogItems",
                column: "catalog_brand_id");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_catalog_type_id",
                schema: "CatalogService",
                table: "CatalogItems",
                column: "catalog_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItems",
                schema: "CatalogService");

            migrationBuilder.DropTable(
                name: "CatalogBrands",
                schema: "CatalogService");

            migrationBuilder.DropTable(
                name: "CatalogTypes",
                schema: "CatalogService");
        }
    }
}
