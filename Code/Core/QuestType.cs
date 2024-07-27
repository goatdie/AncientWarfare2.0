using AncientWarfare.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core
{
    public class QuestType : IManager
    {
        public static readonly QuestType ResourceCollect;
        public static readonly QuestType TypedResourceCollect;
        public static readonly QuestType Custom;
        private string id;
        QuestType()
        {

        }
        public static QuestType Create(string id)
        {
            return new QuestType
            {
                id = id
            };
        }
        public override string ToString()
        {
            return id;
        }
        public override bool Equals(object obj)
        {
            return obj is QuestType type &&
                   id == type.id;
        }
        public override int GetHashCode()
        {
            return id?.GetHashCode() ?? 0;
        }

        public void Initialize()
        {
            var fields = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(QuestType))
                {
                    field.SetValue(null, Create(field.Name));
                }
            }
        }
    }
}
