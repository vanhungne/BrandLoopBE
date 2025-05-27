using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrandLoop.Domain.Entities
{
    public class Skill
    {
        [Key]
        public int SkillId { get; set; }

        [Required]
        [StringLength(32)]
        public string UID { get; set; }

        [Required]
        [StringLength(100)]
        public string SkillName { get; set; }

        [StringLength(50)]
        public string ProficiencyLevel { get; set; } // Beginner, Intermediate, Expert

        // Navigation properties
        [ForeignKey("UID")]
        public virtual User User { get; set; }
    }
}
