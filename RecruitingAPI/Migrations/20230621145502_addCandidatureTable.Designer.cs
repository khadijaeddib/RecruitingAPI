﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecruitingAPI.Context;

#nullable disable

namespace RecruitingAPI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230621145502_addCandidatureTable")]
    partial class addCandidatureTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RecruitingAPI.Models.Admin", b =>
                {
                    b.Property<int>("idAdmin")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idAdmin"));

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("idAdmin");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("RecruitingAPI.Models.Candidate", b =>
                {
                    b.Property<int>("idCand")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idCand"));

                    b.Property<string>("address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("age")
                        .HasColumnType("int");

                    b.Property<string>("candImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("cin")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("cvPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("diploma")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("expYears")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("lmPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("spec")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("studyDegree")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("idCand");

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("RecruitingAPI.Models.Candidature", b =>
                {
                    b.Property<int>("idCandidature")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idCandidature"));

                    b.Property<DateTime>("dateCand")
                        .HasColumnType("datetime2");

                    b.Property<int>("idCand")
                        .HasColumnType("int");

                    b.Property<int>("idOffer")
                        .HasColumnType("int");

                    b.Property<string>("motivation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("statut")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("idCandidature");

                    b.HasIndex("idCand");

                    b.HasIndex("idOffer");

                    b.ToTable("Candidatures");
                });

            modelBuilder.Entity("RecruitingAPI.Models.Company", b =>
                {
                    b.Property<int>("idCo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idCo"));

                    b.Property<string>("address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("businessSector")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("idF")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("legalStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("logoPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("rc")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("website")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("idCo");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("RecruitingAPI.Models.Offer", b =>
                {
                    b.Property<int>("idOffer")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idOffer"));

                    b.Property<string>("availability")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("businessSector")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("city")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("contractType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("diploma")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("endDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("expYears")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("hiredNum")
                        .HasColumnType("int");

                    b.Property<int>("idRec")
                        .HasColumnType("int");

                    b.Property<string>("languages")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("missions")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("pubDate")
                        .HasColumnType("datetime2");

                    b.Property<float>("salary")
                        .HasColumnType("real");

                    b.Property<string>("skills")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("studyDegree")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("idOffer");

                    b.HasIndex("idRec");

                    b.ToTable("Offers");
                });

            modelBuilder.Entity("RecruitingAPI.Models.Recruiter", b =>
                {
                    b.Property<int>("idRec")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idRec"));

                    b.Property<string>("address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("age")
                        .HasColumnType("int");

                    b.Property<string>("career")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("fName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("idCo")
                        .HasColumnType("int");

                    b.Property<string>("lName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("pass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("recImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("token")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("idRec");

                    b.HasIndex("idCo");

                    b.ToTable("Recruiters");
                });

            modelBuilder.Entity("RecruitingAPI.Models.Candidature", b =>
                {
                    b.HasOne("RecruitingAPI.Models.Candidate", "Candidate")
                        .WithMany()
                        .HasForeignKey("idCand")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("RecruitingAPI.Models.Offer", "Offer")
                        .WithMany()
                        .HasForeignKey("idOffer")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Candidate");

                    b.Navigation("Offer");
                });

            modelBuilder.Entity("RecruitingAPI.Models.Offer", b =>
                {
                    b.HasOne("RecruitingAPI.Models.Recruiter", "Recruiter")
                        .WithMany()
                        .HasForeignKey("idRec")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recruiter");
                });

            modelBuilder.Entity("RecruitingAPI.Models.Recruiter", b =>
                {
                    b.HasOne("RecruitingAPI.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("idCo")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });
#pragma warning restore 612, 618
        }
    }
}
