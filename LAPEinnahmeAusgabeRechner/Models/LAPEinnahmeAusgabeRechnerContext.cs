﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using LAPEinnahmeAusgabeRechner.Models.Configurations;
using LAPEinnahmeAusgabeRechner.Models.dboSchema;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
#nullable disable

namespace LAPEinnahmeAusgabeRechner.Models;

public partial class LAPEinnahmeAusgabeRechnerContext : DbContext
{
    public LAPEinnahmeAusgabeRechnerContext(DbContextOptions<LAPEinnahmeAusgabeRechnerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ausgabe> Ausgabe { get; set; }

    public virtual DbSet<Einnahme> Einnahme { get; set; }

    public virtual DbSet<Kategorie> Kategorie { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.AusgabeConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.EinnahmeConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.KategorieConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
