using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tests.Metadata
{
    class DataMapMock
    {

        public string Column1 { get; set; }

        [Column("ColumnB")]
        public string Column2 { get; set; }

        [NotMapped]
        public string Column3 { get; set; }

        [Key]
        public string Column4 { get; set; }

    }
}
