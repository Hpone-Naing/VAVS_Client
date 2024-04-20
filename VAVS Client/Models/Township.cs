﻿namespace VAVS_Client.Models
{
    [Table("TB_Township")]
    public class Township
    {
        [Key]
        public int TownshipPkid { get; set; }

        [Required]
        [StringLength(15)]
        public string TownshipCode { get; set; }

        [StringLength(100)]
        public string TownshipName { get; set; }

        [StringLength(5)]
        public string DistrictCode { get; set; }
    }
}
