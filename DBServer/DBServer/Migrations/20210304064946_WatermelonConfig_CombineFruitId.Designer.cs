﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository.Data;

namespace DBServer.Migrations
{
    [DbContext(typeof(GameDbContext))]
    [Migration("20210304064946_WatermelonConfig_CombineFruitId")]
    partial class WatermelonConfig_CombineFruitId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Repository.Models.FruitConfig", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValueSql("newid()");

                    b.Property<int>("CombineFruitId")
                        .HasColumnType("int")
                        .HasComment("合成后的水果Id");

                    b.Property<int>("FruitId")
                        .HasColumnType("int")
                        .HasComment("水果Id");

                    b.Property<string>("Image")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasComment("图片");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasComment("名称");

                    b.Property<int>("Rate")
                        .HasColumnType("int")
                        .HasComment("概率");

                    b.Property<int>("Score")
                        .HasColumnType("int")
                        .HasComment("合成可得的积分");

                    b.HasKey("Id");

                    b.HasIndex("FruitId")
                        .IsUnique()
                        .HasFilter("[FruitId] IS NOT NULL");

                    b.ToTable("FruitConfig");

                    b
                        .HasComment("合成大西瓜水果配置表");
                });

            modelBuilder.Entity("Repository.Models.GameBox", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("Amount")
                        .HasColumnType("bigint")
                        .HasComment("优惠金额");

                    b.Property<string>("CouponsId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasComment("优惠券Id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasComment("角色Id");

                    b.Property<DateTime>("UpateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("GameBox");

                    b
                        .HasComment("游戏宝箱表");
                });

            modelBuilder.Entity("Repository.Models.GameRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValueSql("newid()");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasComment("昵称");

                    b.Property<long>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasComment("角色Id")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("UpateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("getdate()");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasComment("用户Id");

                    b.HasKey("Id");

                    b.ToTable("GameRole");

                    b
                        .HasComment("游戏角色表");
                });

            modelBuilder.Entity("Repository.Models.GameScore", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasComment("角色Id");

                    b.Property<long>("Score")
                        .HasColumnType("bigint")
                        .HasComment("积分");

                    b.Property<DateTime>("UpateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("GameScore");

                    b
                        .HasComment("游戏积分表");
                });

            modelBuilder.Entity("Repository.Models.GameTruntable", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("AwardId")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasComment("奖励Id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint")
                        .HasComment("角色Id");

                    b.Property<DateTime>("UpateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("GameTruntable");

                    b
                        .HasComment("大转盘记录表");
                });

            modelBuilder.Entity("Repository.Models.TruntableConfig", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("AwardDesc")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("奖励描述");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2");

                    b.Property<string>("ImagePath")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasComment("图片路径");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit")
                        .HasComment("是否开启");

                    b.Property<long>("Price")
                        .HasColumnType("bigint")
                        .HasComment("价值（单位：分）");

                    b.Property<DateTime>("UpateTime")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("TruntableConfig");

                    b
                        .HasComment("合成大西瓜大转盘配置表");
                });
#pragma warning restore 612, 618
        }
    }
}
