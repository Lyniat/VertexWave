/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using System.Threading;
using VertexWave.BoardController;

/**
 * This class allows a Unity program to continually check for messages from a
 * serial device.
 *
 * It creates a Thread that communicates with the serial port and continually
 * polls the messages on the wire.
 * That Thread puts all the messages inside a Queue, and this SerialController
 * class polls that queue by means of invoking SerialThread.GetSerialMessage().
 *
 * The serial device must send its messages separated by a newline character.
 * Neither the SerialController nor the SerialThread perform any validation
 * on the integrity of the message. It's up to the one that makes sense of the
 * data.
 */
public class SerialController
{
    // ------------------------------------------------------------------------
    // Executes a user-defined function before Unity closes the COM port, so
    // the user can send some tear-down message to the hardware reliably.
    // ------------------------------------------------------------------------
    public delegate void TearDownFunction();

    // Constants used to mark the start and end of a connection. There is no
    // way you can generate clashing messages from your serial device, as I
    // compare the references of these strings, no their contents. So if you
    // send these same strings from the serial device, upon reconstruction they
    // will have different reference ids.
    public const string SerialDeviceConnected = "__Connected__";
    public const string SerialDeviceDisconnected = "__Disconnected__";

    private readonly IMessageListener _messageListener;
    private TearDownFunction _userDefinedTearDownFunction;

    public int baudRate = 1000000;

    public int maxUnreadMessages = 20;
    public string portName = "COM16";

    public int reconnectionDelay = 1;
    protected SerialThreadLines serialThread;

    // Internal reference to the Thread and the object that runs in it.
    protected Thread thread;


    // ------------------------------------------------------------------------
    // Invoked whenever the SerialController gameobject is activated.
    // It creates a new thread that tries to connect to the serial device
    // and start reading from it.
    // ------------------------------------------------------------------------
    public SerialController(IMessageListener messageListener)
    {
        _messageListener = messageListener;

        serialThread = new SerialThreadLines(portName,
            baudRate,
            reconnectionDelay,
            maxUnreadMessages);
        thread = new Thread(serialThread.RunForever);
        thread.Start();
    }

    // ------------------------------------------------------------------------
    // Invoked whenever the SerialController gameobject is deactivated.
    // It stops and destroys the thread that was reading from the serial device.
    // ------------------------------------------------------------------------
    private void OnDisable()
    {
        // If there is a user-defined tear-down function, execute it before
        // closing the underlying COM port.
        if (_userDefinedTearDownFunction != null)
            _userDefinedTearDownFunction();

        // The serialThread reference should never be null at this point,
        // unless an Exception happened in the OnEnable(), in which case I've
        // no idea what face Unity will make.
        if (serialThread != null)
        {
            serialThread.RequestStop();
            serialThread = null;
        }

        // This reference shouldn't be null at this point anyway.
        if (thread != null)
        {
            thread.Join();
            thread = null;
        }
    }

    // ------------------------------------------------------------------------
    // Polls messages from the queue that the SerialThread object keeps. Once a
    // message has been polled it is removed from the queue. There are some
    // special messages that mark the start/end of the communication with the
    // device.
    // ------------------------------------------------------------------------
    public void Update()
    {
        //var length = 0;// port.ReadByte();
        //_messageListener.OnMessageArrived("" + length);

        //Console.WriteLine("buffer length: " + length);

        // If the user prefers to poll the messages instead of receiving them
        // via SendMessage, then the message listener should be null.
        if (_messageListener == null) return;


        // Read the next message from the queue
        var message = (string) serialThread.ReadMessage();
        if (message == null) return;
        // Check if the message is plain data or a connect/disconnect event.

        if (ReferenceEquals(message, SerialDeviceConnected))
            _messageListener.OnConnectionEvent(true);
        else if (ReferenceEquals(message, SerialDeviceDisconnected))
            _messageListener.OnConnectionEvent(false);
        else
            _messageListener.OnMessageArrived(message);
    }

    // ------------------------------------------------------------------------
    // Returns a new unread message from the serial device. You only need to
    // call this if you don't provide a message listener.
    // ------------------------------------------------------------------------
    public string ReadSerialMessage()
    {
        // Read the next message from the queue
        return (string) serialThread.ReadMessage();
    }

    // ------------------------------------------------------------------------
    // Puts a message in the outgoing queue. The thread object will send the
    // message to the serial device when it considers it's appropriate.
    // ------------------------------------------------------------------------
    public void SendSerialMessage(string message)
    {
        serialThread.SendMessage(message);
    }

    public void SetTearDownFunction(TearDownFunction userFunction)
    {
        _userDefinedTearDownFunction = userFunction;
    }
}