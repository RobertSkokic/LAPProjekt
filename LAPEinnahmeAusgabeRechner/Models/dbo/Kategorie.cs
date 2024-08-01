﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using LAPEinnahmeAusgabeRechner.Models.dboSchema;


namespace LAPEinnahmeAusgabeRechner.Models.dboSchema;

[Table("KATEGORIE")]
[Index("Valid", Name = "IX_KATEGORIE_VALID", AllDescending = true)]
[Index("Bezeichnung", Name = "UIX_KATEGORIE_BEZEICHNUNG", IsUnique = true)]
public partial class Kategorie
{
    [Key]
    [Column("KATEGORIEID")]
    public long Kategorieid { get; set; }

    [Required]
    [Column("BEZEICHNUNG")]
    [StringLength(50)]
    public string Bezeichnung { get; set; }

    [Required]
    [Column("FARBE")]
    [StringLength(7)]
    [Unicode(false)]
    public string Farbe { get; set; }

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

    [InverseProperty("Kategorie")]
    public virtual ICollection<Ausgabe> Ausgabe { get; set; } = new List<Ausgabe>();

    [InverseProperty("Kategorie")]
    public virtual ICollection<Einnahme> Einnahme { get; set; } = new List<Einnahme>();
}