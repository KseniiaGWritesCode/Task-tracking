using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class MigrationValidator
    {
        public static void ValidateModelAndMigrations(DbContext db)
        {
            var migrationsAssembly = db.GetService<IMigrationsAssembly>();
            var modelDiffer = db.GetService<IMigrationsModelDiffer>();

            var lastMigration = migrationsAssembly.Migrations.LastOrDefault();
            if (lastMigration.Value == null)
                return;

            var snapshotModel = migrationsAssembly
                .ModelSnapshot?
                .Model;

            if (snapshotModel == null)
                return;

            var runtimeModel = db.Model;

            var diffs = modelDiffer.GetDifferences(snapshotModel.GetRelationalModel(), runtimeModel.GetRelationalModel());

            if (diffs.Any())
            {
                throw new InvalidOperationException("Entities are changed, but migration not found, create new migration!");
            }
        }
    }
}
