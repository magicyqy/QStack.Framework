﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using QStack.Framework.Persistent.EFCore;

namespace QStack.Framework.AutoMigration.sfdb.Migrations
{
    [DbContext(typeof(EFCoreDao))]
    [Migration("20201101025825_20201101105818_sfdb.Migrations")]
    partial class _20201101105818_sfdbMigrations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("QStack.Framework.AspNetCore.Plugin.Models.PluginInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text");

                    b.Property<string>("IntallPath")
                        .HasColumnType("text");

                    b.Property<bool>("IsEnable")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsMigration")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("RouteArea")
                        .HasColumnType("text");

                    b.Property<string>("TestUrl")
                        .HasColumnType("text");

                    b.Property<string>("UniqueKey")
                        .HasColumnType("text");

                    b.Property<string>("Version")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PluginInfo");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ArticleType")
                        .HasColumnType("integer");

                    b.Property<string>("Author")
                        .HasColumnType("text");

                    b.Property<int?>("CatagoryId")
                        .HasColumnType("integer");

                    b.Property<string>("CoverUrl")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<bool>("DisableComment")
                        .HasColumnType("boolean");

                    b.Property<int>("HotTop")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<int>("PageViews")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("PublishTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("SeoDescription")
                        .HasColumnType("text");

                    b.Property<string>("SeoKeyWord")
                        .HasColumnType("text");

                    b.Property<string>("Source")
                        .HasColumnType("text");

                    b.Property<string>("SourceUrl")
                        .HasColumnType("text");

                    b.Property<byte>("State")
                        .HasColumnType("smallint");

                    b.Property<string>("Summary")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.Property<int>("ZanNum")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CatagoryId");

                    b.ToTable("Article");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.ArticleContent", b =>
                {
                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<string>("Html")
                        .HasColumnType("text");

                    b.HasKey("ArticleId");

                    b.ToTable("ArticleContent");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.ArticleTag", b =>
                {
                    b.Property<int>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.HasKey("ArticleId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("ArticleTag");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.Catagory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Catagory");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ArticleId")
                        .HasColumnType("integer");

                    b.Property<string>("CommentText")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("NickName")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<int?>("ProductId")
                        .HasColumnType("integer");

                    b.Property<string>("RemoteIp")
                        .HasColumnType("text");

                    b.Property<string>("ReplyTo")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tag");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.DataAuthRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("EntityType")
                        .HasColumnType("text");

                    b.Property<string>("LambdaExpression")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Repository")
                        .HasColumnType("text");

                    b.Property<string>("RuleGroup")
                        .HasColumnType("text");

                    b.Property<int>("RuleState")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataAuthRule");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.Function", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Describe")
                        .HasColumnType("text");

                    b.Property<int>("FunctionType")
                        .HasColumnType("integer");

                    b.Property<bool>("Hidden")
                        .HasColumnType("boolean");

                    b.Property<string>("IconUrl")
                        .HasColumnType("text");

                    b.Property<bool>("IsLeaf")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<string>("Path")
                        .HasColumnType("text");

                    b.Property<string>("RouteName")
                        .HasColumnType("text");

                    b.Property<int>("Sequence")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Function");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.FunctionOperation", b =>
                {
                    b.Property<int>("FunctionId")
                        .HasColumnType("integer");

                    b.Property<int>("OperationId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Describe")
                        .HasColumnType("text");

                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<int?>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int>("Sequence")
                        .HasColumnType("integer");

                    b.HasKey("FunctionId", "OperationId");

                    b.HasIndex("OperationId");

                    b.HasIndex("RoleId");

                    b.ToTable("FunctionOperation");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Describe")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.GroupRole", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("GroupId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("GroupRole");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.Operation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AttributeValue")
                        .HasColumnType("text");

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<string>("ControlAttribute")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Describe")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Operation");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Describe")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Sequence")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.RoleFunction", b =>
                {
                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.Property<int>("FunctionId")
                        .HasColumnType("integer");

                    b.HasKey("RoleId", "FunctionId");

                    b.HasIndex("FunctionId");

                    b.ToTable("RoleFunction");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Describe")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<int?>("GroupId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Mobile")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PassWord")
                        .HasColumnType("text");

                    b.Property<int>("Sequence")
                        .HasColumnType("integer");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.NavigationMenu", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("FlowNo")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("NavigationMenu");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("BrandCD")
                        .HasColumnType("integer");

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("ImageThumbUrl")
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<bool>("IsPublish")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Norm")
                        .HasColumnType("text");

                    b.Property<int?>("OrderIndex")
                        .HasColumnType("integer");

                    b.Property<string>("PartNumber")
                        .HasColumnType("text");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,4)");

                    b.Property<int>("ProductCategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("ProductContent")
                        .HasColumnType("text");

                    b.Property<DateTime?>("PublishDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<decimal?>("PurchasePrice")
                        .HasColumnType("decimal(18,4)");

                    b.Property<decimal?>("RebatePrice")
                        .HasColumnType("decimal(18,4)");

                    b.Property<string>("SEOTitle")
                        .HasColumnType("text");

                    b.Property<string>("SeoDescription")
                        .HasColumnType("text");

                    b.Property<string>("SeoKeyWord")
                        .HasColumnType("text");

                    b.Property<string>("ShelfLife")
                        .HasColumnType("text");

                    b.Property<string>("SourceFrom")
                        .HasColumnType("text");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<string>("TargetFrom")
                        .HasColumnType("text");

                    b.Property<string>("TargetUrl")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.Property<int>("ViewCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductCategoryId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<string>("SEODescription")
                        .HasColumnType("text");

                    b.Property<string>("SEOKeyWord")
                        .HasColumnType("text");

                    b.Property<string>("SEOTitle")
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ProductCategory");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.ProductCategoryTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ParentId")
                        .HasColumnType("integer");

                    b.Property<int>("ProductCategoryId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ProductCategoryTag");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.ProductDownload", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("DownloadNum")
                        .HasColumnType("integer");

                    b.Property<string>("DownloadUrl")
                        .HasColumnType("text");

                    b.Property<string>("ExtDesc")
                        .HasColumnType("text");

                    b.Property<string>("Gid")
                        .HasColumnType("text");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<string>("ValidCode")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductDownload");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.ProductImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ImageUrl")
                        .HasColumnType("text");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("UploadFileId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("ProductImage");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.ProductTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ProductTag");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.UploadFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("CreateUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Extention")
                        .HasColumnType("text");

                    b.Property<string>("FileCaptureUrl")
                        .HasColumnType("text");

                    b.Property<string>("Filename")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifyDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LastModifyUserId")
                        .HasColumnType("integer");

                    b.Property<string>("MD5Code")
                        .HasColumnType("text");

                    b.Property<string>("RUrl")
                        .HasColumnType("text");

                    b.Property<string>("RelativeFucDes")
                        .HasColumnType("text");

                    b.Property<string>("RelativeId")
                        .HasColumnType("text");

                    b.Property<int>("ResouceType")
                        .HasColumnType("integer");

                    b.Property<int>("SHeight")
                        .HasColumnType("integer");

                    b.Property<int>("SWidth")
                        .HasColumnType("integer");

                    b.Property<int>("THeight")
                        .HasColumnType("integer");

                    b.Property<int>("TWidth")
                        .HasColumnType("integer");

                    b.Property<string>("ThumbnailUrl")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UploadFile");
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.Article", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.ArticleContent", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.ArticleTag", b =>
                {

                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.Catagory", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Articles.Comment", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.Function", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.FunctionOperation", b =>
                {


                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.Group", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.GroupRole", b =>
                {

                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.RoleFunction", b =>
                {

                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.User", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Auth.UserRole", b =>
                {

                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.NavigationMenu", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.Product", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.ProductDownload", b =>
                {
                });

            modelBuilder.Entity("QStack.Framework.Basic.Model.Shop.ProductImage", b =>
                {
                });
#pragma warning restore 612, 618
        }
    }
}
