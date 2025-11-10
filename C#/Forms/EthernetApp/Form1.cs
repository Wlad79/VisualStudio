using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Drawing.Printing;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("Server-Socket", "Abfrage", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                //code yes

                Thread workerThread = new Thread(ServerThread);
                
                // Start the worker thread.
                workerThread.Start();
            }

            else
            {

                //code No: Client
                string host;
                int port = 8080;

                //host = Dns.GetHostName();
                //host = Dns.GetHostAddresses();
                host = "localhost";
                //host = "192.168.178.38";
                //host = "178.1.18.239";

                string result = SocketSendReceive(host, port);
                
                //System.Windows.Forms.MessageBox.Show("Client-Socket erfolgreich erstellt: " + host, "title", System.Windows.Forms.MessageBoxButtons.OK);

                MessageBox.Show("TCP-Client wurde erstellt");
            }
            
        }

        private static Socket ConnectSocket(string server, int port)
        {
            Socket s = null;
            IPHostEntry hostEntry = null;

            // Get host related information.
            //hostEntry = Dns.GetHostEntry(server);
            //hostEntry = Dns.GetHostEntry("localhost");
            //hostEntry = Dns.Resolve("localhost");
                hostEntry = Dns.Resolve("192.168.178.38");
            //hostEntry = Dns.Resolve("178.1.18.239");

            // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
            // an exception that occurs when the host IP Address is not compatible with the address family
            // (typical in the IPv6 case).
            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new IPEndPoint(address, port);
                Socket tempSocket =
                    new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    s = tempSocket;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return s;
        }

        // This method requests the home page content for the specified server.
        private static string SocketSendReceive(string server, int port)
        {
            string request = "GET / HTTP/1.1\r\nHost: " + server +
                "\r\nConnection: Close\r\n\r\n";
            Byte[] bytesSent = Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];

            // Create a socket connection with the specified server and port.
            Socket s = ConnectSocket(server, port);

            if (s == null)
                return ("Connection failed");

            // Send request to the server.
            s.Send(bytesSent, bytesSent.Length, 0);

            MessageBox.Show("Client hat gesendet");

            s.Receive(bytesReceived);
            string msg = System.Text.UTF8Encoding.UTF8.GetString(bytesReceived);
            MessageBox.Show("Client hat empfangen: " + msg );

            // Receive the server home page content.
            int bytes = 0;
            string page = "Default HTML page on " + server + ":\r\n";

            // The following will block until te page is transmitted.
            //do
            //{
            //    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
            //    page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            //}
            //while (bytes > 0);

            return page;
        }

        private static void ServerThread()
        {
            byte[] msg = Encoding.UTF8.GetBytes("This is a test");
            //String host = "localhost";
            TcpListener server = null;

            // Set the TcpListener on port 80.
            Int32 port = 8080;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            //IPAddress localAddr = IPAddress.Parse("192.168.178.27");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            // Enter the listening loop.
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                MessageBox.Show("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    MessageBox.Show("Server hat empfangen: " + data);

                    // Process the data sent by the client.
                    data = data.ToUpper();

                    msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }

                // Shutdown and end connection
                client.Close();
            }
        }

        //Drucken des RichTextBox-Inhalts am Drucker
        StringReader Ausleser;
        Font printFont;
        private void button2_Click(object sender, EventArgs e)
        {
            printFont = new Font("Arial", 14);
            MessageBox.Show( richTextBox1.Text);
            PrintDialog printDialog1 = new PrintDialog();
            PrintDocument ThePrintDocument = new PrintDocument();
            ThePrintDocument.PrintPage += new PrintPageEventHandler
               (this.ThePrintDocument_PrintPage);
            printDialog1.Document = ThePrintDocument;
            string strText = this.richTextBox1.Text;
            Ausleser = new StringReader(strText);
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                ThePrintDocument.Print();
                
            }
            Ausleser.Close();
        }


         //The PrintPage event is raised for each page to be printed.
        private void ThePrintDocument_PrintPage(object sender, PrintPageEventArgs ev)
        {
            float linesPerPage = 0;
            float yPos = 0;
            int count = 0;
            float leftMargin = ev.MarginBounds.Left;
            float topMargin = ev.MarginBounds.Top;
            string line = null;

            // Calculate the number of lines per page.
            linesPerPage = ev.MarginBounds.Height /
               printFont.GetHeight(ev.Graphics);

            // Print each line of the file.
            while (count < linesPerPage &&
               ((line = Ausleser.ReadLine()) != null))
            {
                yPos = topMargin + (count *
                   printFont.GetHeight(ev.Graphics));
                ev.Graphics.DrawString(line, printFont, Brushes.Black,
                   leftMargin, yPos, new StringFormat());
                count++;
            }

            ev.Graphics.DrawLine(new Pen(Color.Black, 2), new Point(100, 100), new Point(500, 100));
            ev.Graphics.DrawLine(new Pen(Color.Black, 2), new Point(100, 200), new Point(500, 200));

            // If more lines exist, print another page.
            if (line != null)
                ev.HasMorePages = true;
            else
                ev.HasMorePages = false;
        }

        //Öffnet eine Datei in RichTextBox1
        private void button3_Click(object sender, EventArgs e)
        {
            // Displays an OpenFileDialog so the user can select a Cursor.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Text Files|*.txt";
            openFileDialog1.Title = "Select a Text File";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader sr = new StreamReader(openFileDialog1.FileName);
                //MessageBox.Show(sr.ReadToEnd());
                this.richTextBox1.Text = (sr.ReadToEnd());
                sr.Close();
            }

            
        }

        //Speichert den Inhalt aus RichTextBox in ausgewählte Datei
        private void button4_Click(object sender, EventArgs e)
        {
            // Displays a SaveFileDialog so the user can save the Image
            // assigned to Button2.
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Text|*.txt|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
            sw.Write(richTextBox1.Text);
            sw.Close();
        }

    }
}
