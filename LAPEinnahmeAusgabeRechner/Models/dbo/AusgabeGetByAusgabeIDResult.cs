﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LAPEinnahmeAusgabeRechner.Models.dboSchema
{
    public partial class AusgabeGetByAusgabeIDResult
    {
        public long AUSGABEID { get; set; }
        public long KATEGORIEID { get; set; }
        public string KATEGORIEBEZEICHNUNG { get; set; }
        public string KATEGORIEFARBE { get; set; }
        public string BESCHREIBUNG { get; set; }
        [Column("BETRAG", TypeName = "decimal(18,2)")]
        public decimal BETRAG { get; set; }
        public DateTime DATUM { get; set; }
        public short VALID { get; set; }
        public string MOD_USER { get; set; }
        public DateTime MOD_TIMESTAMP { get; set; }
        public string CR_USER { get; set; }
        public DateTime CR_TIMESTAMP { get; set; }
    }
}
