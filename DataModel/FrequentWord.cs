using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace TextProcessor.DataModel
{
    [Index("Word", IsUnique = true)]
    class FrequentWord
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Word { get; set; }
        [Required]
        public int Frequent { get; set; }
    }
}
