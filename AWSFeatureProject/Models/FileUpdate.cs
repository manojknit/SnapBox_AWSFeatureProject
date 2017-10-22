using System;
using System.ComponentModel.DataAnnotations;

namespace AWSFeatureProject.Models
{
    public class FileUpdate
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name", Description ="First Name")]
        public string firstName { get; set; }
        [Required]
        [Display(Name = "Last Name", Description = "Last Name")]
        public string lastname { get; set; }
        [Required]
        [Display(Name = "Email", Description = "Email")]
        public string email { get; set; }

        [Required]
        [Display(Name = "File Uploaded On", Description = "File Uploaded On")]
        public DateTime upload_date { get; set; }

        [Display(Name = "Last Updated On", Description = "File Last Updated On")]
        public DateTime updated_date { get; set; }

        [Required]
        [Display(Name = "Uploaded File Name", Description = "Uploaded File Name")]
        public string file_name { get; set; }

        [Display(Name = "File Detail", Description = "File Detail")]
        public string file_desc { get; set; }

    }
}
