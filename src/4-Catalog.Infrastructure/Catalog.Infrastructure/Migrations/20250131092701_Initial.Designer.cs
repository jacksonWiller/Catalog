// <auto-generated />
using System;
using Catalog.Infrastructure.PostgreSql.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Catalog.Infrastructure.PostgreSql.Migrations
{
    [DbContext(typeof(CatalogDbContext))]
    [Migration("20250131092701_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("Latin1_General_CI_AI")
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Catalog.Core.SharedKernel.EventStore", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AggregateId")
                        .HasColumnType("uuid");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("text")
                        .HasComment("JSON serialized event");

                    b.Property<string>("MessageType")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("OccurredOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("EventStores", (string)null);

                    b.HasDiscriminator().HasValue("EventStore");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.ProductAggregate.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<bool>("_isDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("Catalog.Domain.Entities.ProductAggregate.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric(18,2)");

                    b.Property<string>("SKU")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("StockQuantity")
                        .HasColumnType("int");

                    b.Property<bool>("_isDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("ProductCategory", b =>
                {
                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.HasKey("CategoryId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductCategory");
                });

            modelBuilder.Entity("Catalog.Domain.Entities.ProductAggregate.Product", b =>
                {
                    b.OwnsMany("Catalog.Domain.ValueObjects.Image", "Images", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uuid");

                            b1.Property<string>("Height")
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)");

                            b1.Property<string>("Prefix")
                                .IsRequired()
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)");

                            b1.Property<Guid>("ProductId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Url")
                                .IsRequired()
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)");

                            b1.Property<string>("Width")
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)");

                            b1.HasKey("Id");

                            b1.HasIndex("ProductId");

                            b1.ToTable("ProductImages", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.OwnsMany("Catalog.Domain.ValueObjects.Tag", "Tags", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uuid");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)");

                            b1.Property<Guid>("ProductId")
                                .HasColumnType("uuid");

                            b1.HasKey("Id");

                            b1.HasIndex("ProductId");

                            b1.ToTable("ProductTags", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.Navigation("Images");

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("ProductCategory", b =>
                {
                    b.HasOne("Catalog.Domain.Entities.ProductAggregate.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ProductCategory_Categories_CategoryId");

                    b.HasOne("Catalog.Domain.Entities.ProductAggregate.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_ProductCategory_Products_ProductId");
                });
#pragma warning restore 612, 618
        }
    }
}
