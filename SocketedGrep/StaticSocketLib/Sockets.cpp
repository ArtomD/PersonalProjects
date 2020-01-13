// TCPServer
#include "pch.h"
#include "Sockets.h"


//unsigned short constexpr PORT = 27015;
//string IPAddressString = "127.0.0.1";

namespace sockets {

	InboundSocket::InboundSocket(short PORT) {
		WSAData wsaData;
		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
			throw std::runtime_error("WSAStartup failed");
		}
		hSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
		sockaddr_in serverAddress = { 0 };
		serverAddress.sin_family = AF_INET;
		serverAddress.sin_port = htons(PORT);
		inet_pton(AF_INET, "0.0.0.0", &(serverAddress.sin_addr));
		int opt = 1;
		
		//setsockopt(hSocket, SOL_SOCKET, SO_REUSEADDR, (char*)&opt, sizeof(opt));

		int error = bind(hSocket, (SOCKADDR*)& serverAddress, sizeof(serverAddress));
		error = WSAGetLastError();
		if (error == SOCKET_ERROR) {
			closesocket(hSocket);
			WSACleanup();
			throw std::runtime_error("Socket binding failed");
		}

		if (listen(hSocket, 10) == SOCKET_ERROR) {
			int temp = WSAGetLastError();
			closesocket(hSocket);
			WSACleanup();
			throw std::runtime_error("Socket listen failed ");
		}
		
	}

	void InboundSocket::acceptConnection() {
		InboundSocket::hAccepted = SOCKET_ERROR;
		while (hAccepted == SOCKET_ERROR) {
			hAccepted = accept(hSocket, NULL, NULL);
		}
	}

	std::string InboundSocket::getLocalAddress() {
		struct sockaddr_in localAddress;
		socklen_t addressLength = sizeof(localAddress);
		getsockname(hSocket, (struct sockaddr*) & localAddress, &addressLength);
		char str[INET_ADDRSTRLEN];
		inet_ntop(AF_INET, &(localAddress.sin_addr), str, INET_ADDRSTRLEN);
		return str;
	}

	InboundSocket::~InboundSocket() {
		closesocket(hSocket);
		WSACleanup();
	}

	CommandData InboundSocket::receiveCommand() {
		CommandData data = { Standby ,0, {0} };
		data.command = Standby;
		int incomingSize = 0;
		char intBuf[sizeof(int)];
		char boolBuf[sizeof(bool)];
		recv(hAccepted, intBuf, 4, 0);
		int i = 0;
		std::copy(
			&intBuf[0],
			&intBuf[0] + sizeof(int),
			reinterpret_cast<char*>(&i)
		);
		bool b = false;
		std::copy(
			&boolBuf[0],
			&boolBuf[0] + sizeof(bool),
			reinterpret_cast<char*>(&b)
		);
		data.command = static_cast<Command>(i);
		if (data.command == Standby || data.command == Exit || data.command == Connect || data.command == Params) {
			return data;
		}else if (data.command == IncomingBool) {
			data.size = sizeof(bool);
			data.data = new char[sizeof(bool)];
			recv(hAccepted, data.data, sizeof(bool), 0);
			return data;
		}
		else if (data.command == IncomingInt) {
			data.size = sizeof(int);
			data.data = new char[sizeof(int)];
			recv(hAccepted, data.data, data.size, 0);
			return data;
		}
		else if (data.command == IncomingString) {
			recv(hAccepted, intBuf, 4, 0);
			int i = 0;
			std::copy(
				&intBuf[0],
				&intBuf[0] + sizeof(int),
				reinterpret_cast<char*>(&i)
			);
			data.size = i;
			data.data = new char[data.size+1];
			data.data[data.size] = 0x00;
			recv(hAccepted, data.data, data.size, 0);
			return data;
		}
		return data;
	}

	void OutboundSocket::sendIntToSocket(int payload) {
		char ch[sizeof(int)] = {};
		std::copy(
			reinterpret_cast<const char*>(&payload),
			reinterpret_cast<const char*>(&payload) + sizeof(int),
			&ch[0]
		);
		send(hSocket, ch, sizeof(payload), 0);
	}

	void OutboundSocket::sendBoolToSocket(bool payload) {
		char ch[sizeof(bool)] = {};
		std::copy(
			reinterpret_cast<const char*>(&payload),
			reinterpret_cast<const char*>(&payload) + sizeof(bool),
			&ch[0]
		);
		send(hSocket, ch, sizeof(payload), 0);
	}

	OutboundSocket::OutboundSocket(short PORT) {
		WSAData wsaData;
		if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0) {
			throw std::runtime_error("WSAStartup failed");
		}
		hSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

	}

	bool OutboundSocket::attemptConnection(std::string IPAddressString, short PORT) {
		sockaddr_in serverAddressOut = { 0 };
		serverAddressOut.sin_family = AF_INET;
		serverAddressOut.sin_port = htons(PORT);
		inet_pton(AF_INET, IPAddressString.c_str(), &(serverAddressOut.sin_addr));
		return (!(connect(hSocket, (SOCKADDR*)& serverAddressOut, sizeof(serverAddressOut)) == SOCKET_ERROR));
	}

	std::string OutboundSocket::getLocalAddress() {
		struct sockaddr_in localAddress;
		socklen_t addressLength = sizeof(localAddress);
		getsockname(hSocket, (struct sockaddr*) & localAddress, & addressLength);
		char str[INET_ADDRSTRLEN];
		inet_ntop(AF_INET, &(localAddress.sin_addr), str, INET_ADDRSTRLEN);
		return str;
	}

	void OutboundSocket::sendPayload(bool payload) {
		//send command
		sendIntToSocket(IncomingBool);
		//send payload
		sendBoolToSocket(payload);
	}

	void OutboundSocket::sendPayload(int payload) {
		//send command
		sendIntToSocket(IncomingInt);
		//send payload
		sendIntToSocket(payload);
	}

	void OutboundSocket::sendPayload(std::string payload) {
		//send command
		sendIntToSocket(IncomingString);
		//send size to expect
		sendIntToSocket(strlen(payload.c_str()));
		//send payload
		send(hSocket, payload.c_str(), strlen(payload.c_str()), 0);
	}

	void OutboundSocket::sendParams(bool verbose, std::string path, std::string regex, std::string extensions) {
		sendIntToSocket(Params);
		sendPayload(verbose);
		sendPayload(path);
		sendPayload(regex);
		sendPayload(extensions);
	}

	void OutboundSocket::openConnection(std::string IPAddress, short port) {
		sendIntToSocket(Connect);
		sendPayload(port);
		sendPayload(IPAddress);
	}

	void OutboundSocket::closeConnection() {
		//send command
		sendIntToSocket(Exit);
	}

	void OutboundSocket::shutdownServer() {
		sendIntToSocket(ShutdownServer);
	}

	OutboundSocket::~OutboundSocket() {
		closesocket(hSocket);
		WSACleanup();
	}

}
