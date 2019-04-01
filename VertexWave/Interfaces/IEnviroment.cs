using System;
namespace VertexWave
{
    public interface IEnviroment
    {
        void UpdatedLight(byte light, int id);

        long LastUpdate { get; }
    }
}
