﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using LAPEinnahmeAusgabeRechner.Models.dboSchema;


namespace LAPEinnahmeAusgabeRechner.Models.dboSchema;

[Table("AUSGABE")]
[Index("Valid", Name = "IX_AUSGABE_VALID", AllDescending = true)]
public partial class Ausgabe
{
    [Key]
    [Column("AUSGABEID")]
    public long Ausgabeid { get; set; }

    [Column("KATEGORIEID")]
    public long Kategorieid { get; set; }

    [Column("BESCHREIBUNG")]
    [StringLength(100)]
    public string Beschreibung { get; set; }

    [Column("BETRAG", TypeName = "decimal(18, 2)")]
    public decimal Betrag { get; set; }

    [Column("DATUM", TypeName = "datetime")]
    public DateTime Datum { get; set; }

    [Column("VALID")]
    public short Valid { get; set; }

    [Required]
    [Column("MOD_USER")]
    [StringLength(256)]
    public string ModUser { get; set; }

    [Column("MOD_TIMESTAMP", TypeName = "datetime")]
    public DateTime ModTimestamp { get; set; }

    [Required]
    [Column("CR_USER")]
    [StringLength(256)]
    public string CrUser { get; set; }

    [Column("CR_TIMESTAMP", TypeName = "datetime")]
    public DateTime CrTimestamp { get; set; }

    [ForeignKey("Kategorieid")]
    [InverseProperty("Ausgabe")]
    public virtual Kategorie Kategorie { get; set; }
}