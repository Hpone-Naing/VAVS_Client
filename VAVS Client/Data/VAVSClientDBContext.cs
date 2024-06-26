﻿namespace VAVS_Client.Data
{
    public class VAVSClientDBContext : DbContext
    {
        public VAVSClientDBContext(DbContextOptions<VAVSClientDBContext> options) : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
        public virtual DbSet<PersonalDetail> PersonalDetails { get; set; }
        public virtual DbSet<StateDivision> StateDivisions { get; set; }
        public virtual DbSet<Township> Townships { get; set; }
        public virtual DbSet<Township1> Township1s { get; set; }
        public virtual DbSet<Fuel> Fuels { get; set; }
        public virtual DbSet<VehicleStandardValue> VehicleStandardValues { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<TaxValidation> TaxValidations { get; set; }
        public virtual DbSet<NRC_And_Township> NRC_And_Townships { get; set; }

    }
}
