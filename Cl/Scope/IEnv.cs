using Cl.Types;

namespace Cl.Scope
{
    public interface IEnv
    {
        bool Bind(IClObj symbol, IClObj obj);
        IClObj Lookup(IClObj symbol);
        bool Assign(IClObj symbol, IClObj obj);
    }
}
