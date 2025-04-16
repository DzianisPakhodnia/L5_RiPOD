using System.Collections.Generic;

namespace L5_RiPOD
{
    struct Operation
    {
        public string Value { get; set; }
        public int TimeCalculation { get; set; }
        public string StateBlock { get; set; }
        public List<int> VectorList;
        public List<string> ConnectionList;
    }
}
