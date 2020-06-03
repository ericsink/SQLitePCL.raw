using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppSqliteTesting
{
    public class Entity
    {
        #region PROPERTIES
        // Event Log Identifier
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// </summary>
        public long EventId { get; set; } = -1;

        /// <summary>
        /// </summary>
        public int ClassId { get; set; } = -1;

        /// <summary>
        /// </summary>
        public string ClassName { get; set; } = "";

        /// <summary>
        /// </summary>
        public int GroupId { get; set; } = -1;

        /// <summary>
        /// </summary>
        public string GroupName { get; set; } = "";

        /// <summary>
        /// </summary>
        public string TechnicalComponentId { get; set; } = "";

        /// <summary>
        /// </summary>
        public int StateId { get; set; } = -1;

        /// <summary>
        /// </summary>
        public string StateName { get; set; } = "";

        /// <summary>
        /// Optional data
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Optional data
        /// </summary>
        public string Parameter1 { get; set; } = "";

        /// <summary>
        /// Optional data
        /// </summary>
        public string Parameter2 { get; set; } = "";

        /// <summary>
        /// Optional data
        /// </summary>
        public string Parameter3 { get; set; } = "";

        /// <summary>
        /// Optional data
        /// </summary>
        public string Parameter4 { get; set; } = "";

        /// <summary>
        /// </summary>
        public DateTime EventDateTime { get; set; }

        /// <summary>
        /// </summary>
        public DateTime EventDateTimeUtc { get; set; }

        /// <summary>
        /// </summary>
        public DateTime CreatedDateTimeUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// </summary>
        public string CreatedBy { get; set; } = "";

        /// <summary>
        /// Message Id of the source message that was converted to this event
        /// </summary>
        public string SourceMessageId { get; set; } = "";
        #endregion
    }
}
