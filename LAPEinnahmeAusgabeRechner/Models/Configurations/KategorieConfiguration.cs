﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using LAPEinnahmeAusgabeRechner.Models;
using LAPEinnahmeAusgabeRechner.Models.dboSchema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace LAPEinnahmeAusgabeRechner.Models.Configurations
{
    public partial class KategorieConfiguration : IEntityTypeConfiguration<Kategorie>
    {
        public void Configure(EntityTypeBuilder<Kategorie> entity)
        {
            entity.ToTable("KATEGORIE", tb =>
                {
                    tb.HasTrigger("TRG_BI_KATEGORIE");
                    tb.HasTrigger("TRG_BU_KATEGORIE");
                });

            entity.Property(e => e.Farbe).IsFixedLength();
            entity.Property(e => e.Valid).HasDefaultValue((short)1);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Kategorie> entity);
    }
}
