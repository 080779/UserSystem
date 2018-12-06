using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Service.Entity
{
  public class TransferEntity : BaseEntity
    {
        public long UserId { get; set; }
        //public long payTypeId { get; set; }
        public virtual UserEntity User { get; set; }
        public decimal Amount { get; set; }
        public int ChangeType { get; set; }
    }
}
