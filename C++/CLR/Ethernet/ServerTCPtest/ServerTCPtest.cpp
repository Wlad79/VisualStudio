// ServerTCPtest.cpp: Hauptprojektdatei.

#include "stdafx.h"

using namespace System;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Threading;
using namespace System::Text;

void main()
{
	Socket^ server = gcnew Socket(AddressFamily::InterNetwork, SocketType::Stream, ProtocolType::Tcp);
	IPEndPoint^ iped = gcnew IPEndPoint(IPAddress::Parse("127.0.0.1"), 12345);
	server->Connect(iped);

	array<unsigned char>^ msg = gcnew array<unsigned char>(1024);
	int rcv = server->Receive(msg);

	Console::WriteLine(Encoding::ASCII->GetString(msg, 0, rcv));

	msg = Encoding::ASCII->GetBytes("input");
	server->Send(msg, msg->Length, SocketFlags::None);

	msg = gcnew array<unsigned char>(1024);
	rcv = server->Receive(msg);
	Console::WriteLine(Encoding::ASCII->GetString(msg, 0, rcv));
	server->Shutdown(SocketShutdown::Both);
	server->Close();
}
