using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMCS.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedDate { get; set; }

        // Foreign Key to link the document to a claim
        public int ClaimId { get; set; }
        [ForeignKey("ClaimId")]
        public Claim Claim { get; set; }
    }
}