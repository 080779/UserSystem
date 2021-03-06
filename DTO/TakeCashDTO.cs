﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DTO
{
    public class TakeCashDTO:BaseDTO
    {
        public long UserId { get; set; }
        public string Mobile { get; set; }
        public string Code { get; set; }
        public string NickName { get; set; }
        public long StateId { get; set; }
        public decimal? Amount { get; set; }
        public string Description { get; set; }
        public string StateName { get; set; }
        public long PayTypeId { get; set; }
        public string PayTypeName { get; set; }
        public PayCodeDTO PayCode { get; set; }
        public BankAccountDTO BankAccount { get; set; }
        public string AdminMobile { get; set; }
        public string partner_trade_no { get; set; }
        public string payment_no { get; set; }
        public DateTime? payment_time { get; set; }
    }
}
