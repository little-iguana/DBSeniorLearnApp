using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using DBSeniorLearnApp.DataAccess.Models;

namespace DBSeniorLearnApp.DataAccess;

public class ServiceDbContext : DbContext
{
	public DbSet<Member> Members { get; set; }
	public DbSet<ProfessionalMember> ProfessionalMembers { get; set; }
	public DbSet<Course> Courses { get; set; }
	public DbSet<CourseEnrolment> CourseEnrolments { get; set; }
	
	public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options) {}
	
	
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Remove(typeof(CascadeDeleteConvention));
    }
	
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		/*{
			in the lambdas, the parameter letters are abbreviations:
				e -> entity
				c -> column
				n -> navigation property
			to try to make it easier to know what is supposed to be what,
			even when naming is unclear
		}*/
		
		modelBuilder.Entity<Member>(entity => {
			entity.Property(c => c.Firstname)
				  .HasMaxLength(255);
			
			entity.Property(c => c.Lastname)
				  .HasMaxLength(255);
			
			entity.Property(c => c.MemberNumber)
				  .HasMaxLength(9);

			entity.Property(c => c.PhoneNumber)
				  .HasMaxLength(11); // idk how long a phone number is
		});
		
		modelBuilder.Entity<Member>()
			  .HasAlternateKey(c => c.MemberNumber);
		
		modelBuilder.Entity<Member>()
			  .Ignore(c => c.Password);
		
		modelBuilder.Entity<ProfessionalMember>()
			  .HasKey(c => c.StandardMemberId);
		modelBuilder.Entity<ProfessionalMember>()
			  .HasOne(n => n.StandardMember)
			  .WithOne(c => c.ProfessionalMember);
		modelBuilder.Entity<ProfessionalMember>()
			  .HasMany(c => c.Courses)
			  .WithOne(e => e.Instructor);
		
		// InstructorId has to be the foreign key to Member (Id)
		modelBuilder.Entity<Course>(entity => {
			entity.Property(c => c.Title)
				  .HasMaxLength(50);
			
			entity.Property(c => c.Description)
				  .HasMaxLength(255);
			
			entity.Property(c => c.Prerequisites)
				  .HasMaxLength(255);
		});
		modelBuilder.Entity<Course>()
			  .HasOne(n => n.Instructor)
			  .WithMany(n => n.Courses);
		
		modelBuilder.Entity<CourseEnrolment>()
			  .HasOne(e => e.Member)
			  .WithMany(e => e.CourseEnrolments);
		modelBuilder.Entity<CourseEnrolment>()
			  .HasOne(e => e.Course)
			  .WithMany(e => e.CourseEnrolments);
	}
}






