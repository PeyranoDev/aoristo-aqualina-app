using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class OptimizationAndNewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop Foreign Keys / Indices que pueden interferir (Esto ya está al principio)
            migrationBuilder.DropForeignKey(name: "FK_AmenityAvailability_Amenities_AmenityId", table: "AmenityAvailability");
            migrationBuilder.DropForeignKey(name: "FK_Users_Apartments_ApartmentId", table: "Users");
            migrationBuilder.DropForeignKey(name: "FK_Vehicles_Users_OwnerId", table: "Vehicles");
            migrationBuilder.DropIndex(name: "IX_Reservations_AmenityId", table: "Reservations");
            migrationBuilder.DropIndex(name: "IX_Requests_VehicleId", table: "Requests");
            migrationBuilder.DropPrimaryKey(name: "PK_AmenityAvailability", table: "AmenityAvailability");
            migrationBuilder.DropIndex(name: "IX_AmenityAvailability_AmenityId", table: "AmenityAvailability");
            // ... (otras eliminaciones/renombramientos)
            migrationBuilder.DropColumn(name: "IsActive", table: "Apartments");
            migrationBuilder.RenameTable(name: "AmenityAvailability", newName: "AmenityAvailabilities");
            migrationBuilder.RenameColumn(name: "id", table: "Reservations", newName: "Id");

            // 2. ALTERAR LOS TIPOS DE COLUMNAS DE INT A STRING (Esto ya está en el orden correcto ahora)
            migrationBuilder.AlterColumn<string>(name: "Type", table: "Roles", type: "nvarchar(max)", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<string>(name: "Status", table: "Requests", type: "nvarchar(450)", nullable: false, oldClrType: typeof(int), oldType: "int");
            migrationBuilder.AlterColumn<string>(name: "Status", table: "Reservations", type: "nvarchar(450)", nullable: false, oldClrType: typeof(int), oldType: "int");
            // ... (otros AlterColumn)
            migrationBuilder.AlterColumn<bool>(name: "IsOnDuty", table: "Users", type: "bit", nullable: false, defaultValue: false, oldClrType: typeof(bool), oldType: "bit", oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "Email", table: "Users", type: "nvarchar(450)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(max)");
            migrationBuilder.AlterColumn<string>(name: "Plate", table: "Vehicles", type: "nvarchar(450)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(max)");
            migrationBuilder.AlterColumn<string>(name: "ImageUrl", table: "News", type: "nvarchar(max)", nullable: true, oldClrType: typeof(string), oldType: "nvarchar(max)");

            // 3. Conversión de datos INT a STRING (Estos ya están en el orden correcto)
            migrationBuilder.Sql(
                @"UPDATE Roles SET Type = CASE Type WHEN 0 THEN 'Admin' WHEN 1 THEN 'Security' WHEN 2 THEN 'User' ELSE NULL END;");
            migrationBuilder.Sql(
                @"UPDATE Requests SET Status = CASE Status WHEN 0 THEN 'Pending' WHEN 1 THEN 'Approved' WHEN 2 THEN 'Completed' WHEN 3 THEN 'Rejected' ELSE NULL END;");
            migrationBuilder.Sql(
                @"UPDATE Reservations SET Status = CASE Status WHEN 0 THEN 'Confirmed' WHEN 1 THEN 'Cancelled' WHEN 2 THEN 'Pending' ELSE NULL END;");


            // 4. CREAR NUEVAS TABLAS Y COLUMNAS NECESARIAS PARA LA FK
            // *** ESTA ES LA SECCIÓN CLAVE A MODIFICAR ***

            // Crea la tabla Towers *antes* de que intentes referenciarla
            migrationBuilder.CreateTable(
                name: "Towers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adress = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Towers", x => x.Id);
                });

            // Agrega la nueva columna TowerId a Apartments (si aún no existe)
            // Ya está en tu migración, pero asegúrate de su posición.
            // La columna TowerId en Apartments es nullable: false con defaultValue: 0.
            // Si Apartments ya tiene registros, todos tendrán TowerId = 0.
            migrationBuilder.AddColumn<int>(
                name: "TowerId",
                table: "Apartments",
                type: "int",
                nullable: false, // Este es el problema si no hay Tower.Id = 0 o si no actualizas.
                defaultValue: 0); // Asumiendo que 0 es el valor por defecto

            // INSERTAR UNA TORRE POR DEFECTO PARA MANEJAR LOS VALORES EXISTENTES
            // Antes de crear la FK, debes asegurar que los valores existentes en Apartments.TowerId
            // existan en Towers.Id.
            // Si defaultValue: 0 es el problema, inserta una fila en Towers con ID=0 (requiere IDENTITY_INSERT ON)
            // O, inserta una torre y actualiza los Apartments para que apunten a esa nueva torre.

            // Opción A (más limpia): Insertar una torre con IDENTITY_INSERT y luego usar su ID.
            // Esta opción es más compleja porque requiere activar/desactivar IDENTITY_INSERT.
            // migrationBuilder.Sql("SET IDENTITY_INSERT Towers ON;");
            // migrationBuilder.Sql("INSERT INTO Towers (Id, Name, Adress) VALUES (0, 'Default Tower', 'N/A');");
            // migrationBuilder.Sql("SET IDENTITY_INSERT Towers OFF;");

            // Opción B (más práctica): Insertar una torre normal y luego actualizar Apartments a su ID.
            // Ejecuta esto inmediatamente después de crear la tabla Towers.
            migrationBuilder.Sql("INSERT INTO Towers (Name, Adress) VALUES ('Default Tower', 'Default Address');");

            // Obtener el ID de la torre recién insertada.
            // Nota: IDENTITY_SCOPE() es específico de SQL Server. Para otros DBs, la sintaxis varía.
            // Para SQL Server, si la columna TowerId en Apartments se agregó con defaultValue: 0,
            // necesitas que exista una Tower con ID 0, o actualizar los apartamentos a un ID válido.
            // Si TowerId tiene defaultValue 0, y Tower.Id es IDENTITY(1,1), entonces no se puede crear una FK
            // que apunte a 0 si no hay una fila con ID 0 en Towers.

            // SOLUCIÓN MÁS ROBUSTA PARA EL DEFAULTVALUE Y FK
            // Necesitamos que el defaultValue de TowerId en Apartments apunte a una Tower que ya existe.
            // Si tu diseño permite TowerId = 0 en Apartments (o cualquier otro valor no existente en Towers):
            // 1. Haz que TowerId en Apartments sea NULLABLE primero:
            migrationBuilder.AlterColumn<int>(
                name: "TowerId",
                table: "Apartments",
                type: "int",
                nullable: true, // Temporalmente nullable
                oldClrType: typeof(int),
                oldType: "int");

            // Ahora que es nullable, agrega la columna si aún no lo has hecho
            // (Si ya la agregaste con defaultValue 0, este AlterColumn la hará nullable sin conflicto)
            // Si la columna TowerId ya existe, esta AlterColumn es para cambiar su nulabilidad.

            // Actualiza todos los Apartments existentes para que apunten a la nueva Tower (Id = 1 si es el primer insert)
            // o a una Tower que sepas que existirá.
            // Esto asume que el primer ID de Tower insertado será 1 (debido a IDENTITY(1,1)).
            migrationBuilder.Sql("UPDATE Apartments SET TowerId = (SELECT TOP 1 Id FROM Towers);"); // Asigna el ID de la primera torre creada
            // Consideración: Si el select retorna NULL (no hay torres), esto podría causar problemas.
            // Si estás seguro de que 'Default Tower' se inserta y tendrá ID 1, puedes hacer:
            // migrationBuilder.Sql("UPDATE Apartments SET TowerId = 1 WHERE TowerId IS NULL OR TowerId = 0;"); // Actualiza los que tienen 0 o NULL

            // Después de actualizar, si quieres que TowerId sea NOT NULLABLE de nuevo, haz otro AlterColumn.
            // Esto solo si es estrictamente necesario que sea NOT NULL.
            migrationBuilder.AlterColumn<int>(
                name: "TowerId",
                table: "Apartments",
                type: "int",
                nullable: false, // Ahora sí, hazla no nullable de nuevo
                defaultValue: 1, // Asegúrate de que este defaultValue sea un ID válido en Towers
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);


            // 5. El resto de las nuevas columnas y tablas (fuera de la FK de Apartments)
            migrationBuilder.AddColumn<int>(
                name: "CompletedById",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TowerId",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0); // Asumiendo que News.TowerId también tiene un defaultValue que puede necesitar manejo.

            migrationBuilder.AddColumn<int>(
                name: "TowerId",
                table: "Amenities",
                type: "int",
                nullable: false,
                defaultValue: 0); // Asumiendo que Amenities.TowerId también tiene un defaultValue que puede necesitar manejo.

            migrationBuilder.AddPrimaryKey(
                name: "PK_AmenityAvailabilities",
                table: "AmenityAvailabilities",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                    TowerId = table.Column<int>(type: "int", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BannerUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondaryColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_AppSettings", x => x.Id);
                        table.ForeignKey(
                            name: "FK_AppSettings_Towers_TowerId",
                            column: x => x.TowerId,
                            principalTable: "Towers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

            // 6. Seed de datos
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Type",
                value: "Admin");
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Type",
                value: "Security");
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Type",
                value: "User");

            // 7. CREAR ÍNDICES Y AGREGAR CLAVES FORÁNEAS (FINALMENTE)
            // Las FKs de Amenity y News y AppSettings a Towers pueden ir aquí.
            // ¡La FK de Apartments a Towers se agrega aquí!

            migrationBuilder.CreateIndex(name: "IX_Vehicles_Plate", table: "Vehicles", column: "Plate", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Users_Email", table: "Users", column: "Email", unique: true);
            migrationBuilder.CreateIndex(name: "IX_Reservations_AmenityId_ReservationDate_Status", table: "Reservations", columns: new[] { "AmenityId", "ReservationDate", "Status" });
            migrationBuilder.CreateIndex(name: "IX_Requests_CompletedById", table: "Requests", column: "CompletedById");
            migrationBuilder.CreateIndex(name: "IX_Requests_VehicleId_Status", table: "Requests", columns: new[] { "VehicleId", "Status" })
                .Annotation("SqlServer:Include", new[] { "CompletedAt", "RequestedAt" });
            migrationBuilder.CreateIndex(name: "IX_News_TowerId", table: "News", column: "TowerId");
            migrationBuilder.CreateIndex(name: "IX_Apartments_TowerId", table: "Apartments", column: "TowerId"); // Asegúrate de que este índice exista
            migrationBuilder.CreateIndex(name: "IX_Amenities_TowerId", table: "Amenities", column: "TowerId");
            migrationBuilder.CreateIndex(name: "IX_AmenityAvailabilities_AmenityId_DayOfWeek", table: "AmenityAvailabilities", columns: new[] { "AmenityId", "DayOfWeek" });
            migrationBuilder.CreateIndex(name: "IX_AppSettings_TowerId", table: "AppSettings", column: "TowerId", unique: true);

            // AGREGAR LA CLAVE FORÁNEA PARA APARTMENTS.TOWERID AHORA QUE LOS DATOS SON VÁLIDOS
            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Towers_TowerId",
                table: "Apartments",
                column: "TowerId",
                principalTable: "Towers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade); // O .Restrict, .SetNull, según tu modelo

            // Otras FKs que hacen referencia a Towers
            migrationBuilder.AddForeignKey(
                name: "FK_Amenities_Towers_TowerId",
                table: "Amenities",
                column: "TowerId",
                principalTable: "Towers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_News_Towers_TowerId",
                table: "News",
                column: "TowerId",
                principalTable: "Towers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Las demás FKs que no tienen que ver con Towers
            migrationBuilder.AddForeignKey(name: "FK_AmenityAvailabilities_Amenities_AmenityId", table: "AmenityAvailabilities", column: "AmenityId", principalTable: "Amenities", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Requests_Users_CompletedById", table: "Requests", column: "CompletedById", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_Users_Apartments_ApartmentId", table: "Users", column: "ApartmentId", principalTable: "Apartments", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
            migrationBuilder.AddForeignKey(name: "FK_Vehicles_Users_OwnerId", table: "Vehicles", column: "OwnerId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ... (restricciones drop, etc. al principio del Down)
            // En el Down, el orden inverso es importante:
            // 1. Eliminar FKs que apuntan a Towers
            // 2. Eliminar datos de Towers si fueron insertados por la migración
            // 3. Eliminar la tabla Towers
            // 4. Revertir los AlterColumn y Sql de los Enums
            // ... (el resto del Down)

            // Eliminar FKs que apuntan a Towers
            migrationBuilder.DropForeignKey(name: "FK_Apartments_Towers_TowerId", table: "Apartments");
            migrationBuilder.DropForeignKey(name: "FK_Amenities_Towers_TowerId", table: "Amenities");
            migrationBuilder.DropForeignKey(name: "FK_News_Towers_TowerId", table: "News");
            migrationBuilder.DropForeignKey(name: "FK_AppSettings_Towers_TowerId", table: "AppSettings"); // Asegúrate de dropear esta también

            // Otros drop FKs
            migrationBuilder.DropForeignKey(name: "FK_AmenityAvailability_Amenities_AmenityId", table: "AmenityAvailability");
            migrationBuilder.DropForeignKey(name: "FK_Users_Apartments_ApartmentId", table: "Users");
            migrationBuilder.DropForeignKey(name: "FK_Vehicles_Users_OwnerId", table: "Vehicles");
            migrationBuilder.DropForeignKey(name: "FK_Requests_Users_CompletedById", table: "Requests"); // Si se agregó en Up

            // Eliminar tablas creadas
            migrationBuilder.DropTable(name: "AppSettings");
            migrationBuilder.DropTable(name: "Towers"); // Elimina la tabla Towers después de las FKs

            // Revertir la columna TowerId en Apartments
            migrationBuilder.DropIndex(name: "IX_Apartments_TowerId", table: "Apartments"); // Drop Index si existía
            migrationBuilder.DropColumn(name: "TowerId", table: "Apartments"); // Elimina la columna TowerId de Apartments

            // Revertir las columnas TowerId de News y Amenities
            migrationBuilder.DropColumn(name: "TowerId", table: "News");
            migrationBuilder.DropColumn(name: "TowerId", table: "Amenities");
            migrationBuilder.DropColumn(name: "CompletedById", table: "Requests");


            // Revertir los AlterColumn y las sentencias SQL de los Enums
            migrationBuilder.Sql(
                @"UPDATE Requests SET Status = CASE Status WHEN 'Pending' THEN 0 WHEN 'Approved' THEN 1 WHEN 'Completed' THEN 2 WHEN 'Rejected' THEN 3 ELSE 0 END;");
            migrationBuilder.Sql(
                @"UPDATE Reservations SET Status = CASE Status WHEN 'Confirmed' THEN 0 WHEN 'Cancelled' THEN 1 WHEN 'Pending' THEN 2 ELSE 0 END;");
            migrationBuilder.Sql(
                @"UPDATE Roles SET Type = CASE Type WHEN 'Admin' THEN 0 WHEN 'Security' THEN 1 WHEN 'User' THEN 2 ELSE 0 END;");

            migrationBuilder.AlterColumn<int>(name: "Type", table: "Roles", type: "int", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(max)");
            migrationBuilder.AlterColumn<int>(name: "Status", table: "Reservations", type: "int", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(450)");
            migrationBuilder.AlterColumn<int>(name: "Status", table: "Requests", type: "int", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(450)");

            // Revertir otros AlterColumn
            migrationBuilder.AlterColumn<string>(name: "Plate", table: "Vehicles", type: "nvarchar(max)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(450)");
            migrationBuilder.AlterColumn<bool>(name: "IsOnDuty", table: "Users", type: "bit", nullable: true, oldClrType: typeof(bool), oldType: "bit");
            migrationBuilder.AlterColumn<string>(name: "Email", table: "Users", type: "nvarchar(max)", nullable: false, oldClrType: typeof(string), oldType: "nvarchar(450)");
            migrationBuilder.AlterColumn<string>(name: "ImageUrl", table: "News", type: "nvarchar(max)", nullable: false, defaultValue: "", oldClrType: typeof(string), oldType: "nvarchar(max)", oldNullable: true);
            migrationBuilder.AddColumn<bool>(name: "IsActive", table: "Apartments", type: "bit", nullable: false, defaultValue: false); // Columna que se había eliminado en Up

            // Revertir renombramientos y Primary Keys
            migrationBuilder.RenameTable(name: "AmenityAvailabilities", newName: "AmenityAvailability");
            migrationBuilder.RenameColumn(name: "Id", table: "Reservations", newName: "id");
            migrationBuilder.DropPrimaryKey(name: "PK_AmenityAvailabilities", table: "AmenityAvailabilities");
            migrationBuilder.AddPrimaryKey(name: "PK_AmenityAvailability", table: "AmenityAvailability", column: "Id");


            // Actualizar Seed Data (con los valores INT de los enums)
            migrationBuilder.UpdateData(table: "Roles", keyColumn: "Id", keyValue: 1, column: "Type", value: 0);
            migrationBuilder.UpdateData(table: "Roles", keyColumn: "Id", keyValue: 2, column: "Type", value: 1);
            migrationBuilder.UpdateData(table: "Roles", keyColumn: "Id", keyValue: 3, column: "Type", value: 2);

            // Re-crear índices y Foreign Keys originales (si fueron eliminados)
            migrationBuilder.CreateIndex(name: "IX_Reservations_AmenityId", table: "Reservations", column: "AmenityId");
            migrationBuilder.CreateIndex(name: "IX_Requests_VehicleId", table: "Requests", column: "VehicleId");
            migrationBuilder.CreateIndex(name: "IX_AmenityAvailability_AmenityId", table: "AmenityAvailability", column: "AmenityId");

            // Re-agregar FKs originales (que no involucran a Towers)
            migrationBuilder.AddForeignKey(name: "FK_AmenityAvailability_Amenities_AmenityId", table: "AmenityAvailability", column: "AmenityId", principalTable: "Amenities", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(name: "FK_Users_Apartments_ApartmentId", table: "Users", column: "ApartmentId", principalTable: "Apartments", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(name: "FK_Vehicles_Users_OwnerId", table: "Vehicles", column: "OwnerId", principalTable: "Users", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
        }
    }
}