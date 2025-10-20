using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Usuario = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contrasenia = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apellido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Segundo_Apellido = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Documento_Identidad = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rol = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Razon_Social = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NIT = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Fecha_Creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fecha_Actualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Usuario_Creacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true),
                    Usuario_Actualizacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Cliente_Usuario_Usuario_Actualizacion_ID",
                        column: x => x.Usuario_Actualizacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Cliente_Usuario_Usuario_Creacion_ID",
                        column: x => x.Usuario_Creacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Huesped",
                columns: table => new
                {
                    ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Apellido = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Segundo_Apellido = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Documento_Identidad = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha_Nacimiento = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Fecha_Creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fecha_Actualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Usuario_Creacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true),
                    Usuario_Actualizacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Huesped", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Huesped_Usuario_Usuario_Actualizacion_ID",
                        column: x => x.Usuario_Actualizacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Huesped_Usuario_Usuario_Creacion_ID",
                        column: x => x.Usuario_Creacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tipo_Habitacion",
                columns: table => new
                {
                    ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Capacidad_Maxima = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Precio_Base = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Fecha_Creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fecha_Actualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Usuario_Creacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true),
                    Usuario_Actualizacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipo_Habitacion", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tipo_Habitacion_Usuario_Usuario_Actualizacion_ID",
                        column: x => x.Usuario_Actualizacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Tipo_Habitacion_Usuario_Usuario_Creacion_ID",
                        column: x => x.Usuario_Creacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reserva",
                columns: table => new
                {
                    ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Cliente_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Estado_Reserva = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Monto_Total = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    Fecha_Creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Usuario_Creacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reserva", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Reserva_Cliente_Cliente_ID",
                        column: x => x.Cliente_ID,
                        principalTable: "Cliente",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reserva_Usuario_Usuario_Creacion_ID",
                        column: x => x.Usuario_Creacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Habitacion",
                columns: table => new
                {
                    ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Tipo_Habitacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Numero_Habitacion = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Piso = table.Column<short>(type: "smallint", nullable: false),
                    Estado_Habitacion = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha_Creacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fecha_Actualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Usuario_Creacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true),
                    Usuario_Actualizacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habitacion", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Habitacion_Tipo_Habitacion_Tipo_Habitacion_ID",
                        column: x => x.Tipo_Habitacion_ID,
                        principalTable: "Tipo_Habitacion",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Habitacion_Usuario_Usuario_Actualizacion_ID",
                        column: x => x.Usuario_Actualizacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Habitacion_Usuario_Usuario_Creacion_ID",
                        column: x => x.Usuario_Creacion_ID,
                        principalTable: "Usuario",
                        principalColumn: "ID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Detalle_Reserva",
                columns: table => new
                {
                    ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Reserva_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Habitacion_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Huesped_ID = table.Column<byte[]>(type: "BINARY(16)", nullable: false),
                    Fecha_Entrada = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fecha_Salida = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Reserva", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Detalle_Reserva_Habitacion_Habitacion_ID",
                        column: x => x.Habitacion_ID,
                        principalTable: "Habitacion",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Detalle_Reserva_Huesped_Huesped_ID",
                        column: x => x.Huesped_ID,
                        principalTable: "Huesped",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Detalle_Reserva_Reserva_Reserva_ID",
                        column: x => x.Reserva_ID,
                        principalTable: "Reserva",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Email",
                table: "Cliente",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Usuario_Actualizacion_ID",
                table: "Cliente",
                column: "Usuario_Actualizacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Usuario_Creacion_ID",
                table: "Cliente",
                column: "Usuario_Creacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Reserva_Habitacion_ID",
                table: "Detalle_Reserva",
                column: "Habitacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Reserva_Huesped_ID",
                table: "Detalle_Reserva",
                column: "Huesped_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Detalle_Reserva_Reserva_ID",
                table: "Detalle_Reserva",
                column: "Reserva_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Habitacion_Numero_Habitacion",
                table: "Habitacion",
                column: "Numero_Habitacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habitacion_Tipo_Habitacion_ID",
                table: "Habitacion",
                column: "Tipo_Habitacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Habitacion_Usuario_Actualizacion_ID",
                table: "Habitacion",
                column: "Usuario_Actualizacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Habitacion_Usuario_Creacion_ID",
                table: "Habitacion",
                column: "Usuario_Creacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Huesped_Usuario_Actualizacion_ID",
                table: "Huesped",
                column: "Usuario_Actualizacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Huesped_Usuario_Creacion_ID",
                table: "Huesped",
                column: "Usuario_Creacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_Cliente_ID",
                table: "Reserva",
                column: "Cliente_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reserva_Usuario_Creacion_ID",
                table: "Reserva",
                column: "Usuario_Creacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tipo_Habitacion_Nombre",
                table: "Tipo_Habitacion",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tipo_Habitacion_Usuario_Actualizacion_ID",
                table: "Tipo_Habitacion",
                column: "Usuario_Actualizacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Tipo_Habitacion_Usuario_Creacion_ID",
                table: "Tipo_Habitacion",
                column: "Usuario_Creacion_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Usuario",
                table: "Usuario",
                column: "Usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Detalle_Reserva");

            migrationBuilder.DropTable(
                name: "Habitacion");

            migrationBuilder.DropTable(
                name: "Huesped");

            migrationBuilder.DropTable(
                name: "Reserva");

            migrationBuilder.DropTable(
                name: "Tipo_Habitacion");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
