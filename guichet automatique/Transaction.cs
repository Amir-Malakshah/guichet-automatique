using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace guichet_automatique
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal Montant { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public int CompteId { get; set; }
    }
}
