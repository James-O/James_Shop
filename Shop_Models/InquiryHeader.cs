﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop_Models
{
    public class InquiryHeader
    {
        [Key]
        public int Id { get; set; } 
        public string ApplicaionUserId { get; set; }
        [ForeignKey(nameof(ApplicaionUserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime InquiryDate { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
