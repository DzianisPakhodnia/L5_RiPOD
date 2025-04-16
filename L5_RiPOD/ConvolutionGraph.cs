using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace L5_RiPOD
{
   class ConvolutionGraph
    {
        public int OperationsCount;
        public int TypesCount;
        public int[][] ArrayTypes;
        private int[] ProcessorsByTypesCount;
        private int[] TimeCalculationByTypes;
        public int[][] ArrayH;
        public List<List<int>> List_chains = new List<List<int>>();

        public List<List<Operation>> StepList = new List<List<Operation>>();
        public List<string> SeqParFunc = new List<string>();

        public ConvolutionGraph()
        {
            OperationsCount = 0;
            TypesCount = 0;
            ArrayH = null;
            ArrayTypes = null;
            ProcessorsByTypesCount = null;
        }

        public void File_Load(string filestr)
        {
            // получение данных из файла

            StreamReader file = new StreamReader(filestr);
            //step 1 чтение кол-ва операций
            string buff = "";
            buff = file.ReadLine();

            buff = Find_value(buff);
            OperationsCount = Convert.ToInt32(buff);

            //step 2 чтение кол-ва типов операций
            buff = "";
            buff = file.ReadLine();
            buff = Find_value(buff);
            TypesCount = Convert.ToInt32(buff);

            //step 3 чтение операций каждого типа
            ArrayTypes = new int[TypesCount][];
            for (int i = 0; i < TypesCount; i++)
            {
                buff = "";
                buff = file.ReadLine();
                buff = Find_value(buff);
                ArrayTypes[i] = GetArray(buff);
            }

            //step 4 чтение количества процессоров каждого типа
            ProcessorsByTypesCount = new int[TypesCount];
            for (int i = 0; i < TypesCount; i++)
            {
                buff = "";
                buff = file.ReadLine();
                buff = Find_value(buff);
                ProcessorsByTypesCount[i] = Convert.ToInt32(buff);
            }


            //step 5 чтение времени выполнения операции каждого типа
            TimeCalculationByTypes = new int[TypesCount];
            for (int i = 0; i < TypesCount; i++)
            {
                buff = "";
                buff = file.ReadLine();
                buff = Find_value(buff);
                TimeCalculationByTypes[i] = Convert.ToInt32(buff);
            }

            //step 6 чтение таблицы смежности
            buff = "";
            buff = file.ReadLine();

            ArrayH = new int[OperationsCount][];

            for (int i = 0; i < OperationsCount; i++)
            {
                buff = "";
                buff = file.ReadLine();
                ArrayH[i] = GetArray(buff);
            }

            file.Close();
        }

        public void Planning()
        {
            GetChains();
            BeginnerGraph();
            Convolution();
        }


        #region Получение цепочек
        
        private void GetChains()
        {
            // построить цепи зависимостей
            List<int> listUse = new List<int>();

            List<int> chain = new List<int>();
            int operation = 0;

            while (listUse.Count < OperationsCount)
            {
                operation = 0;
                while (listUse.Contains(operation))
                {
                    operation++;
                }

                chain.Clear();
                chain.Add(operation);
                listUse.Add(operation);
                while (true)
                {
                    // найти в строке таблицы смежности следующее направление
                    bool index = false;
                    for (int i = 0; i < ArrayH[operation].Length; i++)
                    {
                        if (ArrayH[operation][i] == 1)
                        {
                            index = true;
                            operation = i;
                            break;
                        }
                    }

                    if (index) // если найдено следующее значение в цепи
                    {
                        chain.Add(operation);
                        if (!listUse.Contains(operation))
                        {
                            listUse.Add(operation);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                List_chains.Add(new List<int>(chain));
            }
        }

        private string Find_value(string s)
        {
            int index = s.IndexOf(':') + 2;
            return s.Substring(index);   // извлечение подстроки с указанной позиции и до конца            
        }

        private int[] GetArray(string s)
        {

            string[] nums = s.Split(' ');
            int[] arr = new int[nums.Length];
            for (int i = 0; i < nums.Length; i++)
            {
                arr[i] = int.Parse(nums[i]);
            }

            return arr;
        }

        #endregion

        #region Начальный граф

        // Постройка начального графа
        private void BeginnerGraph()
        {
            List<Operation> operations = new List<Operation>();

            for (int i = 0; i < OperationsCount; i++)
            {
                Operation op = new Operation();
                int type = FindType(i + 1); 
                op.Value = (i + 1).ToString();
                op.StateBlock = op.Value.ToString();
                op.TimeCalculation = TimeCalculationByTypes[type];

                op.VectorList = new List<int>();
                op.ConnectionList = new List<string>();

                for (int k = 0; k < TypesCount; k++)
                {
                    op.VectorList.Add(k == type ? 1 : 0);
                }

                List<int> list = FindChains(i);

                foreach (var chain in List_chains)
                {
                    foreach (var t in chain)
                    {
                        if (!list.Contains(t) && !op.ConnectionList.Contains((t + 1).ToString()))
                        {
                            op.ConnectionList.Add((t + 1).ToString());
                        }
                    }
                }
                op.ConnectionList.Sort();
                operations.Add(op);
            }
            
            StepList.Add(operations);
            
        }

        // Поиск типа операции по ее значению
        private int FindType(int value)
        {
            int res = -1;
            for (int i = 0; i < TypesCount; i++)
            {
                if (ArrayTypes[i].Contains(value))
                {
                    res = i;
                    break;
                }
            }
            return res;
        }

        // Поиск всех цепочек, в которых присутствует операция с указанным значением
        private List<int> FindChains(int value)
        {
            List<int> list = new List<int>();
            foreach (var t in List_chains)
            {
                if (t.Contains(value))
                {
                    list.AddRange(t);
                }
            }
            return list;
        }

        #endregion

        #region Свертывание графа
        
        // Свертывание
        private void Convolution()
        {
            int step = 0;
            List<Operation> opList = FullCloneObject(StepList[step]);
            while (FindAnyConnections(opList) != 0) // пока не останется 1 операция в графе
            {
                step++;
                opList = FullCloneObject(StepList[step - 1]); // предыдущее значение графа
                int countMissedConnections = 0; // количество связей, разрешенных потерять 
                try
                {
                    for (;;)
                    {
                        for (int i = 0; i < opList.Count; i++)
                        {
                            for (int k = 0; k < opList.Count - 1; k++)
                            {
                                if (i == k)
                                    continue;

                                List<string> list = FindCommonConnections(opList[i], opList[k]);
                                int connect = Convert.ToInt32(HaveConnection(opList[i], opList[k]));
                                if (
                                    (Math.Abs(Math.Max(opList[i].ConnectionList.Count, opList[k].ConnectionList.Count) - 
                                    (list.Count + connect))) > countMissedConnections)
                                {
                                    continue;
                                }
                                else
                                {
                                    opList = ConnectOperations(opList, i, k);
                                    throw new Exception("Выход из 3 for-ов");
                                }
                            }
                        }
                        countMissedConnections++;
                        if (countMissedConnections > OperationsCount)
                            break;
                    }
                }
                catch (Exception e)
                {
                    
                }
                finally 
                {
                    // выделение памяти и созранение в StepList
                    List<Operation> memList = new List<Operation>();
                    memList = FullCloneObject(opList);
                    StepList.Add(memList);
                }
            }

            // Соеденить все оставшеся вершины последовательно
            while (opList.Count > 1)
            {
                opList = ConnectOperations(opList, 0, 1);
                List<Operation> memList = new List<Operation>();
                memList = opList.ToList();
                StepList.Add(memList);
            }
        }

        // глубокое копирование
        private List<Operation> FullCloneObject(List<Operation> opList)
        {
            List<Operation> list = new List<Operation>();
            foreach (var t in opList)
            {
                Operation aOperation = new Operation();

                aOperation.Value = t.Value;
                aOperation.StateBlock = t.StateBlock;
                aOperation.TimeCalculation = t.TimeCalculation;
                aOperation.ConnectionList = new List<string>();
                foreach (var val in t.ConnectionList)
                {
                    aOperation.ConnectionList.Add(val);
                }

                aOperation.VectorList = new List<int>();
                foreach (var val in t.VectorList)
                {
                    aOperation.VectorList.Add(val);
                }

                list.Add(aOperation);
            }

            return list;
        }
       
        // проверка на наличие связи между операциями
        private bool HaveConnection(Operation op1, Operation op2)
        {
            if (op1.ConnectionList.Contains(op2.Value))
                return true;
            return false;
        }
       
        // поиск всех связей в графе
        private int FindAnyConnections(List<Operation> opList)
        {
            int count = 0;
            foreach (var op in opList)
            {
                count += op.ConnectionList.Count;
            }
            count = count / 2;
            return count;
        }

        // поиск общих связей
        private List<string> FindCommonConnections(Operation l1, Operation l2)
        {
            List<string> commonOperations = new List<string>();

            foreach (var op in l1.ConnectionList)
            {
                if (op != l2.Value) // исключить проверку возможной связи между операциями
                {
                    if (l2.ConnectionList.Contains(op))
                    {
                        commonOperations.Add(op);
                    }
                }
            }

            return commonOperations;
        }

        // соединение операций в одну
        private List<Operation> ConnectOperations(List<Operation> opList, int indexOp1, int indexOp2)
        {
            opList = ChangeConnectionsWithOperation(opList, opList[indexOp1], opList[indexOp2]);
            opList = CalculateNewResources(opList, opList[indexOp1], opList[indexOp2]);
            opList = DeleteConnectionsWithOperation(opList, opList[indexOp2]);
            opList = DeleteAllInvalidConnections(opList);
            return opList;
        }

        // уделение операции из графа со всеми связями
        private List<Operation> DeleteConnectionsWithOperation(List<Operation> opList, Operation operation)
        {
            foreach (var op in opList)
            {
                if (op.Value != operation.Value)
                {
                    op.ConnectionList.Remove(operation.Value); 
                }
            }
            opList.Remove(operation);
            return opList;
        }

        // изменение соединений в графе в результате соединения операций
        private List<Operation> ChangeConnectionsWithOperation(List<Operation> opList, Operation op1, Operation op2)
        {
            
            for (var i = 0; i < opList.Count; i++)
            {
                var op = opList[i];
                if (op.Value != op1.Value && op.ConnectionList.Contains(op1.Value))
                {
                    int index = opList[i].ConnectionList.IndexOf(op1.Value);
                    opList[i].ConnectionList[index] = op1.Value + op2.Value;
                    
                }
            }
           return opList;
        }

        // вычисление новых параметров вершины графа
        private List<Operation> CalculateNewResources(List<Operation> opList, Operation op1, Operation op2)
        {
            string planFunc;
            if (HaveConnection(op1, op2))
            {
                planFunc = "par";
            }
            else
            {
                planFunc = "seq";
            }
            int index = opList.IndexOf(op1);
            op1 = WriteSeqParFunc(planFunc, op1, op2);
            opList[index] = op1;
            for (int i = 0; i < opList.Count; i++)
            {
                if (opList[i].Value == op1.Value)
                {
                    var y = opList[i];
                    y.Value = op1.Value + op2.Value;

                    switch (planFunc)
                    {
                        case "par":
                        {
                            y.TimeCalculation = Math.Max(op1.TimeCalculation, op2.TimeCalculation);
                            y.VectorList = VectorSum(op1.VectorList, op2.VectorList);
                            break;
                        }
                        case "seq":
                        {
                            y.TimeCalculation = op1.TimeCalculation + op2.TimeCalculation;
                            y.VectorList = VectorMax(op1.VectorList, op2.VectorList);
                            break;
                        }
                    }
                    
                    opList[i] = y;
                }
            }

            return opList;
        }

        // запись операции в хранилищще
        private Operation WriteSeqParFunc(string planFunc, Operation op1, Operation op2)
        {
            // запись функции плана в хранилице
            SeqParFunc.Add(planFunc + "(" + op1.StateBlock + ", " + op2.StateBlock + ")");
            switch (planFunc)
            {
                case "seq":
                {
                    op1.StateBlock = op1.StateBlock + "-" +  op2.StateBlock;
                    break;
                }
                case "par":
                {
                    op1.StateBlock = "[" + op1.StateBlock + "/" + op2.StateBlock + "]";
                    break;
                }
            }

            return op1;
        }

        // суммирование векторов
        private List<int> VectorSum(List<int> vector1, List<int> vector2)
        {
            for (int i = 0; i < vector1.Count; i++)
            {
                vector1[i] = vector1[i] + vector2[i];
            }
            return vector1;
        }

        // нахождение вектора максимумов
        private List<int> VectorMax(List<int> vector1, List<int> vector2)
        {
            for (int i = 0; i < vector1.Count; i++)
            {
                vector1[i] = Math.Max(vector1[i], vector2[i]);
            }
            return vector1;
        }

        // сравнение суммы векторов с ограничением на процессоры
        private bool Compare(List<int> obj1, List<int> obj2)
        {
            for (var i = 0; i < obj1.Count; i++)
            {
                if ((obj1[i] + obj2[i]) > ProcessorsByTypesCount[i])
                {
                    return false;
                }
            }
            
            return true;
        }

        // удаление всех связей, которые невозможны из-за ограничения на процессоры
        private List<Operation> DeleteAllInvalidConnections(List<Operation> opList)
        {
            for (var i = 0; i < opList.Count; i++)
            {
                for (int k = 0; k < opList[i].ConnectionList.Count; k++)
                {
                    int index = -1;
                    string val = opList[i].ConnectionList[k];
                    for (int t = 0; t < opList.Count; t++)
                    {
                        if (opList[t].Value == opList[i].ConnectionList[k])
                        {
                            index = t;
                            break;
                        }
                    }

                    if (!Compare(opList[i].VectorList, opList[index].VectorList))
                    {
                        opList[i].ConnectionList.Remove(val);
                        opList[index].ConnectionList.Remove(val);
                        k = -1;
                    }
                }
                
            }
            return opList;
        }

        #endregion
      
    }
}
 
