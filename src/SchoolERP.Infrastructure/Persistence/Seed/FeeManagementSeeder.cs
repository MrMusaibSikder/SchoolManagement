using Microsoft.EntityFrameworkCore;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;
using SchoolERP.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Persistence.Seed
{
    public static class FeeManagementSeeder
    {
        public static async Task SeedAsync(SchoolERPDbContext context)
        {
            if (!context.FeeCategories.Any())
            {
                var categories = new List<FeeCategory>
        {
            new() { Name = "Academic", DisplayOrder = 1 },
            new() { Name = "Transport", DisplayOrder = 2 },
            new() { Name = "Activity", DisplayOrder = 3 }
        };
                await context.FeeCategories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            if (!context.FeeTypes.Any())
            {
                // Name দিয়ে আসল Id লুকআপ করুন, hardcode না করে
                var academicId = await context.FeeCategories
                    .Where(c => c.Name == "Academic")
                    .Select(c => c.Id)
                    .FirstAsync();

                var activityId = await context.FeeCategories
                    .Where(c => c.Name == "Activity")
                    .Select(c => c.Id)
                    .FirstAsync();

                var types = new List<FeeType>
        {
            new()
            {
                Name = "Tuition Fee",
                Code = "TUI",
                FeeCategoryId = academicId,
                Frequency = FeeFrequency.Monthly,
                IsMandatory = true,
                DefaultDueDayOfMonth = 10,
                DefaultGracePeriodDays = 5
            },
            new()
            {
                Name = "Exam Fee",
                Code = "EXM",
                FeeCategoryId = academicId,
                Frequency = FeeFrequency.Termly,
                IsMandatory = true
            },
            new()
            {
                Name = "Sports Fee",
                Code = "SPT",
                FeeCategoryId = activityId,
                Frequency = FeeFrequency.Yearly,
                IsMandatory = false
            }
        };
                await context.FeeTypes.AddRangeAsync(types);
                await context.SaveChangesAsync();
            }
        }
    }

}
