using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace L5_RiPOD
{
    class DrawGraph
    {
        public TextBox OutputBox { get; set; }
        public List<Operation> opList { get; set; }

        public DrawGraph(TextBox outputBox, List<Operation> opList)
        {
            this.opList = opList;
            OutputBox = outputBox;
        }

        public void DrawTextInfo()
        {
            OutputBox.Clear();
            double angle = 360.0 / opList.Count;
            angle = angle * Math.PI / 180;

            for (int i = 0; i < opList.Count; i++)
            {
                Operation op = opList[i];
                OutputBox.AppendText($"Операция {i + 1}:\r\n");
                OutputBox.AppendText($"  - Блок состояния: {op.StateBlock}\r\n");
                OutputBox.AppendText($"  - Время выполнения: {op.TimeCalculation}\r\n");

                string vectorStr = string.Join(", ", op.VectorList);
                OutputBox.AppendText($"  - Вектор: ({vectorStr})\r\n");

                OutputBox.AppendText($"  - Связи:\r\n");
                foreach (string conn in op.ConnectionList)
                {
                    OutputBox.AppendText($"    -> {conn}\r\n");
                }

                OutputBox.AppendText("\r\n");
            }
        }
    }
}