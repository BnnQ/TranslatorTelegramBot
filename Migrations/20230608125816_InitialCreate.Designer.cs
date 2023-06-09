﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TranslatorTelegramBot.Models.Contexts;

#nullable disable

namespace TranslatorTelegramBot.Migrations
{
    [DbContext(typeof(TranslatorUsersContext))]
    [Migration("20230608125816_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TranslatorTelegramBot.Models.Entities.User", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnType("bigint");

                    b.Property<string>("SourceLanguage")
                        .HasMaxLength(4)
                        .HasColumnType("varchar(4)");

                    b.Property<string>("TargetLanguage")
                        .HasMaxLength(4)
                        .HasColumnType("varchar(4)");

                    b.HasKey("ChatId");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
