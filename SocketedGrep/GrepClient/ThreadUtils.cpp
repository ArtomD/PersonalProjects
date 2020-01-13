#include "ThreadUtils.h"

std::thread UIThread;
std::thread inboundSocketCollectorThread;
std::thread* inboundSocketThread;
bool connectedToServer = false;
bool stopInboundSocketThread = false;
bool exitApplication = false;
bool hasConnected = false;
bool reconnect = false;
HANDLE inboundSocketOpen;
HANDLE inboundSocketThreadUp;

sockets::OutboundSocket* outboundSocket = nullptr;
sockets::InboundSocket* inboundSocket = nullptr;
unsigned const short clientPort = 27025;
unsigned const short hostPort = 27015;
std::string hostAddress;
std::string clientAddress = "0.0.0.0";

void connectToServer();

//runs the listening thread for the inbound socket
void inbound_socket_thread_runner() {
	SetEvent(inboundSocketThreadUp);
	try {
		inboundSocket = new sockets::InboundSocket(clientPort);
	}
	catch (std::exception e) {
		AddText(e.what());
		AddText("Failed to open port.");
	}
	//alerts the outbound thread to request a connection
	SetEvent(inboundSocketOpen);
	//listens for server to connect
	inboundSocket->acceptConnection();
	//recieves output from the server until it is done
	sockets::CommandData data = inboundSocket->receiveCommand();
	while (data.command != sockets::Exit && !stopInboundSocketThread) {
		if (data.command == sockets::IncomingString) {
			AddText(data.data);
		}
		data = inboundSocket->receiveCommand();
	}
	AddText("");
	//closes the listening socket
	outboundSocket->closeConnection();
	delete inboundSocket;
}
//runs the UI
void ui_thread_runner() {
	std::vector<INPUT_RECORD> events(128);
	while (!exitApplication) {
		DWORD numEvents;
		ReadConsoleInput(hConsoleInput, events.data(), (DWORD)events.size(), &numEvents);
		for (decltype(numEvents) i = 0; i < numEvents; ++i) {
			if (events[i].EventType == KEY_EVENT) {
				//listens for key events
				ProcessKeyEvent(events[i].Event.KeyEvent);
			}
		}
		//if a command has been given proccess it
		if (hasCommand) {
			std::vector<std::string> commandV = split_string(commandString, ' ');
			if (commandV[0].compare("grep")==0) {
				//if connected to a server try to run a grep with the supplied params
				if (connectedToServer) {
					bool verbose = false;
					int argc = 2;
					if (commandV[1].compare("-v")==0) {
						verbose = true;
						++argc;
					}
					std::string ext = ".txt";
					if (commandV.size() >= 5) {
						ext = commandV[4];
					}
					if (commandV.size()< (argc+1)) {
						AddText("Incorrect format for grep. Please type -help for documentation");
					}
					else {
						outboundSocket->sendParams(verbose, commandV[argc-1], commandV[argc], ext);
					}					
				}
				else {
					AddText("Not curently connected to any server");
				}
			}
			//if conected to a server disconnect
			else if (commandV[0].compare("drop") == 0) {
				if (connectedToServer) {
					reconnect = false;
					stopInboundSocketThread = true;
					connectedToServer = false;
					outboundSocket->closeConnection();
					AddText("Disconnected from " + hostAddress);
				}
				else {
					AddText("Not connected to a server");
				}
				
			}
			//if not connected to a server atempt a connection
			else if (commandV[0].compare("connect") == 0) {
				if (connectedToServer) {
					AddText("Already connected to " + hostAddress);
				}
				else if (commandV.size() <= 1) {
					AddText("Please provide an IP address to connect to.");
				}
				else {
					reconnect = true;
					hostAddress = commandV[1];					
					connectToServer();
					if (connectedToServer) {
						AddText("Connected to " + hostAddress);
					}					
				}
			}
			//is connected to a server send a shut down request
			else if (commandV[0].compare("stopserver") == 0) {
				if (connectedToServer) {
					reconnect = false;
					outboundSocket->shutdownServer();
					connectedToServer = false;
					AddText("Shutting down server");
				}
				else {
					AddText("Not connected to a server");
				}
			}
			//exit the client interface and close the program
			else if (commandV[0].compare("exit") == 0) {
				exitApplication = true;
			}
			//adjust the display size based on input
			else if (commandV[0].compare("display") == 0) {
				try {
					int width = std::stoi(commandV[1]);
					if (width > 500 || width < 5) {
						AddText("Please enter a more resonable width");
					}
					else {
						int height = 0;
						if (commandV.size() >= 3) {
							height = std::stoi(commandV[2]);
						}
						if ((height > 100 || height < 5) && height !=0) {
							AddText("Please enter a more resonable height");
						}
						else {
							ChangeDisplaySize(width, height);
						}
						
					}
					
				}
				catch (std::exception) {
					AddText("Please enter integers");
				}
				
			}
			//display the help to the screen
			else if (commandV[0].compare("-help") == 0) {
				print_help();
			}
			//if cannot be parsed
			else {
				AddText("Command not recognized. Type -help for documentation" );
			}
			//reset the command flag
			hasCommand = false;
		}		
	}
}

//function that open an outbound socket to target server and tries to open a connection
//if succesfull atempts to open a listening inbound socket and then connect it to the server
void connectToServer() {
	try {
		outboundSocket = new sockets::OutboundSocket(clientPort);
	}
	catch (std::exception e) {
		connectedToServer = false;
		AddText(e.what());
		AddText("Failed to connect to server.");
		return;
	}
	if (!(outboundSocket->attemptConnection(hostAddress, hostPort))) {
		connectedToServer = false;
		AddText("Failed to connect to server.");
	}
	else {
		stopInboundSocketThread = false;
		//launches the inbound socket thread and waits for it to be ready
		inboundSocketOpen = CreateEvent(NULL, false, false, NULL);
		inboundSocketThread = new std::thread(inbound_socket_thread_runner);
		WaitForSingleObject(inboundSocketOpen, INFINITE);
		//once inbound socket is ready and listening tells server to connect to it
		outboundSocket->openConnection(outboundSocket->getLocalAddress(), clientPort);
		connectedToServer = true;
	}
}
//this thread cleans up all inbound socket threads
void socket_collection() {
	for (;;) {
		WaitForSingleObject(inboundSocketThreadUp, INFINITE);
		if (exitApplication) {
			return;
		}
		inboundSocketThread->join();
		delete inboundSocketThread;
		delete outboundSocket;
		if (reconnect) {
			connectToServer();
		}
	}
}
//starts the main ui thread and launches the socket cleanup thread
void start_ui_thread() {
	setupScreen();
	UIThread = std::thread(ui_thread_runner);
	inboundSocketThreadUp = CreateEvent(NULL, false, false, NULL);
	inboundSocketCollectorThread = std::thread(socket_collection);
	UIThread.join();
	SetEvent(inboundSocketThreadUp);
	inboundSocketCollectorThread.join();
	restoreWindow();
}

