namespace VertexWave.Interfaces
{
    public interface IWorldGenerator
    {
        ChunkStruct GetChunkAt(int xPos, int zPos);
    }
}