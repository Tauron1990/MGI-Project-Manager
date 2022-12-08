﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tauron.CQRS.Server.EventStore;

namespace Tauron.CQRS.Server.Migrations
{
    [DbContext(typeof(DispatcherDatabaseContext))]
    [Migration("20190920033604_V1")]
    partial class V1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Tauron.CQRS.Server.EventStore.Data.ApiKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Key");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ApiKeys");
                });

            modelBuilder.Entity("Tauron.CQRS.Server.EventStore.Data.EventEntity", b =>
                {
                    b.Property<long>("SequenceNumber")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Data");

                    b.Property<string>("EventName");

                    b.Property<int>("EventType");

                    b.Property<Guid?>("Id");

                    b.Property<string>("OriginType");

                    b.Property<DateTimeOffset>("TimeStamp");

                    b.Property<int>("Version");

                    b.HasKey("SequenceNumber");

                    b.ToTable("EventEntities");
                });

            modelBuilder.Entity("Tauron.CQRS.Server.EventStore.Data.ObjectStadeEntity", b =>
                {
                    b.Property<string>("Identifer")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Data");

                    b.Property<string>("OriginType");

                    b.HasKey("Identifer");

                    b.ToTable("ObjectStades");
                });
#pragma warning restore 612, 618
        }
    }
}