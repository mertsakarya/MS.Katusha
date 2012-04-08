namespace MS.Katusha.Services.Generators
{
    public interface IGenerator<out T>
    {
        T Generate();
    }
}