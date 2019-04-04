namespace VertexWave
{
    public interface IEnviroment
    {
        long LastUpdate { get; }
        void UpdatedLight(byte light, int id);
    }
}