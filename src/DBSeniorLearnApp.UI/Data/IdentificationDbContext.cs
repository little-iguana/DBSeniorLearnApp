using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DBSeniorLearnApp.UI.Data
{
    public class IdentificationDbContext : IdentityDbContext
    {
        public IdentificationDbContext(DbContextOptions<IdentificationDbContext> options)
            : base(options)
        {
        }
    }
}
