namespace Interfaces
{
    public interface IClicable
    {
        public string GetInfo();
        
    }

    public interface IChargable
    {
        public int GetEnergy(int deficite);
    }
    
    // funkcja zwracająca List<Resources>
}