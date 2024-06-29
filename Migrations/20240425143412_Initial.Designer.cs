﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PRACTICA_OFICIAL.DataLayer;

#nullable disable

namespace PRACTICA_OFICIAL.Migrations
{
    [DbContext(typeof(DB_Bolt))]
    [Migration("20240425143412_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.Comanda", b =>
                {
                    b.Property<int>("IdComanda")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdComanda"), 1L, 1);

                    b.Property<int>("IdCont")
                        .HasColumnType("int");

                    b.Property<int>("IdRestaurant")
                        .HasColumnType("int");

                    b.Property<int>("NumarulProduselor")
                        .HasColumnType("int");

                    b.HasKey("IdComanda");

                    b.HasIndex("IdCont");

                    b.HasIndex("IdRestaurant");

                    b.ToTable("Comenzi");
                });

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.Cont", b =>
                {
                    b.Property<int>("IdCont")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCont"), 1L, 1);

                    b.Property<string>("Adresa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Parola")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdCont");

                    b.ToTable("Cont");
                });

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.Produs", b =>
                {
                    b.Property<int>("IdProdus")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdProdus"), 1L, 1);

                    b.Property<int>("IdRestaurant")
                        .HasColumnType("int");

                    b.Property<string>("Nume")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Pret")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("IdProdus");

                    b.HasIndex("IdRestaurant");

                    b.ToTable("Produse");
                });

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.ProdusComandat", b =>
                {
                    b.Property<int>("IdProdusComandat")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdProdusComandat"), 1L, 1);

                    b.Property<int>("IdComanda")
                        .HasColumnType("int");

                    b.Property<int>("IdProdus")
                        .HasColumnType("int");

                    b.HasKey("IdProdusComandat");

                    b.HasIndex("IdComanda");

                    b.HasIndex("IdProdus");

                    b.ToTable("ProduseComandate");
                });

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.Restaurant", b =>
                {
                    b.Property<int>("IdRestaurant")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdRestaurant"), 1L, 1);

                    b.Property<string>("Adresa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nume")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdRestaurant");

                    b.ToTable("Restaurante");
                });

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.Comanda", b =>
                {
                    b.HasOne("PRACTICA_OFICIAL.DataLayer.Cont", "Cont")
                        .WithMany()
                        .HasForeignKey("IdCont")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PRACTICA_OFICIAL.DataLayer.Restaurant", "Restaurant")
                        .WithMany()
                        .HasForeignKey("IdRestaurant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cont");

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.Produs", b =>
                {
                    b.HasOne("PRACTICA_OFICIAL.DataLayer.Restaurant", "Restaurant")
                        .WithMany("Produse")
                        .HasForeignKey("IdRestaurant")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Restaurant");
                });

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.ProdusComandat", b =>
                {
                    b.HasOne("PRACTICA_OFICIAL.DataLayer.Comanda", "Comanda")
                        .WithMany()
                        .HasForeignKey("IdComanda")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PRACTICA_OFICIAL.DataLayer.Produs", "Produs")
                        .WithMany()
                        .HasForeignKey("IdProdus")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comanda");

                    b.Navigation("Produs");
                });

            modelBuilder.Entity("PRACTICA_OFICIAL.DataLayer.Restaurant", b =>
                {
                    b.Navigation("Produse");
                });
#pragma warning restore 612, 618
        }
    }
}