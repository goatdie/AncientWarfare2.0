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

        public abstract void Update(float elapsed);
        public abstract string NewName();
    }

    public abstract class BaseForce<TData> : LowBaseForce where TData : BaseForceData
    {
        public BaseForce(TData data)
        {
            Data = data;
            BaseData = data;
        }

        public TData Data { get; private set; }
    }
}