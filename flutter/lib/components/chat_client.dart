import 'dart:io';

class ChatClient {
  late Socket _socket;
  late String _address;
  late int _port;

  ChatClient(Socket s) {
    _socket = s;
    _address = _socket.remoteAddress.address;
    _port = _socket.remotePort;

    _socket.listen(messageHandler,
        onError: errorHandler, onDone: finishedHandler);
  }

  void messageHandler(List data) {
    String message = data.cast<String>().join();
    print('${_address}:${_port} message: $message');
  }

  void errorHandler(error) {
    print('${_address}:${_port} Error: $error');
    _socket.close();
  }

  void finishedHandler() {
    print('${_address}:${_port} Disconnected');
    _socket.close();
  }

  void write(String message) {
    _socket.write(message);
  }
}
