﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20221123002422_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DAL.Entites.Attach", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Attaches");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("DAL.Entites.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("DAL.Entites.Like", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserLikeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserLikeId");

                    b.ToTable("Likes");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("DAL.Entites.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DAL.Entites.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DAL.Entites.UserSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<Guid>("RefreshToken")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("DAL.Entites.Avatar", b =>
                {
                    b.HasBaseType("DAL.Entites.Attach");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Avatars", (string)null);
                });

            modelBuilder.Entity("DAL.Entites.PostPicture", b =>
                {
                    b.HasBaseType("DAL.Entites.Attach");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.HasIndex("PostId");

                    b.ToTable("PostPictures");
                });

            modelBuilder.Entity("DAL.Entites.LikeComment", b =>
                {
                    b.HasBaseType("DAL.Entites.Like");

                    b.Property<Guid>("CommentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserLikeCommentId")
                        .HasColumnType("uuid");

                    b.HasIndex("CommentId");

                    b.HasIndex("UserLikeCommentId");

                    b.ToTable("LikeComments", (string)null);
                });

            modelBuilder.Entity("DAL.Entites.LikePost", b =>
                {
                    b.HasBaseType("DAL.Entites.Like");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserLikePostId")
                        .HasColumnType("uuid");

                    b.HasIndex("PostId");

                    b.HasIndex("UserLikePostId");

                    b.ToTable("LikePosts", (string)null);
                });

            modelBuilder.Entity("DAL.Entites.Attach", b =>
                {
                    b.HasOne("DAL.Entites.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("DAL.Entites.Comment", b =>
                {
                    b.HasOne("DAL.Entites.Post", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("DAL.Entites.Like", b =>
                {
                    b.HasOne("DAL.Entites.User", "UserLike")
                        .WithMany("Likes")
                        .HasForeignKey("UserLikeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserLike");
                });

            modelBuilder.Entity("DAL.Entites.Post", b =>
                {
                    b.HasOne("DAL.Entites.User", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("DAL.Entites.UserSession", b =>
                {
                    b.HasOne("DAL.Entites.User", "User")
                        .WithMany("Sessions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAL.Entites.Avatar", b =>
                {
                    b.HasOne("DAL.Entites.Attach", null)
                        .WithOne()
                        .HasForeignKey("DAL.Entites.Avatar", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entites.User", "User")
                        .WithOne("Avatar")
                        .HasForeignKey("DAL.Entites.Avatar", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DAL.Entites.PostPicture", b =>
                {
                    b.HasOne("DAL.Entites.Attach", null)
                        .WithOne()
                        .HasForeignKey("DAL.Entites.PostPicture", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entites.Post", "Post")
                        .WithMany("PostPictures")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");
                });

            modelBuilder.Entity("DAL.Entites.LikeComment", b =>
                {
                    b.HasOne("DAL.Entites.Comment", "Comment")
                        .WithMany("LikeComments")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entites.Like", null)
                        .WithOne()
                        .HasForeignKey("DAL.Entites.LikeComment", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entites.User", "UserLikeComment")
                        .WithMany()
                        .HasForeignKey("UserLikeCommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("UserLikeComment");
                });

            modelBuilder.Entity("DAL.Entites.LikePost", b =>
                {
                    b.HasOne("DAL.Entites.Like", null)
                        .WithOne()
                        .HasForeignKey("DAL.Entites.LikePost", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entites.Post", "Post")
                        .WithMany("LikePosts")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entites.User", "UserLikePost")
                        .WithMany()
                        .HasForeignKey("UserLikePostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("UserLikePost");
                });

            modelBuilder.Entity("DAL.Entites.Comment", b =>
                {
                    b.Navigation("LikeComments");
                });

            modelBuilder.Entity("DAL.Entites.Post", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("LikePosts");

                    b.Navigation("PostPictures");
                });

            modelBuilder.Entity("DAL.Entites.User", b =>
                {
                    b.Navigation("Avatar");

                    b.Navigation("Likes");

                    b.Navigation("Posts");

                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
