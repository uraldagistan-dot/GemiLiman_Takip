using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortTrackingSystem.Core.Migrations
{
    /// <inheritdoc />
    public partial class FixCrewMemberFirstNameAndOptionalContact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirtsName",
                table: "CrewMembers",
                newName: "FirstName");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "CrewMembers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "CrewMembers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ShipVisits_PortId",
                table: "ShipVisits",
                column: "PortId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipVisits_ShipId",
                table: "ShipVisits",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipCrewAssignments_CrewId",
                table: "ShipCrewAssignments",
                column: "CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipCrewAssignments_ShipId",
                table: "ShipCrewAssignments",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargoes_ShipId",
                table: "Cargoes",
                column: "ShipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cargoes_Ships_ShipId",
                table: "Cargoes",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "ShipId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipCrewAssignments_CrewMembers_CrewId",
                table: "ShipCrewAssignments",
                column: "CrewId",
                principalTable: "CrewMembers",
                principalColumn: "CrewId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipCrewAssignments_Ships_ShipId",
                table: "ShipCrewAssignments",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "ShipId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipVisits_Ports_PortId",
                table: "ShipVisits",
                column: "PortId",
                principalTable: "Ports",
                principalColumn: "PortId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShipVisits_Ships_ShipId",
                table: "ShipVisits",
                column: "ShipId",
                principalTable: "Ships",
                principalColumn: "ShipId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cargoes_Ships_ShipId",
                table: "Cargoes");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipCrewAssignments_CrewMembers_CrewId",
                table: "ShipCrewAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipCrewAssignments_Ships_ShipId",
                table: "ShipCrewAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipVisits_Ports_PortId",
                table: "ShipVisits");

            migrationBuilder.DropForeignKey(
                name: "FK_ShipVisits_Ships_ShipId",
                table: "ShipVisits");

            migrationBuilder.DropIndex(
                name: "IX_ShipVisits_PortId",
                table: "ShipVisits");

            migrationBuilder.DropIndex(
                name: "IX_ShipVisits_ShipId",
                table: "ShipVisits");

            migrationBuilder.DropIndex(
                name: "IX_ShipCrewAssignments_CrewId",
                table: "ShipCrewAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ShipCrewAssignments_ShipId",
                table: "ShipCrewAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Cargoes_ShipId",
                table: "Cargoes");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "CrewMembers",
                newName: "FirtsName");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "CrewMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "CrewMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
