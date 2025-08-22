using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAppERP.Models
{
    public class JournalEntry
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ngày ghi sổ")]
        public DateTime EntryDate { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Diễn giải")]
        public string Description { get; set; }

        public virtual ICollection<JournalEntryLine> Lines { get; set; }
    }
}