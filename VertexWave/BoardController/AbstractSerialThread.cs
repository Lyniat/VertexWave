﻿/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */


using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;

/**
 * This class contains methods that must be run from inside a thread and others
 * that must be invoked from Unity. Both types of methods are clearly marked in
 * the code, although you, the final user of this library, don't need to even
 * open this file unless you are introducing incompatibilities for upcoming
 * versions.
 */
public abstract class AbstractSerialThread
{
    // Amount of milliseconds alloted to a single read or connect. An
    // exception is thrown when such operations take more than this time
    // to complete.
    private const int ReadTimeout = 100;

    // Amount of milliseconds alloted to a single write. An exception is thrown
    // when such operations take more than this time to complete.
    private const int WriteTimeout = 100;
    private readonly int _baudRate;
    private readonly int _delayBeforeReconnecting;

    private readonly bool _enqueueStatusMessages;

    // Internal synchronized queues used to send and receive messages from the
    // serial device. They serve as the point of communication between the
    // Unity thread and the Ardity thread.
    private readonly Queue _inputQueue;

    private readonly int _maxUnreadMessages;
    private readonly Queue _outputQueue;

    // Parameters passed from SerialController, used for connecting to the
    // serial device as explained in the SerialController documentation.
    private readonly string _portName;

    // Object from the .Net framework used to communicate with serial devices.
    private SerialPort _serialPort;

    // Indicates when this thread should stop executing. When SerialController
    // invokes 'RequestStop()' this variable is set.
    private bool _stopRequested;


    /**************************************************************************
     * Methods intended to be invoked from the Unity thread.
     *************************************************************************/

    // ------------------------------------------------------------------------
    // Constructs the thread object. This object is not a thread actually, but
    // its method 'RunForever' can later be used to create a real Thread.
    // ------------------------------------------------------------------------
    public AbstractSerialThread(string portName,
        int baudRate,
        int delayBeforeReconnecting,
        int maxUnreadMessages,
        bool enqueueStatusMessages)
    {
        _portName = portName;
        _baudRate = baudRate;
        _delayBeforeReconnecting = delayBeforeReconnecting;
        _maxUnreadMessages = maxUnreadMessages;
        _enqueueStatusMessages = enqueueStatusMessages;

        _inputQueue = Queue.Synchronized(new Queue());
        _outputQueue = Queue.Synchronized(new Queue());
    }

    // ------------------------------------------------------------------------
    // Invoked to indicate to this thread object that it should stop.
    // ------------------------------------------------------------------------
    public void RequestStop()
    {
        lock (this)
        {
            _stopRequested = true;
        }
    }

    // ------------------------------------------------------------------------
    // Polls the internal message queue returning the next available message
    // in a generic form. This can be invoked by subclasses to change the
    // type of the returned object.
    // It returns null if no message has arrived since the latest invocation.
    // ------------------------------------------------------------------------
    public object ReadMessage()
    {
        if (_inputQueue.Count == 0)
            return null;

        return _inputQueue.Dequeue();
    }

    // ------------------------------------------------------------------------
    // Schedules a message to be sent. It writes the message to the
    // output queue, later the method 'RunOnce' reads this queue and sends
    // the message to the serial device.
    // ------------------------------------------------------------------------
    public void SendMessage(object message)
    {
        _outputQueue.Enqueue(message);
    }


    /**************************************************************************
     * Methods intended to be invoked from the Ardity thread (the one
     * created by the SerialController).
     *************************************************************************/

