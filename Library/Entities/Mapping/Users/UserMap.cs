using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using Entities.Domain.Users;

namespace Entities.Mapping.Users
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            this.ToTable("User");
            this.HasKey(u => u.Id);

            this.HasMany(u => u.UserRoles)
                .WithMany()
                .Map(m => m.ToTable("User_UserRole_Mapping"));

            this.HasMany(u => u.Departments)
                .WithMany()
                .Map(m => m.ToTable("User_Department_Mapping"));
            //this.HasMany(u => u.Lines)
            //    .WithMany(l=>l.Users)
            //    .Map(m => m.ToTable("User_Line_Mapping"));

            this.HasMany(u => u.Dmses)
                .WithMany(d=>d.Users)
                .Map(m => m.ToTable("User_Dms_Mapping"));

            this.HasMany(u => u.Measures)
                .WithMany(m=>m.Users)
                .Map(m => m.ToTable("User_Measure_Mapping"));
        }
    }
}
