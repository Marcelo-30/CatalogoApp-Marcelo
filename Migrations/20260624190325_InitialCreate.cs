using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatalogoRopaMVC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Colores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CodigoHex = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tallas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tallas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    TallaId = table.Column<int>(type: "int", nullable: false),
                    ColorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Colores_ColorId",
                        column: x => x.ColorId,
                        principalTable: "Colores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Productos_Tallas_TallaId",
                        column: x => x.TallaId,
                        principalTable: "Tallas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ImagenesProducto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TextoAlternativo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    EsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    ProductoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagenesProducto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagenesProducto_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Playeras casuales y básicas.", "Playeras" },
                    { 2, "Jeans, joggers y pantalones casuales.", "Pantalones" },
                    { 3, "Prendas para frío y outfits urbanos.", "Chamarras" },
                    { 4, "Gorras, bolsas y complementos.", "Accesorios" }
                });

            migrationBuilder.InsertData(
                table: "Colores",
                columns: new[] { "Id", "CodigoHex", "Nombre" },
                values: new object[,]
                {
                    { 1, "#FFFFFF", "Blanco" },
                    { 2, "#1F5AA6", "Azul" },
                    { 3, "#111111", "Negro" },
                    { 4, "#7A4F36", "Café" }
                });

            migrationBuilder.InsertData(
                table: "Tallas",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "XS" },
                    { 2, "S" },
                    { 3, "M" },
                    { 4, "L" },
                    { 5, "XL" },
                    { 6, "32" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "CategoriaId", "ColorId", "Descripcion", "Disponible", "FechaRegistro", "Nombre", "Precio", "Stock", "TallaId" },
                values: new object[,]
                {
                    { 1, 1, 1, "Playera de algodón para uso diario.", true, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Playera básica blanca", 199.00m, 12, 3 },
                    { 2, 2, 2, "Pantalón de mezclilla para outfit casual.", true, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jeans azul clásico", 549.00m, 8, 6 },
                    { 3, 3, 3, "Chamarra ligera para clima fresco.", true, new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chamarra negra urbana", 899.00m, 5, 4 }
                });

            migrationBuilder.InsertData(
                table: "ImagenesProducto",
                columns: new[] { "Id", "EsPrincipal", "ProductoId", "TextoAlternativo", "Url" },
                values: new object[,]
                {
                    { 1, true, 1, "Playera básica blanca", "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab" },
                    { 2, true, 2, "Jeans azul clásico", "https://images.unsplash.com/photo-1542272604-787c3835535d" },
                    { 3, true, 3, "Chamarra negra urbana", "https://images.unsplash.com/photo-1551028719-00167b16eac5" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colores_Nombre",
                table: "Colores",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImagenesProducto_ProductoId",
                table: "ImagenesProducto",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CategoriaId",
                table: "Productos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ColorId",
                table: "Productos",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_TallaId",
                table: "Productos",
                column: "TallaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tallas_Nombre",
                table: "Tallas",
                column: "Nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImagenesProducto");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Colores");

            migrationBuilder.DropTable(
                name: "Tallas");
        }
    }
}
