using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                    new Employee
                    {
                        Id = 1,
                        Name = "Abe",
                        Email = "aberuzz@pragimtech.com",
                        Department = Dept.HR
                    },
                    new Employee
                    {
                        Id = 2,
                        Name = "Dingdong",
                        Email = "dingdong@pragimtech.com",
                        Department = Dept.HR
                    }
                );
        }
    }
}