    // ------------------------------------------------------------------------
    // Enters an almost infinite loop of attempting connection to the serial
    // device, reading messages and sending messages. This loop can be stopped
    // by invoking 'RequestStop'.
    // ------------------------------------------------------------------------
    public void RunForever()
    {
        // This 'try' is for having a log message in case of an unexpected
        // exception.
        try
        {
            while (!IsStopRequested())
                try
                {
                    AttemptConnection();

                    // Enter the semi-infinite loop of reading/writing to the
                    // device.
                    while (!IsStopRequested())
                        RunOnce();
                }
                catch (Exception ioe)
                {
                    // A disconnection happened, or there was a problem
                    // reading/writing to the device. Log the detailed message
                    // to the console and notify the listener.
                    Console.WriteLine("Exception: " + ioe.Message + " StackTrace: " + ioe.StackTrace);
                    if (_enqueueStatusMessages)
                        _inputQueue.Enqueue(SerialController.SerialDeviceDisconnected);

                    // As I don't know in which stage the SerialPort threw the
                    // exception I call this method that is very safe in
                    // disregard of the port's status
                    CloseDevice();

                    // Don't attempt to reconnect just yet, wait some
                    // user-defined time. It is OK to sleep here as this is not
                    // Unity's thread, this doesn't affect frame-rate
                    // throughput.
                    Thread.Sleep(_delayBeforeReconnecting);
                }

            // Before closing the COM port, give the opportunity for all messages
            // from the output queue to reach the other endpoint.
            while (_outputQueue.Count != 0) SendToWire(_outputQueue.Dequeue(), _serialPort);

            // Attempt to do a final cleanup. This method doesn't fail even if
            // the port is in an invalid status.
            CloseDevice();
        }
        catch (Exception e)
        {
            Console.WriteLine("Unknown exception: " + e.Message + " " + e.StackTrace);
        }
    }

    // ------------------------------------------------------------------------
    // Try to connect to the serial device. May throw IO exceptions.
    // ------------------------------------------------------------------------
    private void AttemptConnection()
    {
        _serialPort = new SerialPort(_portName, _baudRate);
        _serialPort.ReadTimeout = ReadTimeout;
        _serialPort.WriteTimeout = WriteTimeout;
        _serialPort.RtsEnable = true;
        _serialPort.Open();

        if (_enqueueStatusMessages)
            _inputQueue.Enqueue(SerialController.SerialDeviceConnected);
    }

    // ------------------------------------------------------------------------
    // Release any resource used, and don't fail in the attempt.
    // ------------------------------------------------------------------------
    private void CloseDevice()
    {
        if (_serialPort == null)
            return;

        try
        {
            _serialPort.Close();
        }
        catch (IOException)
        {
            // Nothing to do, not a big deal, don't try to cleanup any further.
        }

        _serialPort = null;
    }

    // ------------------------------------------------------------------------
    // Just checks if 'RequestStop()' has already been called in this object.
    // ------------------------------------------------------------------------
    private bool IsStopRequested()
    {
        lock (this)
        {
            return _stopRequested;
        }
    }

    // ------------------------------------------------------------------------
    // A single iteration of the semi-infinite loop. Attempt to read/write to
    // the serial device. If there are more lines in the queue than we may have
    // at a given time, then the newly read lines will be discarded. This is a
    // protection mechanism when the port is faster than the Unity progeram.
    // If not, we may run out of memory if the queue really fills.
    // ------------------------------------------------------------------------
    private void RunOnce()
    {
        try
        {
            // Send a message.
            if (_outputQueue.Count != 0) SendToWire(_outputQueue.Dequeue(), _serialPort);

            // Read a message.
            // If a line was read, and we have not filled our queue, enqueue
            // this line so it eventually reaches the Message Listener.
            // Otherwise, discard the line.
            var inputMessage = ReadFromWire(_serialPort);
            if (inputMessage != null)
            {
                if (_inputQueue.Count < _maxUnreadMessages)
                    _inputQueue.Enqueue(inputMessage);
                else
                    Console.WriteLine("Queue is full. Dropping message: " + inputMessage);
            }
        }
        catch (TimeoutException)
        {
            //Console.WriteLine("time out!");
        }
    }

    // ------------------------------------------------------------------------
    // Sends a message through the serialPort.
    // ------------------------------------------------------------------------
    protected abstract void SendToWire(object message, SerialPort serialPort);

    // ------------------------------------------------------------------------
    // Reads and returns a message from the serial port.
    // ------------------------------------------------------------------------
    protected abstract object ReadFromWire(SerialPort serialPort);
}