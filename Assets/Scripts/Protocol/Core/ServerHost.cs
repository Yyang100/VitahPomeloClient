

#define LOCAL       // 内网

public static class ServerHost
{
	#if LOCAL
	private static string ip = "127.0.0.1";
	#endif

	private static int port = 3010;

	public static string Ip {
		get {
			return ip;
		}
	}

	public static int Port {
		get {
			return port;
		}
	}
}
