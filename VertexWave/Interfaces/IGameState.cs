namespace VertexWave.Interfaces
{
    public interface IGameState
    {
        void LostGame();

        void StartedGame();

        void LoadedGame();
    }
}