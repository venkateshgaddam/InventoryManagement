namespace Carrier.CCP.Common.Utils
{
    public interface ICcpAttribute<out T>
    {
        T Value { get; }
    }
}