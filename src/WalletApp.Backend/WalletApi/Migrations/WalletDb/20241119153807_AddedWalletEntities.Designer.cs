﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WalletEntities.Data;

#nullable disable

namespace WalletApi.Migrations.WalletDb
{
    [DbContext(typeof(WalletDbContext))]
    [Migration("20241119153807_AddedWalletEntities")]
    partial class AddedWalletEntities
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WalletEntities.Domain.Entities.AuthorizedUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_authorized_users");

                    b.ToTable("authorized_users", (string)null);
                });

            modelBuilder.Entity("WalletEntities.Domain.Entities.Card", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_cards");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_cards_user_id");

                    b.ToTable("cards", (string)null);
                });

            modelBuilder.Entity("WalletEntities.Domain.Entities.Transaction", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("AuthorizedUserId")
                        .HasColumnType("text")
                        .HasColumnName("authorized_user_id");

                    b.Property<string>("CardId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("card_id");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("date");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2048)
                        .HasColumnType("character varying(2048)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("name");

                    b.Property<bool>("Pending")
                        .HasColumnType("boolean")
                        .HasColumnName("pending");

                    b.Property<decimal>("Sum")
                        .HasColumnType("numeric")
                        .HasColumnName("sum");

                    b.Property<int>("Type")
                        .HasColumnType("integer")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_transactions");

                    b.HasIndex("AuthorizedUserId")
                        .HasDatabaseName("ix_transactions_authorized_user_id");

                    b.HasIndex("CardId")
                        .HasDatabaseName("ix_transactions_card_id");

                    b.ToTable("transactions", (string)null);
                });

            modelBuilder.Entity("WalletEntities.Domain.Entities.Card", b =>
                {
                    b.HasOne("WalletEntities.Domain.Entities.AuthorizedUser", "User")
                        .WithMany("Cards")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cards_authorized_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WalletEntities.Domain.Entities.Transaction", b =>
                {
                    b.HasOne("WalletEntities.Domain.Entities.AuthorizedUser", "AuthorizedUser")
                        .WithMany()
                        .HasForeignKey("AuthorizedUserId")
                        .HasConstraintName("fk_transactions_authorized_users_authorized_user_id");

                    b.HasOne("WalletEntities.Domain.Entities.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_transactions_cards_card_id");

                    b.Navigation("AuthorizedUser");

                    b.Navigation("Card");
                });

            modelBuilder.Entity("WalletEntities.Domain.Entities.AuthorizedUser", b =>
                {
                    b.Navigation("Cards");
                });
#pragma warning restore 612, 618
        }
    }
}
