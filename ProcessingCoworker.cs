using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTracking
{
    public class ProcessingCoworker
    {
        public string Name { get; set; }
        public string Birthday { get; set; }
        public DateTime BirthdayFilnal { get; set; }
        public string EMail { get; set; }
        public string Position { get; set; }
        public Position PositionFinal { get; set; }

        public ProcessingCoworker(List<string> input)
        {
            if (input.Count >= 4)
            {
                Name = input[0];
                Birthday = input[1];
                EMail = input[2];
                Position = input[3];
            }

            // выкиунть исключение если длина меньше 4
        }

        public Coworker TransferToCoworkerItem(string name, DateTime birthday, string eMail, Position position)
        {
            Name = name;
            BirthdayFilnal = birthday;
            EMail = eMail;
            PositionFinal = position;

            Coworker coworkerItem = new(Name, BirthdayFilnal, EMail, PositionFinal);
            return coworkerItem;
        }
    }
}
// 'Kseniia Gerasimenko AKA Big Boss' '12.07.1994' 'theonlykseniia@company.com' 'Designer'