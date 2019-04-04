namespace VertexWave.BoardController
{
    public interface IMessageListener
    {
        void OnConnectionEvent(bool status);
        void OnMessageArrived(string message);
    }
}