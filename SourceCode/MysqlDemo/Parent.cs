using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MysqlDemo
{
    class Parent
    {

        /// <summary>
        /// id
        /// </summary>
        public Guid id { set; get; }

        public string name { set; get; }


        public DateTime birthday { get; set; }


        public bool isWorking { get; set; }


        public int age { get; set; }


        public double tall { get; set; }

        //为表格准备的索引字段
        public int tableIndex {get;set;}


    }
}
