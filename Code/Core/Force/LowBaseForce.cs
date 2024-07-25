using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AncientWarfare.Core.Force
{
    public abstract class LowBaseForce
    {
        public BaseForceData BaseData { get; protected internal set; }
        public string GetName()
        {
            if (string.IsNullOrEmpty(BaseData.name))
            {
                BaseData.name = NewName();
            }
            return BaseData.name;
        }
        public abstract string NewName();
    }
    public abstract class BaseForce<TData> : LowBaseForce where TData : BaseForceData
    {
        public TData Data { get; private set; }
        public BaseForce(TData data)
        {
            Data = data;
            BaseData = data;
        }
    }
}
