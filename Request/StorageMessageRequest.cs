using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace POC.StorageQueue.Request
{
    public class StorageMessageRequest
    {
        public int QuantidadeCarga { get; set; } = 1;
    }
}
